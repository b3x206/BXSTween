#if UNITY_5_6_OR_NEWER
using System.Collections.Generic;
using System.Collections;
using Object = UnityEngine.Object;

namespace BX.Tweening.Interop.Collections
{
    /// <summary>
    /// Used to specify a link object that is a list of weakref objects. (proxy list class)
    /// </summary>
    public sealed class BXSTweenBatchObjectTarget : ICollection<Object>, IBXSTweenLinkObject
    {
        private readonly List<Object> m_targets = new List<Object>();

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
                if (m_targets[i] == null)
                {
                    return false;
                }
            }

            return true;
        }

        public BXSTweenBatchObjectTarget()
        { }
        public BXSTweenBatchObjectTarget(IEnumerable<Object> collection)
        {
            m_targets.AddRange(collection);
        }

        public void Add(Object item)
        {
            m_targets.Add(item);
        }
        public bool Remove(Object item)
        {
            return m_targets.Remove(item);
        }
        public void Clear()
        {
            m_targets.Clear();
        }
        public bool Contains(Object item)
        {
            return m_targets.Contains(item);
        }
        public void CopyTo(Object[] array, int arrayIndex)
        {
            m_targets.CopyTo(array, arrayIndex);
        }
        public IEnumerator<Object> GetEnumerator()
        {
            return m_targets.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
#endif
