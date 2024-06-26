using System;
using BeatLeader.Models;
using BeatLeader.UI.Reactive;
using BeatLeader.UI.Reactive.Components;
using BeatLeader.UI.Reactive.Yoga;
using BeatLeader.Utils;
using UnityEngine;

namespace BeatLeader.UI.Hub {
    internal class TagEditorPanel : ReactiveComponent {
        #region Tag

        public Func<string, ReplayTagValidationResult>? ValidationContract { get; set; }

        public string TagName { get; private set; } = string.Empty;
        public Color TagColor { get; private set; }
        public bool IsValid => _validationResult?.Ok ?? false;

        public void Clear() {
            _colorPicker.Color = ColorUtils.RandomColor();
            _inputField.ClearText();
        }

        #endregion

        #region Construct

        private ReplayTagValidationResult? _validationResult;
        private InputField _inputField = null!;
        private ColorPicker _colorPicker = null!;

        protected override GameObject Construct() {
            return new Dummy {
                Children = {
                    new NamedRail {
                        Label = {
                            Text = "Name"
                        },
                        Component = new InputField {
                            Icon = GameResources.Sprites.EditIcon,
                            TextApplicationContract = x => {
                                _validationResult = ValidationContract?.Invoke(x);
                                NotifyPropertyChanged(nameof(IsValid));
                                return _validationResult?.Ok ?? false;
                            },
                            Keyboard = new KeyboardModal<Keyboard, InputField> {
                                ClickOffCloses = false
                            }
                        }.WithListener(
                            x => x.Text,
                            x => {
                                TagName = x;
                                if (x.Length == 0) _validationResult = null;
                                NotifyPropertyChanged(nameof(TagName));                               
                                NotifyPropertyChanged(nameof(IsValid));
                            }
                        ).AsFlexItem(minSize: new() { x = 25f, y = 8f }).Bind(ref _inputField),
                    }.AsFlexItem(),
                    //
                    new NamedRail {
                        Label = {
                            Text = "Color"
                        },
                        Component = new ColorPicker {
                            Color = ColorUtils.RandomColor()
                        }.WithListener(
                            x => x.Color,
                            x => {
                                TagColor = x;
                                NotifyPropertyChanged(nameof(TagColor));
                            }
                        ).AsFlexItem(size: "auto").Bind(ref _colorPicker)
                    }.AsFlexItem()
                }
            }.AsFlexGroup(
                direction: FlexDirection.Column,
                gap: 1f
            ).Use();
        }

        #endregion
    }
}