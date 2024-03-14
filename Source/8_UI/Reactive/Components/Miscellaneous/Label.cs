﻿using TMPro;
using UnityEngine;

namespace BeatLeader.UI.Reactive.Components {
    internal class Label : ReactiveComponent {
        public string Text {
            get => _text.text;
            set => _text.text = value;
        }

        public bool RichText {
            get => _text.richText;
            set => _text.richText = value;
        }

        public float FontSize {
            get => _text.fontSize;
            set => _text.fontSize = value;
        }

        public bool WrapText {
            get => _text.enableWordWrapping;
            set => _text.enableWordWrapping = value;
        }

        public TextOverflowModes Overflow {
            get => _text.overflowMode;
            set => _text.overflowMode = value;
        }

        public TextAlignmentOptions Alignment {
            get => _text.alignment;
            set => _text.alignment = value;
        }

        public Color Color {
            get => _text.color;
            set => _text.color = value;
        }

        private TMP_Text _text = null!;

        protected override void Construct(RectTransform rect) {
            _text = rect.gameObject.AddComponent<TextMeshProUGUI>();
        }

        protected override void OnInitialize() {
            Alignment = TextAlignmentOptions.Center;
            Overflow = TextOverflowModes.Ellipsis;
            WrapText = false;
        }
    }
}