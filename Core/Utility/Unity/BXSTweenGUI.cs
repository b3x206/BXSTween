#if UNITY_5_6_OR_NEWER
using System;
using UnityEngine;

namespace BX.Tweening.Utility
{
    /// <summary>
    /// Contains additional GUI methods for the custom editors.
    /// <br>Can be used outside of editor.</br>
    /// <br>Most code here is "stable", that is depending on how you consider how much functionality you want.</br>
    /// </summary>
    public static class BXSTweenGUI
    {
        /// <summary>
        /// Get how much value <paramref name="n"/> has progressed between <paramref name="a"/> and <paramref name="b"/>
        /// represented by a value ranging 0f~1f if <paramref name="n"/> is between.
        /// <br>This version is unclamped to represent more ranges outside.</br>
        /// </summary>
        private static float InverseLerp(float a, float b, float n)
        {
            if (a == b)
            {
                return 0f;
            }

            return (n - a) / (b - a);
        }

        /// <summary>
        /// Draws a ui line and returns the padded position rect.
        /// <br>For angled / rotated lines, use the <see cref="DrawLine(Vector2, Vector2, int)"/> method. (uses GUI position)</br>
        /// </summary>
        /// <param name="parentRect">Parent rect to draw relative to.</param>
        /// <param name="color">Color of the line.</param>
        /// <param name="thickness">Thiccness of the line.</param>
        /// <param name="margin">Padding of the line. (Space left for the line, between properties)</param>
        /// <returns>The new position target rect, offseted.</returns>
        public static Rect Line(Rect parentRect, Color color, int thickness = 2, int margin = 3)
        {
            // Rect that is passed as an parameter.
            Rect drawRect = new Rect(parentRect.position, new Vector2(parentRect.width, thickness));

            drawRect.y += margin / 2;
            drawRect.x -= 2;
            drawRect.width += 6;

            // Rect with proper height.
            Rect returnRect = new Rect(new Vector2(parentRect.position.x, drawRect.position.y + (thickness + margin)), parentRect.size);
            if (Event.current.type == EventType.Repaint)
            {
                var gColor = GUI.color;
                GUI.color *= color;
                GUI.DrawTexture(drawRect, Texture2D.whiteTexture);
                GUI.color = gColor;
            }

            return returnRect;
        }

        /// <summary>
        /// Draws line.
        /// <br>Color defaults to <see cref="Color.white"/>.</br>
        /// </summary>
        public static void LineFromTo(Vector2 start, Vector2 end, float width)
        {
            LineFromTo(start, end, width, Color.white);
        }
        /// <summary>
        /// Draws line with color.
        /// </summary>
        public static void LineFromTo(Vector2 start, Vector2 end, float width, Color col)
        {
            var gc = GUI.color;
            GUI.color = col;
            LineFromTo(start, end, width, Texture2D.whiteTexture);
            GUI.color = gc;
        }
        /// <summary>
        /// Draws line with texture.
        /// <br>The texture is not used for texture stuff, only for color if your line is not thick enough.</br>
        /// </summary>
        public static void LineFromTo(Vector2 start, Vector2 end, float width, Texture2D tex)
        {
            var guiMat = GUI.matrix;

            if (start == end)
            {
                return;
            }

            if (width <= 0)
            {
                return;
            }

            Vector2 d = end - start;
            float a = Mathf.Rad2Deg * Mathf.Atan(d.y / d.x);
            if (d.x < 0)
            {
                a += 180;
            }

            int width2 = (int)Mathf.Ceil(width / 2);

            GUIUtility.RotateAroundPivot(a, start);
            GUI.DrawTexture(new Rect(start.x, start.y - width2, d.magnitude, width), tex);

            GUI.matrix = guiMat;
        }

