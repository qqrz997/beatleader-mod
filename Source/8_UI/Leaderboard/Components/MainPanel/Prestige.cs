using BeatLeader.API.Methods;
using BeatLeader.Models;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using UnityEngine.UI;

namespace BeatLeader.Components {
    internal class Prestige : AbstractReeModal<object> {
        #region Components

        private User user;

        #endregion

        #region Init / Dispose

        protected override void OnInitialize() {
            base.OnInitialize();
            InitializePrestigeButtons();
            UserRequest.AddStateListener(OnProfileRequestStateChanged);
            this.offClickCloses = false;
        }

        protected override void OnDispose() {
            UserRequest.RemoveStateListener(OnProfileRequestStateChanged);
        }

        #endregion

        #region Prestige

        private void OnProfileRequestStateChanged(API.RequestState state, User result, string failReason) {
            switch (state) {
                case API.RequestState.Finished:
                    user = result;
                    break;
                default: return;
            }
        }

        private void RequestPrestige() {
            PrestigeRequest.SendRequest(user.player.id);
            UserRequest.SendRequest();
            Close();
        }

        #endregion

        #region PlaylistButtons

        [UIComponent("prestige-yes-button"), UsedImplicitly]
        private Button _PrestigeYesButton;

        [UIComponent("prestige-no-button"), UsedImplicitly]
        private Button _PrestigeNoButton;

        private void InitializePrestigeButtons() {
            _PrestigeYesButton.onClick.AddListener(() => RequestPrestige());
            _PrestigeNoButton.onClick.AddListener(() => Close());
        }

        #endregion
    }
}