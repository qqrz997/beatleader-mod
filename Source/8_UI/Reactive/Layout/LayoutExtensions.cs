﻿using BeatLeader.UI.Reactive.Yoga;
using UnityEngine;

namespace BeatLeader.UI.Reactive {
    internal static class LayoutExtensions {
        #region Flex

        public static T AsFlexItem<T>(
            this T component,
            float grow = 0f,
            float shrink = 1,
            YogaValue? basis = null,
            YogaVector? size = null,
            YogaVector? minSize = null,
            YogaVector? maxSize = null,
            YogaFrame? margin = null,
            YogaValue? aspectRatio = null,
            Align alignSelf = Align.Auto
        ) where T : ReactiveComponent {
            if (component.LayoutModifier is not YogaModifier modifier) {
                modifier = new YogaModifier();
                component.LayoutModifier = modifier;
            }
            if (size == null && !ExpandFlexChild(component)) {
                modifier.Size = YogaVector.Undefined;
            }
            modifier.FlexShrink = shrink;
            modifier.FlexGrow = grow;
            modifier.FlexBasis = basis ?? YogaValue.Undefined;
            modifier.MinSize = minSize ?? YogaVector.Undefined;
            modifier.MaxSize = maxSize ?? YogaVector.Undefined;
            modifier.Margin = margin ?? YogaFrame.Zero;
            modifier.AspectRatio = aspectRatio ?? YogaValue.Undefined;
            modifier.AlignSelf = alignSelf;
            return component;
        }

        public static T AsFlexGroup<T>(
            this T component,
            FlexDirection direction = FlexDirection.Row,
            Justify justifyContent = Justify.SpaceAround,
            Align alignItems = Align.Stretch,
            Wrap wrap = Wrap.NoWrap,
            YogaFrame? padding = null,
            bool expandUnspecifiedChildren = true
        ) where T : ReactiveComponent {
            component.SetLayoutController(
                new YogaLayoutController {
                    FlexDirection = direction,
                    JustifyContent = justifyContent,
                    AlignItems = alignItems,
                    FlexWrap = wrap,
                    Padding = padding ?? YogaFrame.Zero
                }
            );
            if (expandUnspecifiedChildren) {
                foreach (var comp in component.Children) {
                    ExpandFlexChild(comp);
                }
            }
            return component;
        }

        private static bool ExpandFlexChild(ReactiveComponent component) {
            if (component.LayoutModifier is not YogaModifier modifier) return false;
            var sizeIsUndefined = modifier.Size == YogaVector.Auto || modifier.Size == YogaVector.Undefined;

            if (component.Parent?.LayoutController is not YogaLayoutController controller || !sizeIsUndefined) return false;
            var row = controller.FlexDirection is FlexDirection.Row or FlexDirection.RowReverse;
            modifier.Size = row ? new(YogaValue.Undefined, "100%") : new("100%", YogaValue.Undefined);

            return true;
        }

        #endregion

        #region Rect

        public static T WithRectSize<T>(
            this T component,
            float height,
            float width
        ) where T : ReactiveComponent {
            return component.AsRectItem(new(width, height));
        }

        public static T AsRectItem<T>(
            this T component,
            Vector2 sizeDelta = default,
            Vector2 anchorMin = default,
            Vector2 anchorMax = default
        ) where T : ReactiveComponent {
            if (component.LayoutModifier is not RectModifier modifier) {
                modifier = new();
            }
            component.LayoutModifier = modifier;
            modifier.AnchorMin = anchorMin;
            modifier.AnchorMax = anchorMax;
            modifier.SizeDelta = sizeDelta;
            return component;
        }

        public static T WithRectExpand<T>(this T component) where T : ReactiveComponent {
            if (component.LayoutModifier is not RectModifier modifier) {
                modifier = new();
            }
            modifier.With(RectModifier.Expand);
            component.LayoutModifier = modifier;
            return component;
        }
        
        public static RectTransform WithRectExpand(this RectTransform component)  {
            component.anchorMin = Vector2.zero;
            component.anchorMax = Vector2.one;
            component.sizeDelta = Vector2.zero;
            return component;
        }

        #endregion

        public static T With<T>(this T modifier, ILayoutModifier apModifier) where T : ILayoutModifier {
            modifier.CopyFrom(apModifier);
            return modifier;
        }
    }
}