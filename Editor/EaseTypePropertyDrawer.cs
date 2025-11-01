#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
#if BX_SEARCH_DROPDOWN
using BX.Editor;
#endif
using BX.Tweening.Utility;
using UnityEditor.IMGUI.Controls;

namespace BX.Tweening.Editor
{
    /// <summary>
    /// Draws a fancy selector for the <see cref="EaseType"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(EaseType))]
    public class EaseTypePropertyDrawer : PropertyDrawer
    {
        // GUI Plotting
#if BX_SEARCH_DROPDOWN
        /// <summary>
        /// A selection dropdown for <see cref="EaseType"/>.
        /// </summary>
        public class SelectorDropdown : SearchDropdown<UGUISearchDropdownWindow>
        {
            public class Item : SearchDropdownElement
            {
                private const float PlotFieldHeight = 40f;
                private const float PlotFieldWidth = 60f;
                private const float EasePreviewDuration = 0.6f;

                internal PropertyRectContext drawingContext = new(2);

                private DropdownElementState previousGUIState;
                private float currentPreviewElapsed = 0f;
                public readonly EaseType ease;

                public Item(EaseType ease, string label) : base(label)
                {
                    this.ease = ease;
                }
                public Item(EaseType ease, SearchDropdownElementContent content) : base(content)
                {
                    this.ease = ease;
                }
                public Item(EaseType ease, string label, string tooltip) : base(label, tooltip)
                {
                    this.ease = ease;
                }
                public Item(EaseType ease, string label, int childrenCapacity) : base(label, childrenCapacity)
                {
                    this.ease = ease;
                }
                public Item(EaseType ease, SearchDropdownElementContent content, int childrenCapacity) : base(content, childrenCapacity)
                {
                    this.ease = ease;
                }
                public Item(EaseType ease, string label, string tooltip, int childrenCapacity) : base(label, tooltip, childrenCapacity)
                {
                    this.ease = ease;
                }

                public override float GetHeight(float viewWidth)
                {
                    return Mathf.Max(base.GetHeight(viewWidth), PlotFieldHeight);
                }

                /// <summary>
                /// Draws the tween name on the left, a graph preview on the right and when hovered, the background eases in with the same effect.
                /// </summary>
                /// <inheritdoc cref="SearchDropdownElement.OnGUI(Rect, ElementGUIDrawingState)"/>
                public override void OnGUI(Rect position, DropdownElementState drawingState)
                {
                    // Draw base labels
                    drawingContext.Reset();
                    Rect contextRect = drawingContext.GetPropertyRect(position, EditorGUIUtility.singleLineHeight);

                    Rect iconRect = new Rect(contextRect)
                    {
                        width = EditorGUIUtility.singleLineHeight
                    };
                    Rect textRect = new Rect(contextRect)
                    {
                        x = contextRect.x + EditorGUIUtility.singleLineHeight + 5f,
                        width = contextRect.width - (iconRect.width + EditorGUIUtility.singleLineHeight + PlotFieldWidth)
                    };
                    Rect plottingAreaRect = new Rect(contextRect)
                    {
                        x = textRect.x + textRect.width,
                        height = PlotFieldHeight,
                        width = PlotFieldWidth,
                    };

                    // Elements | Background
                    // Background box tint
                    // Initial background rect
                    EditorGUI.DrawRect(position, new Color(0.2f, 0.2f, 0.2f));

                    Color stateColor = new Color(0.15f, 0.15f, 0.15f);
                    // Update 'currentPreviewElapsed' only in Repaint
                    bool isRepaintEvent = Event.current.type == EventType.Repaint;
                    switch (drawingState)
                    {
                        case DropdownElementState.Selected:
                            stateColor = new Color(0.15f, 0.35f, 0.39f);
                            if (isRepaintEvent)
                            {
                                currentPreviewElapsed = 1f;
                            }
                            break;

                        case DropdownElementState.Hover:
                            RequestsRepaint = !Mathf.Approximately(currentPreviewElapsed, 1f);
                            // Reset the hover rect to be zero sized
                            if (isRepaintEvent)
                            {
                                if (previousGUIState == DropdownElementState.Selected)
                                {
                                    currentPreviewElapsed = 0f;
                                }

                                if (RequestsRepaint)
                                {
                                    currentPreviewElapsed = Mathf.Clamp01(currentPreviewElapsed + (EditorTime.DeltaTime / EasePreviewDuration));
                                }
                            }
                            break;

                        default:
                            RequestsRepaint = !Mathf.Approximately(currentPreviewElapsed, 0f);
                            if (isRepaintEvent && RequestsRepaint)
                            {
                                currentPreviewElapsed = Mathf.Clamp01(currentPreviewElapsed - (EditorTime.DeltaTime / EasePreviewDuration));
                            }
                            break;
                    }

                    // Draw the partial animated rect 
                    EditorGUI.DrawRect(new Rect(position)
                    {
                        width = position.width * Mathf.Clamp01(BXTweenEase.EasedValue(currentPreviewElapsed, ease))
                    }, stateColor);

                    if (content.Image != null)
                    {
                        // Icon
                        GUI.DrawTexture(iconRect, content.Image, ScaleMode.ScaleToFit);
                    }
                    // Label
                    GUI.Label(textRect, content.ToTempGUIContent(), UGUISearchDropdownWindow.StyleList.LabelStyle);
                    // Plotting
                    Color gColor = GUI.color;
                    GUI.color = Color.green;
                    BXSTweenGUI.Plot(plottingAreaRect, t => BXTweenEase.EasedValue(t, ease), true, false, 0f, 1f, 1.5f, 28);
                    GUI.color = gColor;

                    if (isRepaintEvent)
                    {
                        previousGUIState = drawingState;
                    }
                }
            }

