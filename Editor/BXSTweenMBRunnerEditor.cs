#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System;
using BX.Tweening.Utility;
using BX.Tweening.Interop;

namespace BX.Tweening.Editor
{
    [CustomEditor(typeof(BXSTweenMBRunner))]
    public class BXSTweenMBRunnerEditor : UnityEditor.Editor
    {
        // Emoji to save this file as unicode UTF8 bom, because visual studio could also pull us with a UCS2 save : ❌
        // "this app can break" IsTextUnicode moment
        protected void CheckRepaint()
        {
            if (shouldRepaint)
            {
                Repaint();
            }
        }
        private void OnEnable()
        {
            // Put repaint of this inspector to update to get realtime data viewed on inspector.
            EditorApplication.update += CheckRepaint;
        }
        private void OnDisable()
        {
            EditorApplication.update -= CheckRepaint;
        }

        /// <summary>
        /// Used to filter tweenables, but it is now used as view options.
        /// </summary>
        private struct EditorTweensViewOptions
        {
            public bool reverseTweensView;
            public int breakAtTweenCount;
        }
        private EditorTweensViewOptions viewOptions;

        protected bool shouldRepaint = false;
        private const float TweensScrollAreaHeight = 400;
        private GUIStyle boxStyle;
        private GUIStyle headerTextStyle;
        private GUIStyle miniTextStyle;
        private GUIStyle detailsLabelStyle;
        private GUIStyle buttonStyle;

        // This should improve things a little.. Unfortunately IMGUI is inefficient and I dislike GUIElements for this type "crude" devtool
        private static readonly Dictionary<Type, PropertyInfo[]> m_PropertyTypeCache = new();
        private static readonly Dictionary<Type, FieldInfo[]> m_FieldInfoCache = new();
        protected static PropertyInfo[] GetPropertiesFromType(Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (m_PropertyTypeCache.TryGetValue(type, out var result))
            {
                return result;
            }

            result = type.GetProperties(flags);
            m_PropertyTypeCache.Add(type, result);
            return result;
        }
        protected static FieldInfo[] GetFieldsFromType(Type type, BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            if (m_FieldInfoCache.TryGetValue(type, out var result))
            {
                return result;
            }

            result = type.GetFields(flags);
            m_FieldInfoCache.Add(type, result);
            return result;
        }

