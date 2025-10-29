using BX.Tweening.Events;
using System;
using System.Collections;
using System.Collections.Generic;

namespace BX.Tweening.Collections
{
    /// <summary>
    /// Contains batch management functions per the given <see cref="BXSTweenable"/> list (such as stop, play, etc.)
    /// <br>Also binds data to it.</br>
    /// </summary>
    public class BXSTweenableCollection : IList<BXSTweenable>
    {
        // > Isn't the BXSTweenSequence a BXSTweenableCollection technically?
        // Yes, but this one has the explicit intent of **not being a sequence** and instead being a batch management proxy for BXSTweenable.
        // > Shouldn't you have used a "base class" like BXSTweenableBehaviour and put those in here?
        // Eh.. I like repeating myself. Do repeat yourself, keep yapping. 99% of `sloc` users quit before they reach the 1m+ sloc, keep DRY away.

        // - Values
        private readonly List<BXSTweenable> m_list = new List<BXSTweenable>();
        /// <summary>
        /// Tag for this list if a certain tweenable collection with tags are received.
        /// <br>This value can be anything.</br>
        /// </summary>
        public readonly string tag;

        // - Batch State
        /// <summary>
        /// Check if any tweens are playing.
        /// </summary>
        public bool AnyPlaying
        {
            get
            {
                for (int i = 0; i < m_list.Count; i++)
                {
                    if (m_list[i].IsPlaying)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Check if all tweens are playing.
        /// </summary>
        public bool AllPlaying
        {
            get
            {
                for (int i = 0; i < m_list.Count; i++)
                {
                    if (m_list[i].IsPlaying)
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }
        }
        /// <summary>
        /// Check if any tweens are paused.
        /// </summary>
        public bool AnyPaused
        {
            get
            {
                for (int i = 0; i < m_list.Count; i++)
                {
                    if (m_list[i].IsPaused)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        /// <summary>
        /// Check if all tweens are paused.
        /// </summary>
        public bool AllPaused
        {
            get
            {
                for (int i = 0; i < m_list.Count; i++)
                {
                    if (m_list[i].IsPaused)
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }
        }

        public BXSTweenable this[int index] => m_list[index];
        BXSTweenable IList<BXSTweenable>.this[int index]
        {
            get => m_list[index];
            set => m_list[index] = value ?? throw new ArgumentNullException(nameof(value));
        }
        public int Count => m_list.Count;
        public bool IsReadOnly => false;
        public int Capacity { get => m_list.Capacity; set => m_list.Capacity = value; }

        // - Ctor
        public BXSTweenableCollection()
        { }
        public BXSTweenableCollection(string tag)
        {
            this.tag = tag;
        }
        public BXSTweenableCollection(string tag, int capacity)
        {
            this.tag = tag;
            m_list.Capacity = capacity;
        }
        public BXSTweenableCollection(IEnumerable<BXSTweenable> collection)
        {
            m_list.AddRange(collection);
        }
        public BXSTweenableCollection(string tag, IEnumerable<BXSTweenable> collection)
        {
            this.tag = tag;
            m_list.AddRange(collection);
        }

        // - Batch Manage
        /// <summary>
        /// Plays all tweens.
        /// </summary>
        public void Play()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Play();
            }
        }
        /// <summary>
        /// Plays all tweens from a set given progress.
        /// </summary>
        /// <param name="currentElapsed">Tween status elapsed in current loop or state.</param>
        /// <param name="loopsElapsed">Amount of loops elapsed.</param>
        public void PlayFrom(float currentElapsed, int loopsElapsed)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PlayFrom(currentElapsed, loopsElapsed);
            }
        }
        /// <summary>
        /// Plays all tweens from a set given progress.
        /// </summary>
        /// <param name="totalElapsed">Tween status elapsed total.</param>
        public void PlayFrom(float totalElapsed)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PlayFrom(totalElapsed);
            }
        }
        /// <summary>
        /// Pauses all tweens.
        /// </summary>
        public void Pause()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Pause();
            }
        }
        /// <summary>
        /// Stops all tweens.
        /// </summary>
        public void Stop()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Stop();
            }
        }
        /// <summary>
        /// Resets all tweens.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].Reset();
            }
        }
        /// <summary>
        /// Plays all tweens with 1 frame delay.
        /// </summary>
        public void PlayDelayed()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PlayDelayed();
            }
        }
        /// <summary>
        /// Plays all tweens with 1 frame delay.
        /// </summary>
        /// <param name="currentElapsed">Tween status elapsed in current loop or state.</param>
        /// <param name="loopsElapsed">Amount of loops elapsed.</param>
        public void PlayFromDelayed(float currentElapsed, int loopsElapsed)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PlayFromDelayed(currentElapsed, loopsElapsed);
            }
        }
        /// <summary>
        /// Plays all tweens with 1 frame delay.
        /// </summary>
        /// <param name="totalElapsed">Tween status elapsed total.</param>
        public void PlayFromDelayed(float totalElapsed)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PlayFromDelayed(totalElapsed);
            }
        }
        /// <summary>
        /// Pauses all tweens with 1 frame delay.
        /// </summary>
        public void PauseDelayed()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].PauseDelayed();
            }
        }
        /// <summary>
        /// Stops all tweens with 1 frame delay.
        /// </summary>
        public void StopDelayed()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].StopDelayed();
            }
        }
        /// <summary>
        /// Stops all children's XDelayed calls.
        /// </summary>
        public void CancelDelayedActions()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                m_list[i].CancelDelayedActions();
            }
        }

        /// <summary>
        /// Clears the <see cref="OnStartAction"/>
        /// </summary>
        public void ClearStartAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearStartAction();
            }
        }
        /// <summary>
        /// Clears the <see cref="OnTickAction"/>.
        /// </summary>
        public void ClearTickAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearUpdateAction();
            }
        }
        /// <summary>
        /// Clears the <see cref="OnTickAction"/>.
        /// <br>Alias for <see cref="ClearTickAction"/></br>
        /// </summary>
        public void ClearUpdateAction()
        {
            ClearTickAction();
        }
        /// <summary>
        /// Clears the <see cref="OnPauseAction"/>.
        /// </summary>
        public void ClearPauseAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearPauseAction();
            }
        }
        /// <summary>
        /// Clears the <see cref="OnEndAction"/>.
        /// </summary>
        public void ClearEndAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearEndAction();
            }
        }
        /// <summary>
        /// Clears the <see cref="OnStopAction"/>.
        /// </summary>
        public void ClearStopAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearStopAction();
            }
        }
        /// <summary>
        /// Clears the <see cref="TickConditionAction"/>.
        /// </summary>
        public void ClearTickConditionAction()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearTickConditionAction();
            }
        }
        /// <summary>
        /// Clears all control actions.
        /// </summary>
        public void ClearAllActions()
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                tw.ClearAllActions();
            }
        }

        // - Setter
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnPlayAction"/> event.
        /// <br>This is called when <see cref="BXSTweenable.Play"/> is called on this tween.</br>
        /// </summary>
        public BXSTweenableCollection SetPlayAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnPlayAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnPlayAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnPlayAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnStartAction"/> event.
        /// <br>This is called when the tween has waited out it's delay and it is starting for the first time.</br>
        /// </summary>
        public BXSTweenableCollection SetStartAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnStartAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnStartAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnStartAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnTickAction"/> event.
        /// <br>This is called every time the tween ticks. It is started to be called after the delay was waited out.</br>
        /// </summary>
        public BXSTweenableCollection SetTickAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnTickAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnTickAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnTickAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnTickAction"/> event.<br/>
        /// This method is an alias for <see cref="SetTickAction(BXSAction, ValueSetMode)"/>.
        /// </summary>
        public BXSTweenableCollection SetUpdateAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            return SetTickAction(action, mode);
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnPauseAction"/> event.
        /// <br>It is called when <see cref="BXSTweenable.Pause"/> is called on this tween.</br>
        /// </summary>
        public BXSTweenableCollection SetPauseAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnPauseAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnPauseAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnPauseAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnLoopRepeatAction"/> event.
        /// <br>This is called when the loop repeats.</br>
        /// </summary>
        public BXSTweenableCollection SetLoopRepeatAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnLoopRepeatAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnLoopRepeatAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnLoopRepeatAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnEndAction"/> event.
        /// <br>The difference between the <see cref="SetStopAction(BXSAction, ValueSetMode)"/> 
        /// and this is that this only gets invoked when the tween ends after the tweens duration.</br>
        /// <br/>
        /// <br><b>Note : </b> If you are want to play the same tween from this tweens ending action use <see cref="SetStopAction(BXSAction, ValueSetMode)"/> instead,
        /// this is due to the <see cref="BXSTweenable.Stop"/> sets <see cref="BXSTweenable.IsPlaying"/> to false immediately after this event.</br>
        /// <br>Or use <see cref="BXSTweenable.PlayDelayed"/> to delay the playback by one frame.</br>
        /// <br>Or use <see cref="BXSTweenSequence"/> in conjuction with <see cref="BXSTweenable.AsCopy{T}"/>.</br>
        /// </summary>
        public BXSTweenableCollection SetEndAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnEndAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnEndAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnEndAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.OnStopAction"/> event.
        /// <br>The difference between the <see cref="SetEndAction(BXSAction, ValueSetMode)"/>
        /// and this is that this gets called both when the tween ends or when <see cref="BXSTweenable.Stop"/> gets called.</br>
        /// </summary>
        public BXSTweenableCollection SetStopAction(BXSAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.OnStopAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.OnStopAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.OnStopAction = action;
                        break;
                }
            }

            return this;
        }
        /// <summary>
        /// Sets the <see cref="BXSTweenable.TickConditionAction"/> action.
        /// <br>Return the suitable <see cref="TickSuspendAction"/> in the function.</br>
        /// </summary> 
        public BXSTweenableCollection SetTickConditionAction(BXSTickConditionAction action, ValueSetMode mode = ValueSetMode.Equals)
        {
            for (int i = 0; i < m_list.Count; i++)
            {
                var tw = m_list[i];
                switch (mode)
                {
                    case ValueSetMode.Remove:
                        tw.TickConditionAction -= action;
                        break;
                    case ValueSetMode.Add:
                        tw.TickConditionAction += action;
                        break;

                    default:
                    case ValueSetMode.Equals:
                        tw.TickConditionAction = action;
                        break;
                }
            }

            return this;
        }

        // - List
        /// <summary>
        /// Add a tween to this collection.
        /// </summary>
        /// <param name="item">Item to add. This musn't be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Add(BXSTweenable item)
        {
            m_list.Add(item ?? throw new ArgumentNullException(nameof(item)));
        }
        /// <summary>
        /// Clears this collection.
        /// </summary>
        public void Clear()
        {
            m_list.Clear();
        }
        /// <summary>
        /// Check if <paramref name="item"/> is in this collection.
        /// </summary>
        public bool Contains(BXSTweenable item)
        {
            if (item == null)
            {
                return false;
            }

            return m_list.Contains(item);
        }
        public void CopyTo(BXSTweenable[] array, int arrayIndex)
        {
            m_list.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Locate the index of given element.
        /// </summary>
        /// <returns>Index of <paramref name="item"/>. <c>-1</c> if the <paramref name="item"/> doesn't exist.</returns>
        public int IndexOf(BXSTweenable item)
        {
            if (item == null)
            {
                return -1;
            }

            return m_list.IndexOf(item);
        }
        /// <summary>
        /// Remove a tween from this collection.
        /// </summary>
        /// <param name="item">Tween to remove.</param>
        /// <returns><see langword="true"/> if the item existed and was removed, <see langword="false"/> otherwise.</returns>
        public bool Remove(BXSTweenable item)
        {
            if (item == null)
            {
                return false;
            }

            return m_list.Remove(item);
        }
        /// <summary>
        /// Insert an <paramref name="item"/> at given <paramref name="index"/>
        /// </summary>
        /// <param name="item">Item to insert. This musn't be <see langword="null"/>.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void Insert(int index, BXSTweenable item)
        {
            m_list.Insert(index, item ?? throw new ArgumentNullException(nameof(item)));
        }
        /// <summary>
        /// Remove an item at given index.
        /// </summary>
        public void RemoveAt(int index)
        {
            m_list.RemoveAt(index);
        }

        public IEnumerator<BXSTweenable> GetEnumerator()
        {
            return m_list.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}