﻿using System;
using System.Linq;
using BeatLeader.Models;
using BeatLeader.UI.Hub.Models;
using BeatSaberMarkupLanguage.Attributes;
using HMUI;
using IPA.Utilities;
using JetBrains.Annotations;

namespace BeatLeader.Components {
    internal class SearchFiltersPanel : ReeUIComponentV2 {
        #region ReplayFilter

        private IPreviewBeatmapLevel? BeatmapLevel => _beatmapFiltersPanel.BeatmapLevel;
        private BeatmapCharacteristicSO? BeatmapCharacteristic => _beatmapFiltersPanel.BeatmapCharacteristic;
        private BeatmapDifficulty? BeatmapDifficulty => _beatmapFiltersPanel.BeatmapDifficulty;
        private string? PlayerName { get; set; }
        
        public event Action? FilterUpdatedEvent;

        private void NotifyFilterUpdated() {
            FilterUpdatedEvent?.Invoke();
        }
        
        public bool MatchesFilter(IReplayInfo? info) {
            if (_beatmapFiltersPanel.Enabled) return true;
            
            if (info is null) return false;
            var level = BeatmapLevel?.levelID;
            var characteristic = BeatmapCharacteristic?.serializedName;
            var diff = BeatmapDifficulty;

            var playerMatches = PlayerName is null || info.PlayerName.ToLower().Contains(PlayerName);
            var hashMatches = level is null || level.Replace("custom_level_", "") == info.SongHash;
            var diffMatches = !diff.HasValue || info.SongDifficulty == diff.Value.ToString();
            var characteristicMatches = characteristic is null || info.SongMode == characteristic;
            return playerMatches && hashMatches && diffMatches && characteristicMatches;
        }
        
        #endregion

        #region UI Components

        [UIValue("search-panel"), UsedImplicitly]
        private SearchPanel _searchPanel = null!;

        [UIValue("filters-panel"), UsedImplicitly]
        private FilterPanel _filterPanel = null!;

        [UIValue("filters-menu"), UsedImplicitly]
        private BeatmapFiltersPanel _beatmapFiltersPanel = null!;

        [UIValue("settings-button"), UsedImplicitly]
        private ReplayerSettingsButton _replayerSettingsButton = null!;

        [UIComponent("filters-modal")]
        private readonly ModalView _modal = null!;

        #endregion

        #region Init

        protected override void OnInitialize() {
            _modal.blockerClickedEvent += HandleModalClosed;
            _modal.SetField("_animateParentCanvas", false);
            _searchPanel.Placeholder = "Search Player";
            _beatmapFiltersPanel.Setup(Content.GetComponentInParent<ViewController>());
        }

        protected override void OnInstantiate() {
            _searchPanel = Instantiate<SearchPanel>(transform);
            _filterPanel = Instantiate<FilterPanel>(transform);
            _beatmapFiltersPanel = Instantiate<BeatmapFiltersPanel>(transform);
            _replayerSettingsButton = Instantiate<ReplayerSettingsButton>(transform);
            _searchPanel.TextChangedEvent += HandleSearchPromptChanged;
            //_beatmapFiltersPanel.FilterUpdatedEvent += HandleFilterUpdated;
            _filterPanel.FilterButtonClickedEvent += HandleFilterButtonClicked;
            _filterPanel.ResetFiltersButtonClickEvent += HandleResetFiltersButtonClicked;
        }

        public void Setup(IReplayManager replayManager) {
            _replayerSettingsButton.Setup(replayManager);
        }

        public void NotifyContainerStateChanged() {
            _beatmapFiltersPanel.NotifyContainerStateChanged(false);
        }

        #endregion

        #region Callbacks

        private void HandleModalClosed() {
            _beatmapFiltersPanel.NotifyContainerStateChanged(false);
        }

        private void HandleFilterUpdated() {
            _filterPanel.SetFilters(new[] {
                BeatmapLevel is not null ? "Beatmap Filter" : null,
                BeatmapCharacteristic is not null ? "Characteristic Filter" : null,
                BeatmapDifficulty is not null ? "Difficulty Filter" : null
            }.OfType<string>().ToArray());
            NotifyFilterUpdated();
        }

        private void HandleSearchPromptChanged(string text) {
            PlayerName = text;
            NotifyFilterUpdated();
        }

        private void HandleResetFiltersButtonClicked() {
            _beatmapFiltersPanel.ResetFilters();
        }

        private void HandleFilterButtonClicked() {
            _beatmapFiltersPanel.NotifyContainerStateChanged(true);
            _modal.Show(true);
        }

        #endregion
    }
}