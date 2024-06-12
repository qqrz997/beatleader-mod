using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BeatLeader.Components {
    internal class PrestigeExperienceBar : ReeUIComponentV2 {
        #region Setup

        protected override void OnInitialize() {
            SetupBar();
            SetPlayer(null);
        }

        protected override void OnDispose() {
            DisposeBar();
        }

        #endregion

        #region SetInfo

        private IPlayer? _player;

        public void SetPlayer(IPlayer? player) {
            _player = player;
            RefreshBar();
            RefreshTexts();
        }

        #endregion

        #region Level Text

        [UIComponent("prev-level-text"), UsedImplicitly]
        private TMP_Text _prevLevelText = null!;

        [UIComponent("next-level-text"), UsedImplicitly]
        private TMP_Text _nextLevelText = null!;

        private void RefreshTexts() {
            var level = _player?.level ?? 0;
            _prevLevelText.text = FormatUtils.FormatLevel(level);
            _nextLevelText.text = FormatUtils.FormatLevel(level + 1);
        }

        #endregion

        #region Bar

        [UIComponent("bar"), UsedImplicitly]
        private Image _bar = null!;

        private Material _barMaterial = null!;
        private SmoothHoverController _barHoverController = null!;

        private static readonly int progressProperty = Shader.PropertyToID("_Progress");
        private static readonly int focusProperty = Shader.PropertyToID("_Focus");

        private void RefreshBar() {
            float percentage;
            if (_player != null) {
                var reqExp = 1000 * (_player.level + 1);
                percentage = (float)_player.experience / reqExp;
            } else {
                percentage = 0.5f;
            }
            _barMaterial.SetFloat(progressProperty, percentage);
        }

        private void SetupBar() {
            _barMaterial = Instantiate(BundleLoader.ExperienceBarMaterial);
            _bar.sprite = BundleLoader.WhiteBG;
            _bar.material = _barMaterial;
            var go = _bar.gameObject;
            //
            var canvasGroup = go.AddComponent<CanvasGroup>();
            canvasGroup.ignoreParentGroups = true;
            //
            _barHoverController = go.AddComponent<SmoothHoverController>();
            _barHoverController.AddStateListener(HandleBarHoverProgressChanged);
        }

        private void DisposeBar() {
            Destroy(_barMaterial);
        }

        private void HandleBarHoverProgressChanged(bool hovered, float progress) {
            _barMaterial.SetFloat(focusProperty, progress);
        }

        #endregion
    }
}