﻿using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using BeatLeader.API;

namespace BeatLeader.Models {
    public class GenericReplayHeader : IReplayHeader {
        public GenericReplayHeader(
            IReplayFileManager replayManager,
            string filePath,
            IReplayInfo? replayInfo
        ) {
            _replayManager = replayManager;
            FilePath = filePath;
            ReplayInfo = replayInfo;
            _status = replayInfo is null ? FileStatus.Corrupted : FileStatus.Unloaded;
        }

        public GenericReplayHeader(
            IReplayFileManager replayManager,
            string filePath,
            Replay.Replay replay
        ) : this(replayManager, filePath, replay.info) {
            _cachedReplay = replay;
            _status = FileStatus.Loaded;
        }

        public FileStatus FileStatus {
            get => _status;
            private set {
                _status = value;
                StatusChangedEvent?.Invoke(value);
            }
        }

        public string FilePath { get; }
        public IReplayInfo? ReplayInfo { get; private set; }

        public event Action<FileStatus>? StatusChangedEvent;

        private readonly IReplayFileManager _replayManager;
        private FileStatus _status;
        private Replay.Replay? _cachedReplay;
        private IPlayer? _cachedPlayer;

        public async Task<Replay.Replay?> LoadReplayAsync(CancellationToken token) {
            if (_cachedReplay is not null) return _cachedReplay;
            FileStatus = FileStatus.Loading;
            _cachedReplay = await _replayManager.LoadReplayAsync(this, token);
            FileStatus = _cachedReplay is null ? FileStatus.Corrupted : FileStatus.Loaded;
            return _cachedReplay;
        }

        public async Task<IPlayer> LoadPlayerAsync(bool bypassCache, CancellationToken token) {
            if (_cachedPlayer is not null && !bypassCache) return _cachedPlayer;
            var request = PlayerRequest.SendRequest(ReplayInfo!.PlayerID);
            await request.Join();
            if (request.RequestStatusCode is not HttpStatusCode.OK) {
                Plugin.Log.Error($"Failed to load player(id: {ReplayInfo.PlayerID}) from the server!");
            } else {
                _cachedPlayer = request.Result;
            }
            return _cachedPlayer ?? Player.GuestPlayer;
        }

        public bool DeleteReplay() {
            if (FileStatus is FileStatus.Deleted || !_replayManager.DeleteReplay(this)) return false;
            ReplayInfo = null;
            FileStatus = FileStatus.Deleted;
            return true;
        }
    }
}