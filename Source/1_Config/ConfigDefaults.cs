﻿using BeatLeader.Models;
using UnityEngine;

namespace BeatLeader {
    internal static class ConfigDefaults {
        #region Enabled

        public const bool Enabled = true;

        #endregion

        #region MenuButtonEnabled

        public const bool MenuButtonEnabled = true;

        #endregion

        #region BeatLeaderServer

        public const BeatLeaderServer MainServer = BeatLeaderServer.XYZ_DOMAIN;

        #endregion

        #region ScoresContext

        public static readonly ScoresContext ScoresContext = ScoresContext.Modifiers;

        #endregion

        #region LeaderboardTableMask

        public const ScoreRowCellType LeaderboardTableMask = ScoreRowCellType.Rank |
            ScoreRowCellType.Username |
            ScoreRowCellType.Modifiers |
            ScoreRowCellType.Accuracy |
            ScoreRowCellType.PerformancePoints |
            ScoreRowCellType.Score;

        #endregion

        #region LeaderboardDisplaySettings

        public static LeaderboardDisplaySettings LeaderboardDisplaySettings = new() {
            ClanCaptureDisplay = true
        };

        #endregion

        #region ReplayerSettings

        public static readonly ReplayerSettings ReplayerSettings = new() {
            AutoHideUI = false,
            LoadPlayerEnvironment = false,
            ExitReplayAutomatically = true,

            ShowHead = false,
            ShowLeftSaber = true,
            ShowRightSaber = true,
            ShowWatermark = true,

            ShowTimelineMisses = true,
            ShowTimelineBombs = true,
            ShowTimelinePauses = true,

            CameraSettings = new InternalReplayerCameraSettings() {
                MaxCameraFOV = 110,
                MinCameraFOV = 70,
                CameraFOV = 90,
                FpfcCameraView = "PlayerView",
                VRCameraView = "BehindView"
            },

            UISettings = new() {
                FloatingSettings = new() {
                    Pose = new() {
                        position = new(0f, 1f, 2f),
                        rotation = Quaternion.Euler(30f, 0f, 0f)
                    },
                    InitialPose = new() {
                        position = new(0f, 1f, 2f),
                        rotation = Quaternion.Euler(30f, 0f, 0f)
                    },
                    Pinned = true,
                    SnapEnabled = true,
                    CurvatureRadius = 90f,
                    CurvatureEnabled = true
                },
                //TODO: add other components
                LayoutEditorSettings = new() { }
            },

            Shortcuts = new() {
                LayoutEditorPartialModeHotkey = KeyCode.H,
                HideCursorHotkey = KeyCode.C,
                PauseHotkey = KeyCode.Space,
                RewindForwardHotkey = KeyCode.RightArrow,
                RewindBackwardHotkey = KeyCode.LeftArrow,
            }
        };

        #endregion

        #region ReplaySavingSettings

        public const bool EnableReplayCaching = false;

        public const bool OverrideOldReplays = true;

        public const bool SaveLocalReplays = true;

        public const ReplaySaveOption ReplaySavingOptions = ReplaySaveOption.Exit | ReplaySaveOption.Fail | ReplaySaveOption.ZeroScore;

        #endregion

        #region Language

        public const BLLanguage SelectedLanguage = BLLanguage.GameDefault;

        #endregion
    }
}