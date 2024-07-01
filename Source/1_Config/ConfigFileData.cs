using System.Runtime.CompilerServices;
using BeatLeader.Models;
using Hive.Versioning;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using JetBrains.Annotations;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]

namespace BeatLeader {
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal class ConfigFileData {
        #region Serialization

        private const string ConfigPath = "UserData\\BeatLeader.json";

        public static void Initialize() {
            if (File.Exists(ConfigPath)) {
                var text = File.ReadAllText(ConfigPath);
                try {
                    Instance = JsonConvert.DeserializeObject<ConfigFileData>(text);
                    Plugin.Log.Debug("BeatLeader config initialized");
                    return;
                } catch (Exception ex) {
                    Plugin.Log.Error($"Failed to load config (default will be used):\n{ex}");
                }
            }
            Instance = new();
        }

        public static void Save() {
            try {
                var text = JsonConvert.SerializeObject(
                    Instance, Formatting.Indented, new JsonSerializerSettings {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        Converters = {
                            new StringEnumConverter()
                        }
                    }
                );
                File.WriteAllText(ConfigPath, text);
                Plugin.Log.Debug("BeatLeader config saved");
            } catch (Exception ex) {
                Plugin.Log.Error($"Failed to save configuration:\n{ex}");
            }
        }

        public static ConfigFileData Instance { get; private set; } = null!;

        #endregion

        #region ConfigVersion

        public const string CurrentConfigVersion = "1.0";

        [UsedImplicitly]
        public virtual string ConfigVersion { get; set; } = CurrentConfigVersion;

        #endregion

        #region ModVersion

        [UsedImplicitly]
        public string LastSessionModVersion { get; set; } = Version.Zero.ToString();

        #endregion
        
        #region Enabled

        public bool Enabled = ConfigDefaults.Enabled;

        #endregion
        
        #region MenuButtonEnabled

        public bool MenuButtonEnabled = ConfigDefaults.MenuButtonEnabled;

        #endregion

        #region BeatLeaderServer
        
        public BeatLeaderServer MainServer = ConfigDefaults.MainServer;

        #endregion

        #region ScoresContext
        
        public ScoresContext ScoresContext = ConfigDefaults.ScoresContext;

        #endregion

        #region LeaderboardTableMask
        
        public ScoreRowCellType LeaderboardTableMask = ConfigDefaults.LeaderboardTableMask;

        #endregion

        #region LeaderboardDisplaySettings

        public LeaderboardDisplaySettings LeaderboardDisplaySettings = ConfigDefaults.LeaderboardDisplaySettings;

        #endregion

        #region ReplayerSettings

        public InternalReplayerCameraSettings InternalReplayerCameraSettings { get; set; } = ConfigDefaults.InternalReplayerCameraSettings;

        public ReplayerSettings ReplayerSettings {
            get {
                _replayerSettings.CameraSettings = InternalReplayerCameraSettings;
                return _replayerSettings;
            }
            set => _replayerSettings = value;
        }
        
        private ReplayerSettings _replayerSettings = ConfigDefaults.ReplayerSettings;

        #endregion

        #region ReplaySavingSettings

        public bool EnableReplayCaching = ConfigDefaults.EnableReplayCaching;

        public bool OverrideOldReplays = ConfigDefaults.OverrideOldReplays;

        public bool SaveLocalReplays = ConfigDefaults.SaveLocalReplays;
        
        public ReplaySaveOption ReplaySavingOptions = ConfigDefaults.ReplaySavingOptions;

        #endregion

        #region Language
        
        public BLLanguage SelectedLanguage = ConfigDefaults.SelectedLanguage;

        #endregion

        #region OnReload

        [UsedImplicitly]
        public virtual void OnReload() {
            if (ConfigVersion != CurrentConfigVersion) ConfigVersion = CurrentConfigVersion;
        }

        #endregion
    }
}