        /// <summary>
        /// Tiny value for <see cref="Plot(Rect, Func{float, float}, float, float, float, int)"/>.
        /// </summary>
        private const float PlotDrawEpsilon = .01f;
        /// <summary>
        /// Size of drawn label padding.
        /// </summary>
        private const float PlotTextPaddingX = 24f;
        private const float PlotTextPaddingY = 12f;
        private const int PlotTextFontSize = 9;
        /// <summary>
        /// Used with the <see cref="Plot"/> functions.
        /// </summary>
        public static GUIStyle plotSmallerFontStyle;
        /// <summary>
        /// Used with the <see cref="Plot"/> functions.
        /// </summary>
        public static GUIStyle plotSmallerCenteredFontStyle;
        /// <summary>
        /// Plots the <paramref name="plotFunction"/> to the <see cref="GUI"/>.
        /// <br>The plotting is not accurate and does ignore some of the characteristics of certain functions
        /// (i.e <see cref="Mathf.Tan(float)"/>), but it looks good enough for a rough approximation.</br>
        /// <br/>
        /// <br>Note : This calls <see cref="LineFromTo(Vector2, Vector2, float)"/> lots of times instead of doing something optimized.</br>
        /// <br>It is also very aliased. For drawing (only) bezier curves that look good, use the <see cref="UnityEditor.Handles"/> class. (editor only)</br>
        /// </summary>
        /// <param name="position">Rect positioning to draw the line.</param>
        /// <param name="plotFunction">The plot function that returns rational numbers and is linear. (no self intersections, double values in one value or anything)</param>
        /// <param name="plotMinValue">The minimum Y value for the plotting. If this is 0 and equal to <paramref name="plotMaxValue"/> then the graph won't draw.</param>
        /// <param name="plotMaxValue">The maximum Y value for the plotting. If this is 0 and equal to <paramref name="plotMinValue"/> then the graph won't draw.</param>
        /// <param name="vFrom">The first value to feed the plot function while linearly interpolating.</param>
        /// <param name="vTo">The last value to feed the plot function while linearly interpolating.</param>
        /// <param name="segments">Amount of times that the <see cref="LineFromTo(Vector2, Vector2, float)"/> will be called. This should be a value larger than 1</param>
        public static void Plot(Rect position, Func<float, float> plotFunction, bool showFromToLabels, bool showMinMaxLabels, float plotMinValue, float plotMaxValue, float vFrom = 0f, float vTo = 1f, float lineWidth = 2.5f, int segments = 20)
        {
            Event e = Event.current;

            // Only do this plotting if we are actually drawing and not layouting
            // TODO : Determine how to handle the plot hover tooltip on runtime,
            // as it will just require constant repaint regardless of mouse event.
            if (e == null || (e.type != EventType.Repaint && e.type != EventType.MouseMove))
            {
                return;
            }
            // Invalid size (will cause drawing errors / DivideByZero)
            if (position.width <= 0f || position.height <= 0f)
            {
                return;
            }

            Color guiPrevColor = GUI.color;
            plotSmallerFontStyle ??= new GUIStyle(GUI.skin.label) { fontSize = PlotTextFontSize, wordWrap = true };
            plotSmallerCenteredFontStyle ??= new GUIStyle(plotSmallerFontStyle) { alignment = TextAnchor.MiddleCenter };

            if (segments < 1)
            {
                segments = 2;
            }

            if ((vFrom + PlotDrawEpsilon) >= vTo)
            {
                vFrom = vTo - PlotDrawEpsilon;
            }

            // Draw dark box behind
            GUI.color = new Color(.4f, .4f, .4f, .2f);
            GUI.DrawTexture(
                position,
                Texture2D.whiteTexture, ScaleMode.StretchToFill
            );
            GUI.color = guiPrevColor;

            // very naive plotting for GUI, using approximation + stepping (sigma)
            // If someone that is good at math saw this they would have a seizure
            // Here's how to make it less naive
            // A : Make it more efficient
            // B : A better drawing algorithm (perhaps use meshes? stepping is more different? idk.)
            // C : Actually learn about plotting and just, like, do the way it should be done.
            //     But unity doesn't give too many options on drawing unless you setup the whole rendering context yourself.
            // --
            // Check this for avoiding NaN explosion, probably a divide by zero happens if all is zero
            bool allValuesZero = Mathf.Approximately(plotMinValue, plotMaxValue) && Mathf.Approximately(plotMinValue, 0f);

            // Labels have a reserved 'PLOT_TEXT_PADDING' width
            Rect plotPosition = position;

            // Draw from/to text (x, positioned bottom)
            if (showFromToLabels)
            {
                plotPosition.height -= PlotTextPaddingY;

                plotSmallerFontStyle.alignment = TextAnchor.UpperLeft;
                Rect leftLabelRect = new Rect
                {
                    x = position.x,
                    y = position.yMax - PlotTextPaddingY,
                    width = 32f,
                    height = PlotTextPaddingY
                };
                if (showMinMaxLabels)
                {
                    leftLabelRect.x += PlotTextPaddingX;
                }
                GUI.Label(
                    leftLabelRect,
                    vFrom.ToString("0.0#"), plotSmallerFontStyle
                ); // left

                plotSmallerFontStyle.alignment = TextAnchor.UpperRight;
                Rect rightLabelRect = new Rect
                {
                    x = position.xMax - 32f,
                    y = position.yMax - PlotTextPaddingY,
                    width = 32f,
                    height = PlotTextPaddingY
                };
                GUI.Label(
                    rightLabelRect,
                    vTo.ToString("0.0#"), plotSmallerFontStyle
                ); // right
            }
            if (showMinMaxLabels)
            {
                plotPosition.x += PlotTextPaddingX;
                plotPosition.width -= PlotTextPaddingX;

                plotSmallerFontStyle.alignment = TextAnchor.UpperLeft;
                // Draw local min/max text (y, positioned left)
                Rect topLabelRect = new Rect
                {
                    x = position.x,
                    y = position.yMin,
                    width = 32f,
                    height = PlotTextPaddingY
                };
                GUI.Label(
                    topLabelRect,
                    plotMaxValue.ToString("0.0#"), plotSmallerFontStyle
                ); // up
                Rect bottomLabelRect = new Rect
                {
                    x = position.x,
                    y = position.yMax - PlotTextPaddingY,
                    width = 32f,
                    height = PlotTextPaddingY
                };
                if (showFromToLabels)
                {
                    // Offset again for the 'from-to' labels
                    bottomLabelRect.y -= PlotTextPaddingY;
                }
                GUI.Label(
                    bottomLabelRect,
                    plotMinValue.ToString("0.0#"), plotSmallerFontStyle
                ); // down
            }

            // This will throw a lot of errors, especially if the values are 0.
            if (allValuesZero)
            {
                // As a fallback, draw a line that goes through lowest part of 'plotPosition'
                LineFromTo(new Vector2(plotPosition.x, plotPosition.yMax), new Vector2(plotPosition.xMax, plotPosition.yMax), lineWidth);
                return;
            }

            // Draw the area divider (if suitable)
            //     |
            //     |
            // ---------]-> i call this divider lmao
            //     |
            //     |
            // Y divider
            if (vFrom < 0f && vTo > 0f)
            {
                // xMin is on the left
                float yDividerXpos = plotPosition.xMin + (plotPosition.width * Mathf.InverseLerp(vFrom, vTo, 0f));
                LineFromTo(new Vector2(yDividerXpos, plotPosition.yMin), new Vector2(yDividerXpos, plotPosition.yMax), 2, new Color(0.6f, 0.6f, 0.6f, 0.2f));
            }
            // X divider
            if (plotMinValue < 0f && plotMaxValue > 0f)
            {
                // yMax is on the bottom
                float xDividerYpos = plotPosition.yMax - (plotPosition.height * Mathf.InverseLerp(plotMinValue, plotMaxValue, 0f));
                LineFromTo(new Vector2(plotPosition.xMin, xDividerYpos), new Vector2(plotPosition.xMax, xDividerYpos), 2, new Color(0.6f, 0.6f, 0.6f, 0.2f));
            }

            Vector2 previousPosition = new Vector2(
                plotPosition.xMin,
                // Initial plot position
                plotPosition.y + (plotPosition.height * InverseLerp(plotMaxValue, plotMinValue, plotFunction(vFrom)))
            );

            for (int i = 1; i < segments + 1; i++)
            {
                float currentSegmentElapsed = (float)i / segments;
                float lerpValue = Mathf.Lerp(vFrom, vTo, currentSegmentElapsed);
                float plotValue = plotFunction(lerpValue);

                if (float.IsNaN(plotValue))
                {
                    Debug.LogError($"[GUIAdditionals::PlotLine] Given 'plotFunction' returns NaN for value '{lerpValue}'. This will cause issues.");
                    continue;
                }

                // Get yLerp between the plot values
                float yLerp = InverseLerp(plotMaxValue, plotMinValue, plotValue);

                float currentX = plotPosition.x + (currentSegmentElapsed * plotPosition.width);
                // 'y' is inverted in GUI
                // Closer to maximum, the less should be the added height
                float currentY = plotPosition.y + (plotPosition.height * yLerp);

                // Discard not visible at all lines
                // ---
                // This doesn't discard the probably visible lines but it also fails :
                // if ((previousPosition.y < plotPosition.y && yLerp < 0f) || (previousPosition.y > (plotPosition.y + plotPosition.height) && yLerp > 1f))
                // This discards the "should not visible" lines but it also discards the visible parts of some lines, which seems to be the better compromise.. :
                if ((previousPosition.y < plotPosition.y || previousPosition.y > (plotPosition.y + plotPosition.height)) || (yLerp < 0f && yLerp > 1f))
                {
                    previousPosition = new Vector2(currentX, currentY);
                    continue;
                }

                // A line is, linear (duh), so if the currentY is out of bounds, we can subtract the same amount (in the width scale) using the yLerp
                // This makes the line positioning better. (just ignore that the math is wrong, it works so if i can improve it i will but not feeling it)
                Vector2 lineToPosition = new Vector2(currentX, currentY);
                if (yLerp < 0f)
                {
                    lineToPosition.y = plotPosition.y;
                    // move 'currentX' by the given yLerp overshoot (yLerp is negative and less than 0 number)
                    lineToPosition.x -= (1f / segments) * yLerp;
                }
                else if (yLerp > 1f)
                {
                    lineToPosition.y = plotPosition.y + plotPosition.height;
                    // move 'currentX' by the given yLerp overshoot (yLerp is positive and larger than 1 number)
                    lineToPosition.x += (1f / segments) * (1f - yLerp);
                }

                LineFromTo(previousPosition, lineToPosition, lineWidth, GUI.color);

                previousPosition = new Vector2(currentX, currentY);
            }

            // Note : This tooltip shows only while Repaint()'ing constantly.
            // Show a tooltip on top of the cursor if we are on top of the value
            // The value tolerance will be scaled relatively with the lineWidth and the plotMinValue and plotMaxValue
            if (plotPosition.Contains(e.mousePosition))
            {
                float cursorXLerp = Mathf.Lerp(vFrom, vTo, (e.mousePosition.x - plotPosition.x) / plotPosition.width);
                float cursorPlotValue = Mathf.Lerp(plotMaxValue, plotMinValue, (e.mousePosition.y - plotPosition.y) / plotPosition.height);
                float cursorTolerance = Mathf.Abs(plotMaxValue - plotMinValue) *
                    // how to make mathe formula legit tutorial 2014
                    Mathf.MoveTowards(
                        0.33f, 0.06f,
                        (Mathf.Min(300f, plotPosition.width) / 600f) + (Mathf.Min(300f, plotPosition.height) / 600f)
                    );

                float cursorToPlot = plotFunction(cursorXLerp);
                // Inbetween the given tolerance values (do math ops only if the cursorToPlot is normal)
                if (float.IsNormal(cursorToPlot) && cursorPlotValue > (cursorToPlot - cursorTolerance) && cursorPlotValue < (cursorToPlot + cursorTolerance))
                {
                    int guiPrevDepth = GUI.depth;
                    GUI.depth++;

                    float drawRectWidth = 60f;
                    float drawRectXOffset = 0f;
#if UNITY_EDITOR
                    if ((e.mousePosition.x + drawRectWidth) > UnityEditor.EditorGUIUtility.currentViewWidth)
                    {
                        drawRectXOffset -= drawRectWidth;
                    }
#endif
                    Rect tooltipDrawRect = new Rect(e.mousePosition.x + drawRectXOffset, e.mousePosition.y - 35f, drawRectWidth, 30f);
                    GUI.color = new Color(0f, 0f, 0f, 0.5f);
                    GUI.DrawTexture(tooltipDrawRect, Texture2D.whiteTexture);
                    GUI.color = guiPrevColor;
                    GUI.Label(tooltipDrawRect, $"X:{cursorXLerp:0.0##}\nY:{cursorToPlot:0.0##}", plotSmallerCenteredFontStyle);

                    GUI.depth = guiPrevDepth;
                }
            }
        }
        /// <inheritdoc cref="Plot(Rect, Func{float, float}, bool, bool, float, float, float, float, float, int)"/>
        public static void Plot(Rect position, Func<float, float> plotFunction, bool showFromToLabels, bool showMinMaxLabels, float vFrom = 0f, float vTo = 1f, float lineWidth = 2.5f, int segments = 20)
        {
            // Calculate the min-max from the given 'plotFunction'
            // ----
            // Get local maximum value in the given range (because Y is calculated by min/max)
            float localMin = float.MaxValue; // Minimum text to draw
            float localMax = float.MinValue; // Maximum text to draw
            for (int i = 0; i < segments; i++)
            {
                // i is always 1 less then segments
                float currentSegmentElapsed = (float)i / (segments - 1);
                float lerpValue = Mathf.Lerp(vFrom, vTo, currentSegmentElapsed);

                float plotValue = plotFunction(lerpValue);

                if (plotValue > localMax)
                {
                    localMax = plotValue;
                }

                if (plotValue < localMin)
                {
                    localMin = plotValue;
                }
            }

            Plot(position, plotFunction, showFromToLabels, showMinMaxLabels, localMin, localMax, vFrom, vTo, lineWidth, segments);
        }
        /// <inheritdoc cref="Plot(Rect, Func{float, float}, bool, bool, float, float, float, float, float, int)"/>
        public static void Plot(Rect position, Func<float, float> plotFunction, float vFrom = 0f, float vTo = 1f, float lineWidth = 2.5f, int segments = 20)
        {
            Plot(position, plotFunction, true, true, vFrom, vTo, lineWidth, segments);
        }
    }
}
#endif
