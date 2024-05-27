﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace BeatLeader.UI.Reactive {
    internal interface ILayoutController : IContextMember {
        event Action? LayoutControllerUpdatedEvent;
        
        void ReloadChildren(IEnumerable<ILayoutItem> children);
        void ReloadDimensions(Rect controllerRect);
        void Recalculate(bool root);
        void Apply();
    }
}