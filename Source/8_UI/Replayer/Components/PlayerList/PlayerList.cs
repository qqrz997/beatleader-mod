﻿using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BeatLeader.Models;
using BeatLeader.Models.AbstractReplay;
using BeatLeader.Utils;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BeatLeader.Components {
    internal class PlayerList : ReeListComponentBase<PlayerList, IVirtualPlayer, PlayerList.Cell> {
        #region Cell

        public class Cell : ReeTableCell<Cell, IVirtualPlayer> {
            #region UI Components

            [UIComponent("mini-profile"), UsedImplicitly]
            private QuickMiniProfile _miniProfile = null!;

            [UIComponent("background-image"), UsedImplicitly]
            private AdvancedImage _backgroundImage = null!;

            [UIComponent("score-text"), UsedImplicitly]
            private TMP_Text _scoreText = null!;

            #endregion

            #region Setup

            protected override string Markup { get; } = BSMLUtility.ReadMarkupOrFallback(
                "PlayerListCell", Assembly.GetExecutingAssembly()
            );

            private IBeatmapTimeController _timeController = null!;
            private IReplayScoreEventsProcessor? _scoreEventsProcessor;
            private IVirtualPlayer? _previousVirtualPlayer;
            private PlayerList _playerList = null!;

            public void Setup(IBeatmapTimeController timeController) {
                _timeController = timeController;
            }

            protected override void Init(IVirtualPlayer player) {
                if (_scoreEventsProcessor is not null) {
                    _scoreEventsProcessor.ScoreEventDequeuedEvent -= HandleScoreEventDequeued;
                }
                _scoreEventsProcessor = player.ReplayScoreEventsProcessor;
                _scoreEventsProcessor.ScoreEventDequeuedEvent += HandleScoreEventDequeued;
                _playerList = (PlayerList)List!;
                RefreshPlayer();
                RefreshScore();
                if (_previousVirtualPlayer != null && _previousVirtualPlayer != player) {
                    _playerList.NotifyCellReadyForUpdate();
                }
                _previousVirtualPlayer = player;
            }

            private void RefreshPlayer() {
                var replay = Item!.Replay;
                var replayData = replay.ReplayData;
                _miniProfile.SetPlayer(replayData.Player!);
                enabled = true;
            }

            protected override void OnInitialize() {
                //TODO: load color from the replay data
                enabled = false;
                var color = new Color(
                    Random.Range(0, 100) / 100f,
                    Random.Range(0, 100) / 100f,
                    Random.Range(0, 100) / 100f
                );
                //required to fill the TEXCOORD1 with non-sliced UV
                _backgroundImage.ImageView.gradient = true;
                LoadBackgroundMaterial(color);
            }

            protected override void OnDispose() {
                if (_scoreEventsProcessor is not null) {
                    _scoreEventsProcessor.ScoreEventDequeuedEvent -= HandleScoreEventDequeued;
                }
                Destroy(_backgroundFillMaterial);
            }

            private void Update() {
                UpdateFillAnimation();
            }

            #endregion

            #region Fill Animation

            private static readonly int fillAspectProperty = Shader.PropertyToID("_FillAspect");
            private static readonly int colorProperty = Shader.PropertyToID("_Color");
            private static readonly int backgroundColorProperty = Shader.PropertyToID("_BackgroundColor");

            private Material _backgroundFillMaterial = null!;

            private void UpdateFillAnimation() {
                var songTime = _timeController.SongTime;
                var replayFinishTime = Item!.Replay.ReplayData.FinishTime;
                var aspect = MathUtils.Map(songTime, 0, replayFinishTime, 0, 1);
                _backgroundFillMaterial.SetFloat(fillAspectProperty, aspect);
            }

            private void LoadBackgroundMaterial(Color color) {
                _backgroundFillMaterial = Instantiate(BundleLoader.OpponentBackgroundMaterial);
                _backgroundFillMaterial.SetColor(colorProperty, color);
                var backgroundColor = color.ColorWithAlpha(0.4f);
                _backgroundFillMaterial.SetColor(backgroundColorProperty, backgroundColor);
                _backgroundImage.Material = _backgroundFillMaterial;
            }

            #endregion

            #region Move Animation

            private bool _receivedEventWhileAnimating;
            private bool _isAnimating;

            public Vector2 ReportPosition() {
                return ContentTransform!.position;
            }

            public void NotifyPositionUpdated(Vector2 oldPosition) {
                if (_isAnimating) return;
                var destination = ContentTransform!.position;
                ContentTransform!.position = oldPosition;
                var coroutine = LinearMoveCoroutine(0.4f, 120f, oldPosition, destination);
                RoutineFactory.StartUnmanagedCoroutine(coroutine);
            }

            private IEnumerator LinearMoveCoroutine(float time, float framerate, Vector3 origin, Vector3 destination) {
                _isAnimating = true;
                var totalFrames = time * framerate;
                var timePerFrame = time / framerate;
                var vectorSubtr = destination - origin;
                var vectorSummand = vectorSubtr / totalFrames;
                var tr = ContentTransform!;
                tr.position = origin;
                for (var i = 0f; i < totalFrames; i++) {
                    yield return new WaitForSeconds(timePerFrame);
                    tr.position += vectorSummand;
                }
                _isAnimating = false;
                if (_receivedEventWhileAnimating) {
                    _playerList.NotifyCellReadyForUpdate();
                }
            }

            #endregion

            #region Score

            private void RefreshScore(LinkedListNode<ScoreEvent>? node = null) {
                node ??= _scoreEventsProcessor!.CurrentScoreEvent;
                _scoreText.text = $"score: {node?.Value.score ?? 0}";
            }

            #endregion

            #region Callbacks

            private void HandleScoreEventDequeued(LinkedListNode<ScoreEvent> node) {
                if (_scoreEventsProcessor!.QueueIsBeingAdjusted) return;
                RefreshScore(node);
                if (_isAnimating) {
                    _receivedEventWhileAnimating = true;
                } else {
                    _playerList.NotifyCellReadyForUpdate();
                }
            }

            public override void OnStateChange(bool selected, bool highlighted) { }

            #endregion
        }

        #endregion

        #region Sorting

        private class VirtualPlayerComparator : IComparer<IVirtualPlayer> {
            public int Compare(IVirtualPlayer x, IVirtualPlayer y) {
                var xScore = GetScore(x);
                var yScore = GetScore(y);
                return Comparer<int>.Default.Compare(yScore, xScore);
            }

            private static int GetScore(IVirtualPlayer player) {
                return player.ReplayScoreEventsProcessor.CurrentScoreEvent?.Value.score ?? 0;
            }
        }

        private readonly VirtualPlayerComparator _virtualPlayerComparator = new();

        private void RefreshSorting() {
            items.Sort(_virtualPlayerComparator);
        }

        #endregion

        #region Setup

        protected override float CellSize => 20f;

        private IBeatmapTimeController? _timeController;
        private readonly HashSet<Cell> _cells = new();

        public void Setup(IBeatmapTimeController timeController) {
            _timeController = timeController;
        }

        protected override void OnCellConstruct(Cell cell) {
            ValidateAndThrow();
            cell.Setup(_timeController!);
            _cells.Add(cell);
        }

        protected override bool OnValidation() {
            return _timeController is not null;
        }

        #endregion

        #region Cell Animation

        private readonly Dictionary<IVirtualPlayer, Vector2> _cellPositions = new();
        
        private void SaveCellsPosition() {
            foreach (var cell in _cells) {
                if (cell.Item is not { } item) continue;
                var position = cell.ReportPosition();
                _cellPositions[item] = position;
            }
        }

        private void StartCellsAnimation() {
            foreach (var cell in _cells) {
                if (!_cellPositions.TryGetValue(cell.Item!, out var oldPosition)) continue;
                var actualCellPosition = cell.ReportPosition();
                if (actualCellPosition == oldPosition) continue;
                cell.NotifyPositionUpdated(oldPosition);
            }
        }

        private void NotifyCellReadyForUpdate() {
            StartCoroutine(RefreshCoroutine());
        }

        private IEnumerator RefreshCoroutine() {
            yield return new WaitForSeconds(0.1f);
            Refresh();
        }

        #endregion

        #region Callbacks

        protected override void OnEarlyRefresh() {
            SaveCellsPosition();
            RefreshSorting();
            _cells.Clear();
        }

        protected override void OnRefresh() {
            StartCellsAnimation();
        }

        #endregion
    }
}