        /// <summary>
        /// Initializes styles. Should be called from <c>OnGUI()</c> functions.
        /// </summary>
        protected void InitStyles()
        {
            boxStyle ??= new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleLeft,
                stretchWidth = false,
                //fixedWidth = EditorGUIUtility.currentViewWidth,
                fixedWidth = 600f,
                richText = true,
                fontSize = 14,
                //font = BXSTweenGUI.MonospaceFont
            };
            detailsLabelStyle ??= new GUIStyle(GUI.skin.label)
            {
                richText = true,
                fontSize = 14,
                //font = BXSTweenGUI.MonospaceFont
            };
            buttonStyle ??= new GUIStyle(GUI.skin.button);
            headerTextStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 18,
                fontStyle = FontStyle.Bold
            };
            miniTextStyle ??= new GUIStyle(EditorStyles.miniBoldLabel)
            {
                alignment = TextAnchor.UpperLeft,
                fontStyle = FontStyle.BoldAndItalic
            };
        }
        private int pageIndex = 0;
        protected const int EntriesPerPage = 100;
        private Vector2 tweensListScroll;
        private bool expandFilterDebugTweens = false;
        /// <summary>
        /// A boolean array for the allocated tweens list.
        /// </summary>
        private readonly List<bool> m_expandedTweens = new List<bool>(65535);
        protected virtual void DrawGUI(IBXSTweenLoop loop)
        {
            shouldRepaint = false;

            // Draw a field for 'm_Script'
            using (EditorGUI.DisabledScope scope = new EditorGUI.DisabledScope(true))
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            }

            // Draw ReadOnly status properties 
            if (!Application.isPlaying)
            {
                EditorGUILayout.HelpBox("BXSTween UnityRunner only works in runtime.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("[BXSTweenUnityRunner]", headerTextStyle, GUILayout.Height(32f));

            // Draw stats from the BXSTween class
            EditorGUILayout.LabelField(string.Format("Tween Amount = {0}", loop?.RunningTweens.Count));
            EditorGUILayout.LabelField(string.Format("Sequence Amount = {0}", loop?.RunningTweens.Where(t => t is BXSTweenSequence).Count()));
            EditorGUILayout.LabelField(string.Format("BXSTween Status = {0}", BXSTween.NeedsInitialization ? "Error (Needs Initialize)" : "OK"));

            if (loop == null)
            {
                EditorGUILayout.HelpBox("No IBXSTweenLoop supplied, cannot draw extra info.", MessageType.Warning);
                return;
            }

            // Draw the list of running tweens
            BXSTweenGUI.Line(GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, 8f), Color.gray);

            DrawTweensListGUI(loop.RunningTweens);
        }
        protected virtual void DrawTweensListGUI(List<BXSTweenable> tweens)
        {
            if (tweens == null)
            {
                throw new ArgumentNullException(nameof(tweens));
            }

            // Draw filter button/toggle + info text
            GUILayout.BeginHorizontal();
            expandFilterDebugTweens = GUILayout.Toggle(expandFilterDebugTweens, "Filter", buttonStyle, GUILayout.Width(70));
            GUILayout.Label("  -- Click on any box to view details about the tween --  ", miniTextStyle);
            GUILayout.EndHorizontal();
            if (expandFilterDebugTweens)
            {
                EditorGUI.indentLevel += 2;

                // Draw filter tweens area
                viewOptions.breakAtTweenCount = Mathf.Clamp(EditorGUILayout.IntField(
                    new GUIContent(
                        "Tween Amount To Pause (Break)",
                        "Pause editor after the amount of current tweens that is >= from this value.\nTo stop pausing set this value to 0 or lower."
                    ),
                    viewOptions.breakAtTweenCount), -1, int.MaxValue
                );
                viewOptions.reverseTweensView = EditorGUILayout.Toggle(
                    new GUIContent(
                        "Reverse Tweens View",
                        "Reverses the current view, so the last tween run is the topmost."
                    ),
                    viewOptions.reverseTweensView
                );

                EditorGUI.indentLevel -= 2;
            }
            // Pause editor if the tween amount exceeded
            if (viewOptions.breakAtTweenCount > 0 && tweens.Count >= viewOptions.breakAtTweenCount)
            {
                EditorApplication.isPaused = true;
            }

            // Draw the list of current running tweens (with name)

            // I give up on the "nice list virtualization", I can't blindly get the GUILayout element sizes
            // (without high perf penalty) and accumulate to a rect. Even if i could, it could shift drastically when the GUI is toggled..
            // Pagination it is. Makes it more usable on larger quantities of tweens instead of this being useless on that situation.
            GUILayout.Label($"Viewing {pageIndex}/{tweens.Count}", miniTextStyle);
            GUILayout.BeginHorizontal();
            using (EditorGUI.DisabledScope scope = new EditorGUI.DisabledScope(pageIndex <= 0))
                if (GUILayout.Button("←", GUILayout.Width(20f)))
                {
                    pageIndex = Math.Clamp(pageIndex - EntriesPerPage, 0, tweens.Count - 1);
                }
            GUILayout.FlexibleSpace();
            pageIndex = Math.Clamp(EditorGUILayout.IntField(pageIndex, GUILayout.Width(40f)), 0, tweens.Count - 1);
            GUILayout.FlexibleSpace();
            using (EditorGUI.DisabledScope scope = new EditorGUI.DisabledScope((pageIndex + EntriesPerPage) > tweens.Count))
                if (GUILayout.Button("→", GUILayout.Width(20f)))
                {
                    pageIndex = Math.Clamp(pageIndex + EntriesPerPage, 0, tweens.Count - 1);
                }
            GUILayout.EndHorizontal();
            tweensListScroll = GUILayout.BeginScrollView(tweensListScroll, GUILayout.Height(TweensScrollAreaHeight));
            for (int guiIndex = pageIndex; guiIndex < Math.Min(tweens.Count, pageIndex + EntriesPerPage); guiIndex++)
            {
                int tweenIndex = viewOptions.reverseTweensView ? tweens.Count - (guiIndex + 1) : guiIndex;
                BXSTweenable tween = tweens[tweenIndex];

                // Allocate toggles (use 'i' parameter, as it's the only one that goes sequentially)
                // We just want to reverse the 'CurrentRunningTweens'
                // Otherwise it's very easy to get ArgumentOutOfRangeException
                while (guiIndex > m_expandedTweens.Count - 1)
                {
                    m_expandedTweens.Add(false);
                }

                // All of these do complex 'CalcHeight' and 'GUIStyle' stuff
                // Which causes the extreme lag on the 10000 elements debug view

                // Get target type using reflection instead, no need to add extra attributes to the base types
                // > This allows for seamless debug display, without having to integrate the fields directly.
                try
                {
                    m_expandedTweens[guiIndex] = GUILayout.Toggle(m_expandedTweens[guiIndex], $"[*] Tween {tweenIndex} | Type={tween.GetType()}, Tag={tween.Tag}", boxStyle);
                }
                catch (Exception e)
                {
                    m_expandedTweens[guiIndex] = GUILayout.Toggle(m_expandedTweens[guiIndex], $"[!] Tween {tweenIndex} | Exception={e.Message}", boxStyle);
                }

                if (m_expandedTweens[guiIndex])
                {
                    shouldRepaint = true;
                    // Show more information about the tween
                    // Assume that this type is BXSTweenable, but the type details otherwise is runtime defined
                    // Interfaces always return concrete type : So GetType is used.
                    foreach (PropertyInfo v in GetPropertiesFromType(tween.GetType()))
                    {
                        // Unsupported index parameters, can be triggered by 'this[int idx]' expressions
                        if (v.GetIndexParameters().Length > 0)
                        {
                            continue;
                        }

                        GUILayout.Label(string.Format("  <color=#f3bd28>[ Property ]</color> <color=#2eb6ae>{0}</color> <color=#dcdcdc>{1}</color> = {2}", v.PropertyType.Name, v.Name, v.GetValue(tween)), detailsLabelStyle);
                    }
                    foreach (FieldInfo v in GetFieldsFromType(tween.GetType(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                    {
                        // Don't draw properties twice
                        if (v.Name.Contains("k__BackingField"))
                        {
                            continue;
                        }

                        GUILayout.Label(string.Format("  <color=#f3bd28>[ Field    ]</color> <color=#2eb6ae>{0}</color> <color=#dcdcdc>{1}</color> = {2}", v.FieldType.Name, v.Name, v.GetValue(tween)), detailsLabelStyle);
                    }

                    // Draw options for the tweenable
                    if (GUILayout.Button("Stop", GUILayout.Width(EditorGUIUtility.currentViewWidth)))
                    {
                        Debug.Log($"[BXSTween | EditorDebug] Stopped tween {tween}");
                        tween.Stop();
                    }
                }
            }
            if (BXSTween.CurrentLoop.RunningTweens.Count <= 0)
            {
                EditorGUILayout.HelpBox("There's no currently running tween.", MessageType.Info);
            }

            GUILayout.EndScrollView();
        }

        public override void OnInspectorGUI()
        {
            // TODO : 
            // 1 : Add filtering + Searchbar filtering
            // 2 : This unity editor only shows BXSTween.Current IBXSTweenLoop (it can't show arbitrary loops
            // unless custom editor is overridden. only the known and registered loops can be shown)
            //     (Because the unity editor GUI is only invoked in the main thread and
            //     that you are supposed to dispose the arbitrary loops to go back
            //     into the main thread from the async/other thread context, it literally can't)
            // --
            // For now this is just a direct port of the pretty old BXTweenCoreInspector with coloring,
            // which was totally fine except for times where there is a lot of tweens. (+ pagination virtualization now, because GUILayout)

            InitStyles();

            IBXSTweenLoop loop = null;
            try
            {
                loop = BXSTween.CurrentLoop;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }

            DrawGUI(loop);
        }
    }
}
#endif
