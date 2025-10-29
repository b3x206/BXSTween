using System;
using System.Collections.Generic;
using System.Linq;

namespace BX.Tweening
{
    /// <summary>
    /// Class used to defer tasks.
    /// <br>The <see cref="Tick"/> should be called in the same tween loop, owned by the <see cref="Interop.IBXSTweenLoop"/>.</br>
    /// </summary>
    public sealed class BXSTweenTaskDeferrer<T> where T : class
    {
        // -- Defer
        private int m_deferIterIndex = 0;
        private readonly List<(T tween, Action callback, int leftFrames)> m_deferredCallbacks = new List<(T tween, Action callback, int leftFrames)>();
        /// <summary>
        /// Function to tick and process the deferred tasks.
        /// </summary>
        public void Tick()
        {
            for (m_deferIterIndex = m_deferredCallbacks.Count - 1; m_deferIterIndex >= 0; m_deferIterIndex--)
            {
                (T tween, Action callback, int leftFrames) = m_deferredCallbacks[m_deferIterIndex];

                leftFrames--;
                m_deferredCallbacks[m_deferIterIndex] = (tween, callback, leftFrames);
                if (leftFrames <= 0)
                {
                    m_deferredCallbacks.RemoveAt(m_deferIterIndex);
                    callback();
                }
            }
        }

        public bool IsScheduled(T tween)
        {
            return m_deferredCallbacks.FindIndex(t => t.tween == tween) >= 0;
        }

        /// <summary>
        /// Defer an <paramref name="action"/> by given <paramref name="frameCount"/> that was requested by <paramref name="tween"/>.
        /// </summary>
        public void DeferFrames(T tween, Action action, int frameCount)
        {
            m_deferredCallbacks.Add((tween, action, frameCount));
        }
        public void CancelAllDeferActions(bool doCallbacks = false)
        {
            List<Action> callbacks = null;

            if (doCallbacks)
            {
                callbacks = new List<Action>(m_deferredCallbacks.Select(v => v.callback));
            }

            m_deferredCallbacks.Clear();
            m_deferIterIndex = -1;

            if (doCallbacks)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i]();
                }
            }
        }
        private void CancelDeferAction(Predicate<(T tween, Action callback, int leftFrames)> selector, bool doCallbacks = false)
        {
            // Depending on where we remove the tween, the iteration index should be moved.
            int index = m_deferredCallbacks.FindIndex(selector);
            List<Action> callbacks = doCallbacks ? new List<Action>() : null;

            while (index >= 0)
            {
                if (doCallbacks)
                {
                    callbacks.Add(m_deferredCallbacks[index].callback);
                }

                m_deferredCallbacks.RemoveAt(index);

                if (index <= m_deferIterIndex)
                {
                    m_deferIterIndex--;
                }

                index = m_deferredCallbacks.FindIndex(selector);
            }

            if (doCallbacks)
            {
                for (int i = 0; i < callbacks.Count; i++)
                {
                    callbacks[i]();
                }
            }
        }
        /// <summary>
        /// Cancells all defer action for <paramref name="tween"/>
        /// </summary>
        public void CancelDeferActions(T tween, bool doCallbacks = false)
        {
            CancelDeferAction(v => tween == v.tween, doCallbacks);
        }
        /// <summary>
        /// Cancells a specific defer <paramref name="action"/> for <paramref name="tween"/>
        /// </summary>
        public void CancelDeferAction(T tween, Action action, bool doCallbacks = false)
        {
            CancelDeferAction(v => tween == v.tween && action == v.callback, doCallbacks);
        }
    }
}
