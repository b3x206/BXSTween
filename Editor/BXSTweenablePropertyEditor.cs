#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BX.Tweening.Editor
{
    /// <summary>
    /// Draws the properties for a BXSTweenable.
    /// <br>Some properties are affected according to the states of the other variables,
    /// so you should use these methods to draw child properties :</br>
    /// <br/>
    /// <br>
    /// <see cref="PropertyDrawer.GetPropertyHeight(SerializedProperty, GUIContent)"/> :
    /// <see cref="GatherChildPropertyState(SerializedProperty)"/>, then after <see cref="GetChildPropertyHeight(SerializedProperty)"/>
    /// </br>
    /// <br>
    /// <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/> :
    /// <see cref="DrawChildPropertyGUI(Rect, SerializedProperty, SerializedProperty)"/>
    /// </br>
    /// </summary>
    [CustomPropertyDrawer(typeof(BXSTweenable), true)]
    public class BXSTweenablePropertyEditor : PropertyDrawer
    {
        internal readonly PropertyRectContext ctx = new(2);

        /// <summary>
        /// This method allows for copying and iterating a given <see cref="SerializedProperty"/>.
        /// <br>Without this (using the <see cref="System.Collections.IEnumerable.GetEnumerator"/>
        /// of <see cref="SerializedProperty"/>) the entire UI errors out, requiring registry of every single property manually.</br>
        /// <br>While somewhat inconvenient for the editor script, it is more convenient for registering properties that were added later.</br>
        /// </summary>
        protected static IEnumerable<SerializedProperty> GetVisibleChildren(SerializedProperty property)
        {
            using SerializedProperty iterProperty = property.Copy();
            using SerializedProperty nextSibling = property.Copy();
            {
                nextSibling.NextVisible(false);
            }

            // This is quite necessary, the SerializedProperty.GetEnumerator() doesn't function as I expect.
            if (iterProperty.NextVisible(true))
            {
                yield return iterProperty;

                while (iterProperty.NextVisible(false) && !SerializedProperty.EqualContents(iterProperty, nextSibling))
                {
                    yield return iterProperty;
                }
            }
        }

        protected bool stateIsLoopable, stateUseEaseCurve;
        /// <summary>
        /// Gather state to the state values (<see cref="stateIsLoopable"/>, etc.).
        /// <br>
        /// This must be called in <see cref="GetPropertyHeight(SerializedProperty, GUIContent)"/>,
        /// before <see cref="GetChildPropertyHeight(SerializedProperty)"/>.</br>
        /// </summary>
        protected virtual void GatherChildPropertyState(SerializedProperty parent)
        {
            // Check BXSTweenable state
            // Because of "quirk chungus" behaviour of Unity Engine with SerializedProperty this is the way i do it.
            stateIsLoopable = parent.FindPropertyRelative("m_LoopCount").intValue != 0;
            stateUseEaseCurve = parent.FindPropertyRelative("m_UseEaseCurve").boolValue;
        }
        protected float GetChildPropertyHeight(SerializedProperty child)
        {
            // noop situations
            if (!stateIsLoopable && (child.name == "m_LoopType" || child.name == "m_WaitDelayOnLoop"))
            {
                // Don't draw loop related fields if not looping tween
                return 0f;
            }
            if ((!stateUseEaseCurve && child.name == "m_EaseCurve") || (stateUseEaseCurve && child.name == "m_Ease"))
            {
                // Toggle between showing AnimationCurve and EaseType
                return 0f;
            }

            return EditorGUI.GetPropertyHeight(child) + ctx.YMargin;
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + ctx.YMargin;

            if (!property.isExpanded)
            {
                return height;
            }

            GatherChildPropertyState(property);

            foreach (SerializedProperty child in GetVisibleChildren(property))
            {
                height += GetChildPropertyHeight(child);
            }

            return height;
        }

        /// <summary>
        /// Draw the GUI for a given <see cref="BXSTweenable"/> child property.
        /// </summary>
        /// <param name="parentPos">
        /// The parent position (or a custom value) supplied in 
        /// <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/>.
        /// </param>
        /// <param name="parent">The parent property supplied in <see cref="PropertyDrawer.OnGUI(Rect, SerializedProperty, GUIContent)"/></param>
        /// <param name="child">
        /// Child element, this can be <see cref="SerializedProperty.FindPropertyRelative(string)"/> or one 
        /// of the iterated elements of <see cref="GetVisibleChildren(SerializedProperty)"/>
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the property was drawn, <see langword="false"/> if not.
        /// Note that properties outside of <see cref="BXSTweenable"/> are drawn with default behaviour,
        /// so those always return <see langword="true"/>.
        /// </returns>
        protected bool DrawChildPropertyGUI(Rect parentPos, SerializedProperty parent, SerializedProperty child)
        {
            // noop situations
            if (!stateIsLoopable && (child.name == "m_LoopType" || child.name == "m_WaitDelayOnLoop"))
            {
                // Don't draw loop related fields if not looping tween
                return false;
            }
            if ((!stateUseEaseCurve && child.name == "m_EaseCurve") || (stateUseEaseCurve && child.name == "m_Ease"))
            {
                // Toggle between showing AnimationCurve and EaseType
                return false;
            }

            // Do different behaviours depending on the field.
            if (child.name == "m_Delay") // [Clamp(0f, float.Max)]
            {
                child.floatValue = EditorGUI.FloatField(
                    ctx.GetRect(parentPos, EditorGUIUtility.singleLineHeight),
                    "Delay",
                    Math.Clamp(child.floatValue, 0f, float.MaxValue)
                );
            }
            else if (child.name == "m_Duration") // [Clamp(0f, float.Max)]
            {
                child.floatValue = EditorGUI.FloatField(
                    ctx.GetRect(parentPos, EditorGUIUtility.singleLineHeight),
                    "Duration",
                    Math.Clamp(child.floatValue, 0f, float.MaxValue)
                );
            }
            else if (child.name == "m_LoopCount") // [Clamp(-1, int.Max)]
            {
                child.intValue = EditorGUI.IntField(
                    ctx.GetRect(parentPos, EditorGUIUtility.singleLineHeight),
                    "Loop Count",
                    Math.Clamp(child.intValue, -1, int.MaxValue)
                );
            }
            else if (child.name == "m_Speed") // [Clamp(0, 65535f)]
            {
                child.floatValue = EditorGUI.FloatField(
                    ctx.GetRect(parentPos, EditorGUIUtility.singleLineHeight),
                    "Speed",
                    Math.Clamp(child.floatValue, 0f, 65535f)
                );
            }
            else
            {
                if (child.name == "m_UseEaseCurve")
                {
                    // Assign value on boolean toggle for curve use
                    bool prevValue = child.boolValue;
                    EditorGUI.PropertyField(ctx.GetRect(parentPos, child), child);
                    if (prevValue != child.boolValue && child.boolValue)
                    {
                        using SerializedProperty curveProperty = parent.FindPropertyRelative("m_EaseCurve");
                        if (curveProperty.animationCurveValue == null || curveProperty.animationCurveValue.keys.Length <= 0)
                        {
                            curveProperty.animationCurveValue = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                        }
                    }
                }
                else
                {
                    EditorGUI.PropertyField(ctx.GetRect(parentPos, child), child);
                }
            }

            // likely drawn
            return true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ctx.Reset();
            label = EditorGUI.BeginProperty(position, label, property);
            property.isExpanded = EditorGUI.Foldout(ctx.GetRect(position, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

            if (!property.isExpanded)
            {
                return;
            }

            EditorGUI.indentLevel++;

            Rect indentedPosition = EditorGUI.IndentedRect(position);
            // EditorGUI.IndentedRect indents too much
            float indentDiffScale = (position.width - indentedPosition.width) / 1.33f;
            indentedPosition.x -= indentDiffScale;
            indentedPosition.width += indentDiffScale;

            EditorGUI.BeginChangeCheck();
            foreach (SerializedProperty child in GetVisibleChildren(property))
            {
                DrawChildPropertyGUI(indentedPosition, property, child);
            }

            if (EditorGUI.EndChangeCheck())
            {
                property.serializedObject.ApplyModifiedProperties();
            }

            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
}
#endif
