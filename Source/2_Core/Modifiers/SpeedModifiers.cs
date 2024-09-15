﻿using ModifiersCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BeatLeader {
    public class BetterFSModifier : ICustomModifier {
        public string Id => "BFS";
        public string Name => "Better Faster Song";
        public string Description => "Increases song speed by 20%, note speed by 10%";
        public Color? Color => UnityEngine.Color.yellow;
        public Color? MultiplierColor => UnityEngine.Color.yellow;
        public Sprite Icon => BundleLoader.AnchorIcon;
        public float Multiplier => 1.2f;
        public IEnumerable<string>? Requires => null;
        public IEnumerable<string>? RequiredBy => null;
        IEnumerable<string>? IModifier.MutuallyExclusives => new string[] { "SS", "FS", "SF" };
    }

    public class BetterSFModifier : ICustomModifier {
        public string Id => "BSF";
        public string Name => "Better Super Fast Song";
        public string Description => "Increases song speed by 50%, note speed by 25%";
        public Color? Color => new Color(204, 92, 0);
        public Color? MultiplierColor => new Color(204, 92, 0);
        public Sprite Icon => BundleLoader.AnchorIcon;
        public float Multiplier => 1.4f;
        public IEnumerable<string>? Requires => null;
        public IEnumerable<string>? RequiredBy => null;
        IEnumerable<string>? IModifier.MutuallyExclusives => new string[] { "SS", "FS", "SF" };
    }
    public static class SpeedModifiers {
        public static BetterFSModifier BFS = new BetterFSModifier();
        public static BetterSFModifier BSF = new BetterSFModifier();

        public static float SongSpeed() {
            if (ModifiersManager.GetModifierState(BFS.Id))
            {
                return 1.2f;
            }
            else if (ModifiersManager.GetModifierState(BSF.Id))
            {
                return 1.5f;
            }
            return 1f;
        }
    }
}
