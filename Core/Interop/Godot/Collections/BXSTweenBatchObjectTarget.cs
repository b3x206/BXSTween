#if GODOT
using Godot;
using System.Collections.Generic;
using System.Collections;
using System;

namespace BX.Tweening.Interop.Collections
{
    /// <summary>
    /// Used to specify a link object that is a list of weakref objects. (proxy list class)
    /// </summary>
    public sealed class BXSTweenBatchObjectTarget : ICollection<GodotObject>, IBXSTweenLinkObject
    {
        private readonly List<WeakReference<GodotObject>> m_targets = new List<WeakReference<GodotObject>>();

        public int Count => m_targets.Count;
        public int Capacity
        {
            get => m_targets.Capacity;
            set => m_targets.Capacity = value;
        }
        public bool IsReadOnly => false;
        public bool CheckValidityOnce => true;
        public bool IsValid()
        {
            for (int i = 0; i < m_targets.Count; i++)
            {
                // GetRef() returns a default "Variant()"
                // But regardless, if the ref is disposed, the Variant class on
                // GodotSharp is struct and uses and derefs an unsafe pointer to read data from..
                // Hmm. Whatever, will use "WeakReference<T>" instead. Though maybe these
                // weak references _will_ increment the ref count
                var target = m_targets[i];

                if (!target.TryGetTarget(out var obj) || !GodotObject.IsInstanceValid(obj))
                {
                    return false;
                }
            }

            return true;
        }

        public BXSTweenBatchObjectTarget()
        { }
        public BXSTweenBatchObjectTarget(IEnumerable<GodotObject> collection)
        {
            foreach (var obj in collection)
            {
                m_targets.Add(new WeakReference<GodotObject>(obj));
            }
        }

        public void Add(GodotObject item)
        {
            m_targets.Add(new WeakReference<GodotObject>(item));
        }
        public bool Remove(GodotObject item)
        {
            for (int i = m_targets.Count - 1; i >= 0; i--)
            {
                var target = m_targets[i];
                if (target.TryGetTarget(out var obj) && obj == item)
                {
                    m_targets.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
        public void Clear()
        {
            m_targets.Clear();
        }
        public bool Contains(GodotObject item)
        {
            for (int i = 0; i < m_targets.Count; i++)
            {
                var target = m_targets[i];
                if (target.TryGetTarget(out var obj) && obj == item)
                {
                    return true;
                }
            }

            return false;
        }
        public void CopyTo(GodotObject[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if ((m_targets.Count + arrayIndex) > array.Length)
            {
                throw new ArgumentException("Given array size is less than the collection size + offset.", nameof(arrayIndex));
            }

            for (int i = 0; i < m_targets.Count; i++)
            {
                array[i + arrayIndex] = m_targets[i].TryGetTarget(out var obj) ? obj : null; ;
            }
        }
        public IEnumerator<GodotObject> GetEnumerator()
        {
            foreach (var target in m_targets)
            {
                yield return target.TryGetTarget(out var obj) ? obj : null;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif
