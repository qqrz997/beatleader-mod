﻿using System.Collections.Generic;
using BeatLeader.API.Methods;
using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace BeatLeader.UI.MainMenu {
    internal class MapNewsPanel : AbstractNewsPanel {
        #region UI Components

        [UIComponent("empty-text"), UsedImplicitly] private TextMeshProUGUI _emptyText = null!;

        [UIObject("loading-indicator"), UsedImplicitly] private GameObject _loadingIndicator = null!;

        protected override void OnInitialize() {
            base.OnInitialize();
            header.Setup("Trending Maps");
            TrendingMapsRequest.SendRequest();
            TrendingMapsRequest.AddStateListener(OnRequestStateChanged);
        }

        protected override void OnDispose() {
            TrendingMapsRequest.RemoveStateListener(OnRequestStateChanged);
        }

        #endregion

        #region Request

        private void OnRequestStateChanged(API.RequestState state, Paged<MapData> result, string failReason) {
            switch (state) {
                case API.RequestState.Uninitialized:
                case API.RequestState.Started:
                default: {
                    _loadingIndicator.SetActive(true);
                    _emptyText.gameObject.SetActive(false);
                    DisposeList();
                    break;
                }
                case API.RequestState.Failed:
                    _loadingIndicator.SetActive(false);
                    _emptyText.gameObject.SetActive(true);
                    _emptyText.text = "<color=#ff8888>Failed to load";
                    DisposeList();
                    break;
                case API.RequestState.Finished: {
                    _loadingIndicator.SetActive(false);

                    if (result.data is { Count: > 0 }) {
                        _emptyText.gameObject.SetActive(false);
                        PresentList(result.data);
                    } else {
                        _emptyText.gameObject.SetActive(true);
                        _emptyText.text = "There is no trending maps";
                        DisposeList();
                    }

                    break;
                }
            }
        }

        #endregion

        #region List

        private readonly List<FeaturedPreviewPanel> _list = new List<FeaturedPreviewPanel>();

        private void PresentList(IEnumerable<MapData> items) {
            DisposeList();

            foreach (var item in items) {
                var component = Instantiate<FeaturedPreviewPanel>(transform);
                component.ManualInit(mainContainer);
                SetupFeaturePreview(component, item);
                _list.Add(component);
            }

            MarkScrollbarDirty();
        }

        private void SetupFeaturePreview(FeaturedPreviewPanel panel, MapData item) {
            panel.Setup(item.song.coverImage, item.song.name, item.song.mapper, "Play", ButtonAction);
            return;

            void ButtonAction() {
                MapDownloadDialog.OpenSongOrDownloadDialog(item.song, Content.transform);
            }
        }


        private void DisposeList() {
            foreach (var post in _list) {
                Destroy(post.gameObject);
            }

            _list.Clear();
            MarkScrollbarDirty();
        }

        #endregion
    }
}