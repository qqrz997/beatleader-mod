﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BeatLeader.Models;
using BeatSaberMarkupLanguage;
using HMUI;
using Reactive;
using Reactive.BeatSaber.Components;
using Reactive.Components;
using UnityEngine;

namespace BeatLeader.UI.Hub {
    internal class BeatmapFilterPanel : ReactiveComponent, IPanelListFilter<IReplayHeaderBase> {
        #region Dummies

        private class NotSelectedPreviewMediaData : IPreviewMediaData {
            public Task<Sprite> GetCoverSpriteAsync(CancellationToken cancellationToken) {
                return Task.FromResult(BundleLoader.UnknownIcon);
            }

            public Task<AudioClip?> GetPreviewAudioClip(CancellationToken cancellationToken) {
                return Task.FromResult<AudioClip?>(null);
            }

            public void UnloadPreviewAudioClip() { }
        }
        
        private static readonly BeatmapLevel previewBeatmapLevel = new(
            0,
            false,
            "",
            "Click to select",
            null,
            null,
            Array.Empty<string>(),
            Array.Empty<string>(),
            0f,
            0f,
            0f,
            0f, 
            0f,
            0f, 
            PlayerSensitivityFlag.Safe,
            new NotSelectedPreviewMediaData(),
            null
        );

        #endregion

        #region Filter

        public IEnumerable<IPanelListFilter<IReplayHeaderBase>>? DependsOn => null;
        public string FilterName => "Beatmap Filter";
        public BeatmapLevelWithKey BeatmapLevel { get; private set; }

        public event Action? FilterUpdatedEvent;

        public bool Matches(IReplayHeaderBase value) {
            if (!BeatmapLevel.HasValue) return false;
            var levelId = BeatmapLevel.Level.levelID;
            return levelId.Replace("custom_level_", "") == value.ReplayInfo.SongHash;
        }

        #endregion

        #region Setup

        public event Action<BeatmapLevelWithKey>? BeatmapSelectedEvent;

        private FlowCoordinator? _flowCoordinator;
        private LevelSelectionFlowCoordinator? _selectionFlowCoordinator;

        public void Setup(FlowCoordinator flowCoordinator, LevelSelectionFlowCoordinator selectionFlowCoordinator) {
            _flowCoordinator = flowCoordinator;
            _selectionFlowCoordinator = selectionFlowCoordinator;
        }

        private void Present() {
            if (_flowCoordinator == null || _selectionFlowCoordinator == null) {
                throw new UninitializedComponentException();
            }
            _flowCoordinator.PresentFlowCoordinator(_selectionFlowCoordinator);
            _selectionFlowCoordinator.AllowDifficultySelection = false;
            _selectionFlowCoordinator.FlowCoordinatorDismissedEvent += HandleFlowCoordinatorDismissed;
            _selectionFlowCoordinator.BeatmapSelectedEvent += HandleBeatmapSelected;
        }

        #endregion

        #region Construct

        private BeatmapPreviewPanel _beatmapPreviewPanel = null!;

        private void SetBeatmapLevel(BeatmapLevel level) {
            _beatmapPreviewPanel.SetBeatmapLevel(level).ConfigureAwait(true);
        }

        protected override GameObject Construct() {
            return new Dummy {
                Children = {
                    new ImageButton {
                        Image = {
                            Color = Color.white,
                            Sprite = BundleLoader.Sprites.background,
                            PixelsPerUnit = 12f,
                            Material = GameResources.UINoGlowMaterial
                        },
                        Colors = null,
                        OnClick = Present,
                        GradientColors1 = new SimpleColorSet {
                            Color = Color.clear,
                            HoveredColor = Color.white.ColorWithAlpha(0.2f)
                        },
                        Skew = UIStyle.Skew,
                        Children = {
                            new BeatmapPreviewPanel {
                                    Skew = UIStyle.Skew
                                }
                                .AsFlexItem(grow: 1f, margin: new() { right = 1f })
                                .Bind(ref _beatmapPreviewPanel)
                        }
                    }.AsFlexGroup().AsFlexItem(
                        grow: 1f,
                        margin: new() { left = 1f, right = 1f }
                    )
                }
            }.AsFlexGroup(padding: 1f).Use();
        }

        protected override void OnInitialize() {
            SetBeatmapLevel(previewBeatmapLevel);
            this.AsFlexItem(size: new() { x = 52f, y = 12f });
        }

        #endregion

        #region Callbacks

        private void HandleFlowCoordinatorDismissed() {
            _selectionFlowCoordinator!.FlowCoordinatorDismissedEvent -= HandleFlowCoordinatorDismissed;
            _selectionFlowCoordinator.BeatmapSelectedEvent -= HandleBeatmapSelected;
        }

        private void HandleBeatmapSelected(BeatmapLevelWithKey beatmap) {
            BeatmapLevel = beatmap;
            SetBeatmapLevel(beatmap.Level);
            BeatmapSelectedEvent?.Invoke(BeatmapLevel);
            FilterUpdatedEvent?.Invoke();
        }

        #endregion
    }
}