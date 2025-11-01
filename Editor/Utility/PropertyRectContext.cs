#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace BX.Tweening.Editor
{
    /// <summary>
    /// Vertical <see cref="UnityEngine.Rect"/> context that can be implicitly <see cref="UnityEngine.Rect"/>
    /// <br>Used for convenience and horizontal tiling.</br>
    /// </summary>
    internal struct HorizontalRectContext : IEquatable<Rect>
    {
        private Rect m_Rect;
        public readonly Rect Rect => m_Rect;

        /// <inheritdoc cref="GetRemaining(int)"/>
        public readonly float Remaining => GetRemaining();
        /// <summary>
        /// Remaining size of the base <see cref="Rect"/>.
        /// <br>Compare this to be equal to zero or less than zero to check if this context is complete.</br>
        /// </summary>
        public readonly float GetRemaining(int marginCount = 1)
        {
            return m_Rect.width - (xCurrent + (marginCount * xMargin));
        }

        public float xCurrent;
        public float xMargin;
        public void Reset()
        {
            xCurrent = 0f;
        }
        public Rect GetRect(float width)
        {
            var baseRect = new Rect(m_Rect.x, m_Rect.y, width, m_Rect.height);
            baseRect.x += xCurrent + (xMargin / 2f);
            xCurrent += width + xMargin;

            return baseRect;
        }
        public Rect PeekRect(float width)
        {
            var baseRect = new Rect(m_Rect.x, m_Rect.y, width, m_Rect.height);
            baseRect.x += xCurrent + (xMargin / 2f);

            return baseRect;
        }

        public HorizontalRectContext(Rect rect, float margin)
        {
            m_Rect = rect;
            xCurrent = 0f;
            xMargin = margin;
        }

        // This is implicit so that i don't have to change the code
        // + Usually horizontal space isn't used often, vertical space is used more within editors
        public static implicit operator Rect(HorizontalRectContext c)
        {
            return c.m_Rect;
        }
        public static implicit operator HorizontalRectContext(Rect r)
        {
            return new HorizontalRectContext(r, 1f);
        }

        public bool Equals(Rect other)
        {
            return m_Rect.Equals(other);
        }
    }

    /// <summary>
    /// A simpler way to get <see cref="PropertyDrawer"/>'s rects seperated with given height and padding (similar to <see cref="GUILayout"/> without using it)
    /// </summary>
    internal sealed class PropertyRectContext
    {
        /// <summary>
        /// The current Y elapsed for this rect context.
        /// <br>Can be reset to zero using <see cref="Reset"/> or be used for tallying the height (not recommended).</br>
        /// </summary>
        public float CurrentY => m_CurrentY;
        /// <inheritdoc cref="CurrentY"/>
        private float m_CurrentY;

        /// <summary>
        /// Y axis margin. Applies up and down.
        /// </summary>
        public float YMargin { get; set; } = 2f;
        /// <summary>
        /// X axis margin. Applies left and right.
        /// </summary>
        public float XMargin { get; set; } = 1f;

        /// <summary>
        /// Returns the <paramref name="property"/>'s rect.
        /// (by getting the height with <see cref="EditorGUI.GetPropertyHeight(SerializedProperty)"/>)
        /// </summary>
        public HorizontalRectContext GetRect(Rect baseRect, SerializedProperty property)
        {
            return GetRect(baseRect, EditorGUI.GetPropertyHeight(property));
        }
        /// <summary>
        /// Returns the base target rect.
        /// </summary>
        public HorizontalRectContext GetRect(Rect baseRect, float height)
        {
            baseRect.height = height;                  // set to target height
            baseRect.y += m_CurrentY + (YMargin / 2f); // offset by Y
            m_CurrentY += height + YMargin;            // add Y offset

            return new HorizontalRectContext(baseRect, XMargin);
        }

        /// <summary>
        /// Returns the next target rect for <paramref name="property"/> that is going to have it's height pushed.
        /// <br>This DOES NOT move the <see cref="CurrentY"/> in any way, use <see cref="GetRect(Rect, float)"/>.</br>
        /// </summary>
        public HorizontalRectContext PeekRect(Rect baseRect, SerializedProperty property)
        {
            return PeekRect(baseRect, EditorGUI.GetPropertyHeight(property));
        }
        /// <summary>
        /// Returns the next target rect that is going to have it's height pushed.
        /// <br>This DOES NOT move the <see cref="CurrentY"/> in any way, use <see cref="GetRect(Rect, float)"/>.</br>
        /// </summary>
        public HorizontalRectContext PeekRect(Rect baseRect, float height)
        {
            baseRect.height = height;                  // set to target height
            baseRect.y += m_CurrentY + (YMargin / 2f); // offset by Y
            // don't offset Y as this is a peek.

            return new HorizontalRectContext(baseRect, XMargin);
        }

        /// <summary>
        /// Resets the context's current Y positioning.
        /// <br>Can be used when the context is to be used for reserving new rects.</br>
        /// <br>Always call this before starting new contexts to not have the positions shift forever.</br>
        /// </summary>
        public void Reset()
        {
            m_CurrentY = 0f;
        }

        /// <summary>
        /// Creates a PropertyRectContext where the <see cref="YMargin"/> is 2f.
        /// </summary>
        public PropertyRectContext()
        { }
        /// <summary>
        /// Creates a PropertyRectContext where the <see cref="YMargin"/> is the given parameter <paramref name="yMargin"/>.
        /// </summary>
        public PropertyRectContext(float yMargin)
        {
            YMargin = yMargin;
        }
        /// <summary>
        /// Creates a PropertyRectContext where the <see cref="YMargin"/> is the given parameter <paramref name="ymargin"/>.
        /// </summary>
        public PropertyRectContext(float xMargin, float yMargin)
        {
            XMargin = xMargin;
            YMargin = yMargin;
        }

        /// <summary>
        /// Converts the '<see cref="PropertyRectContext"/>' into information string.
        /// <br>May throw exceptions if <see cref="YMargin"/> was overriden and could throw an exception on it's getter.</br>
        /// </summary>
        public override string ToString()
        {
            return $"PropertyRectContext | CurrentY={m_CurrentY}, Margin={YMargin}";
        }
    }
}
#endif
