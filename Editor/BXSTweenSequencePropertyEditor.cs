#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace BX.Tweening.Editor
{
    [CustomPropertyDrawer(typeof(BXSTweenSequence))]
    public class BXSTweenSequencePropertyEditor : BXSTweenablePropertyEditor
    {
        /// <summary>
        /// Name list of fields to be omitted.
        /// </summary>
        private static readonly string[] FieldOmitNameList =
        {
            // TODO
            $"m_{nameof(BXSTweenable.Duration)}",
            // ---
            $"m_{nameof(BXSTweenable.TickType)}",
            $"m_{nameof(BXSTweenable.UseEaseCurve)}",
            $"m_{nameof(BXSTweenable.Ease)}",
            $"m_{nameof(BXSTweenable.EaseCurve)}",
            $"m_{nameof(BXSTweenable.Clamp01EasingSetter)}",
            $"m_{nameof(BXSTweenable.Speed)}",
            $"m_{nameof(BXSTweenable.LoopType)}",
        };

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight + ctx.YMargin;

            if (!property.isExpanded)
            {
                return height;
            }

            GatherChildPropertyState(property);

            // property.GetVisibleChildren(), which seemingly does nothing much except for linq compat.. must test it on multiple unity versions.
            foreach (SerializedProperty child in GetVisibleChildren(property))
            {
                if (FieldOmitNameList.Any(name => child.name == name))
                {
                    continue;
                }

                //if (child.name == $"m_{nameof(BXSTweenable.Duration)}")
                //{
                //    height += EditorGUIUtility.singleLineHeight + ctx.YMargin;
                //
                //    continue;
                //}

                height += GetChildPropertyHeight(child);
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ctx.Reset();
            label = EditorGUI.BeginProperty(position, label, property);

            property.isExpanded = EditorGUI.Foldout(ctx.GetPropertyRect(position, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

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

            // property.GetVisibleChildren()
            foreach (SerializedProperty child in GetVisibleChildren(property))
            {
                if (FieldOmitNameList.Contains(child.name))
                {
                    continue;
                }

                //if (visibleProp.name == $"m_{nameof(BXSTweenable.Duration)}")
                //{
                //    using (EditorGUI.DisabledScope disabled = new EditorGUI.DisabledScope(true))
                //    {
                //        // Draw a read-only property
                //        // Until we can access the "value type-like" value of the SerializedProperty this has to be disabled.
                //        // While SerializedPropertyUtility.GetTarget(this SerializedProperty) isn't too complex,
                //        // i wouldn't like to add it (for time being) as it's somewhat long (and BX_BASE_UTIL is not done yet)
                //        // > This iterates the FieldInfo through the "SerializedProperty.propertyPath" until we reach the object reference value.
                //        // > It only has problem when a value that is root FieldInfo in a "struct" type is given which makes the C# object read-only.
                //        // EditorGUI.FloatField(mainCtx.GetPropertyRect(indentedPosition, EditorGUIUtility.singleLineHeight), "Total Duration", ((BXSTweenable)property.GetTarget().value).Duration);
                //    }
                //
                //    continue;
                //}

                DrawChildPropertyGUI(indentedPosition, property, child);
            }
            EditorGUI.indentLevel--;

            EditorGUI.EndProperty();
        }
    }
}
#endif
