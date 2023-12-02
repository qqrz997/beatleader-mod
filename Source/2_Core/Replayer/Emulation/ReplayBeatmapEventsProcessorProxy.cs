﻿using System;
using System.Collections.Generic;
using BeatLeader.Models;
using BeatLeader.Models.AbstractReplay;
using Zenject;

namespace BeatLeader.Replayer.Emulation {
    /// <summary>
    /// The ReplayBeatmapEventsProcessor proxy class. Useful for things that don't need to take additional info from players.
    /// </summary>
    internal class ReplayBeatmapEventsProcessorProxy : IDisposable, IReplayBeatmapEventsProcessor {
        #region Injection

        [Inject] private readonly IVirtualPlayersManager _playersManager = null!;

        #endregion

        #region Proxy Impl

        public bool CurrentEventHasTimeMismatch {
            get {
                Initialize();
                return _beatmapEventsProcessor.CurrentEventHasTimeMismatch;
            }
        }

        public bool QueueIsBeingAdjusted {
            get {
                Initialize();
                return _beatmapEventsProcessor.QueueIsBeingAdjusted;
            }
        }

        public event Action<LinkedListNode<NoteEvent>>? NoteEventDequeuedEvent {
            add {
                Initialize();
                _beatmapEventsProcessor.NoteEventDequeuedEvent += value;
            }
            remove => _beatmapEventsProcessor.NoteEventDequeuedEvent -= value;
        }

        public event Action<LinkedListNode<WallEvent>>? WallEventDequeuedEvent {
            add {
                Initialize();
                _beatmapEventsProcessor.WallEventDequeuedEvent += value;
            }
            remove => _beatmapEventsProcessor.WallEventDequeuedEvent -= value;
        }

        public event Action? EventQueueAdjustStartedEvent {
            add {
                Initialize();
                _beatmapEventsProcessor.EventQueueAdjustStartedEvent += value;
            }
            remove => _beatmapEventsProcessor.EventQueueAdjustStartedEvent -= value;
        }

        #endregion

        #region Setup

        private IReplayBeatmapEventsProcessor _beatmapEventsProcessor = null!;
        private bool _isInitialized;
        
        //Zenject calls Initialize too lately, so we forced to do such shenanigans if we want to avoid Behaviour usage
        public void Initialize() {
            if (_isInitialized) return;
            _playersManager.PriorityPlayerWasChangedEvent += HandlePriorityPlayerChanged;
            HandlePriorityPlayerChanged(_playersManager.PriorityPlayer);
            _isInitialized = true;
        }

        public void Dispose() {
            _playersManager.PriorityPlayerWasChangedEvent -= HandlePriorityPlayerChanged;
        }

        #endregion

        #region Callbacks

        private void HandlePriorityPlayerChanged(IVirtualPlayer player) {
            _beatmapEventsProcessor = player.ReplayBeatmapEventsProcessor;
        }
        
        #endregion
    }
}