            public readonly EaseType selectedEase = EaseType.Linear;
            public override bool SearchIgnoreCase => true;
            public override bool StartFromFirstSelected => true;

            protected override SearchDropdownElement BuildRoot()
            {
                SearchDropdownElement rootElement = new SearchDropdownElement("Ease List");

                foreach (EaseType ease in Enum.GetValues(typeof(EaseType)).Cast<EaseType>())
                {
                    Item easeItem = new Item(ease, ease.ToString())
                    {
                        Selected = selectedEase == ease
                    };

                    rootElement.Add(easeItem);
                }

                return rootElement;
            }

            public SelectorDropdown(EaseType selected)
            {
                selectedEase = selected;
            }
        }
#else
        // Instead of using BXSearchDropdown, do:
        // 1. Use UnityEditor.Controls.IMGUI.AdvancedDropdown for searchable dropdown
        // 2. Show the ease plot under the GUI

        // - While BXSearchDropdown is not an hard dependency, it is recommended to display all ease previews at once.
        public class SelectorDropdown : AdvancedDropdown
        {
            public readonly EaseType selectedEase;
            public event Action<AdvancedDropdownItem> OnElementSelected;
            public event Action OnDiscardEvent;

            public class Item : AdvancedDropdownItem
            {
                public EaseType ease;

                public Item(EaseType ease, string name) : base(name)
                {
                    this.ease = ease;
                }
            }

            public SelectorDropdown(EaseType selected, AdvancedDropdownState state) : base(state)
            {
                selectedEase = selected;
            }

            protected override AdvancedDropdownItem BuildRoot()
            {
                AdvancedDropdownItem rootElement = new AdvancedDropdownItem("Ease List");

                foreach (EaseType ease in Enum.GetValues(typeof(EaseType)).Cast<EaseType>())
                {
                    Item easeItem = new Item(ease, ease.ToString())
                    {
                        enabled = selectedEase == ease
                    };

                    rootElement.AddChild(easeItem);
                }

                return rootElement;
            }

            protected override void ItemSelected(AdvancedDropdownItem item)
            {
                if (item == null)
                {
                    OnDiscardEvent?.Invoke();
                    return;
                }

                OnElementSelected?.Invoke(item);
            }
        }
#endif

        private readonly PropertyRectContext mainCtx = new PropertyRectContext();
#if BX_SEARCH_DROPDOWN
        public virtual bool EmbedEasePlot { get; set; } = false;
#else
        public virtual bool EmbedEasePlot { get; set; } = true;
#endif

        private static readonly Vector2 EasePlotViewSize = new Vector2(80f, 60f);
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + mainCtx.YMargin;

            if (EmbedEasePlot)
            {
                height += EasePlotViewSize.y + mainCtx.YMargin;
            }

            return height;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            mainCtx.Reset();

            label = EditorGUI.BeginProperty(position, label, property);

            Rect paddedPosition = mainCtx.GetRect(position, EditorGUIUtility.singleLineHeight);
            Rect labelPosition = new Rect(paddedPosition)
            {
                width = EditorGUIUtility.labelWidth,
            };
            Rect dropdownSelectorPosition = new Rect(paddedPosition)
            {
                x = paddedPosition.x + labelPosition.width,
                width = Mathf.Max(paddedPosition.width - labelPosition.width, EditorGUIUtility.fieldWidth)
            };

            EaseType selectedValue = (EaseType)property.longValue;
            bool prevShowMixed = EditorGUI.showMixedValue;

            EditorGUI.LabelField(labelPosition, label);
            EditorGUI.showMixedValue = property.hasMultipleDifferentValues;
            if (EditorGUI.DropdownButton(dropdownSelectorPosition, new GUIContent(ObjectNames.NicifyVariableName(selectedValue.ToString()), label.tooltip), FocusType.Keyboard))
            {
#if BX_SEARCH_DROPDOWN
                SelectorDropdown selectorDropdown = new SelectorDropdown(selectedValue);
                selectorDropdown.Show(dropdownSelectorPosition);

                SerializedObject copySo = new SerializedObject(property.serializedObject.targetObjects);
                SerializedProperty copySetProperty = copySo.FindProperty(property.propertyPath);

                selectorDropdown.OnElementSelected += (element) =>
                {
                    if (!(element is SelectorDropdown.Item item))
                    {
                        return;
                    }

                    copySetProperty.longValue = (long)item.ease;

                    copySo.ApplyModifiedProperties();
                    // --
                    copySo.Dispose();
                    copySetProperty.Dispose();
                };
                selectorDropdown.OnDiscardEvent += () =>
                {
                    copySo.Dispose();
                    copySetProperty.Dispose();
                };
#else
                SelectorDropdown selectorDropdown = new SelectorDropdown(selectedValue, new AdvancedDropdownState());
                selectorDropdown.Show(dropdownSelectorPosition);

                SerializedObject copySo = new SerializedObject(property.serializedObject.targetObjects);
                SerializedProperty copySetProperty = copySo.FindProperty(property.propertyPath);

                selectorDropdown.OnElementSelected += (element) =>
                {
                    if (!(element is SelectorDropdown.Item item))
                    {
                        return;
                    }

                    copySetProperty.longValue = (long)item.ease;

                    copySo.ApplyModifiedProperties();
                    // --
                    copySo.Dispose();
                    copySetProperty.Dispose();
                };
                selectorDropdown.OnDiscardEvent += () =>
                {
                    copySo.Dispose();
                    copySetProperty.Dispose();
                };
#endif
            }
            EditorGUI.showMixedValue = prevShowMixed;

            // TODO ?? : If arrow keys are pressed and the previous DropdownButton element is the
            // active element on keyboard navigation, change the ease value up/down. This will help with selecting ease.
            if (EmbedEasePlot)
            {
                Rect plotViewRect = mainCtx.GetRect(position, EasePlotViewSize.y);
                // Move right
                plotViewRect.x += position.width - EasePlotViewSize.x;
                plotViewRect.width = EasePlotViewSize.x;
                Color gColor = GUI.color;
                GUI.color *= Color.green;
                BXSTweenGUI.Plot(plotViewRect, (v) => BXSTweenEase.EasedValue(v, selectedValue));
                GUI.color = gColor;
            }

            EditorGUI.EndProperty();
        }
    }
}
#endif
