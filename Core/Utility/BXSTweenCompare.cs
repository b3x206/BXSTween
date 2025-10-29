using System;
using System.Collections.Generic;
using System.Reflection;

namespace BX.Tweening.Utility
{
    public static class BXSTweenCompare
    {
        public sealed class UnitySafeEqualityComparer : IEqualityComparer<object>
        {
            public new bool Equals(object x, object y)
            {
                return BXSTweenCompare.Equals(x, y);
            }
            public int GetHashCode(object obj)
            {
                return BXSTweenCompare.GetHashCode(obj);
            }
        }

        /// <summary>
        /// Checks whether if two c# objects are equal, with respect to the fake null 
        /// (because it's a weakref handle) unity Object's.
        /// </summary>
        /// <param name="x">Left-hand side object to compare.</param>
        /// <param name="y">Right-hand side object to compare.</param>
        /// <returns>Whether if two objects are equal.</returns>
        public static new bool Equals(object x, object y)
        {
#if UNITY_5_6_OR_NEWER
            // Use unity object comparison, because UnityEngine's Object are WeakRef
            if (x is UnityEngine.Object xObj)
            {
                return xObj.Equals(y);
            }
            if (y is UnityEngine.Object yObj)
            {
                return yObj.Equals(x);
            }
#endif
            // Assuming non-special procedure for null reference comparison
            // on C# objects that don't manage special references..
            // -
            // Though, this still may fail unless the object gets GC disposed
            // if it has explicit lifetime, which on a game engine context, it does.
            // -
            // Because of this, if one object isn't reference null, do still attempt to get the type.
            bool xIsNull = x is null;
            bool yIsNull = y is null;

            if (xIsNull)
            {
                if (yIsNull)
                {
                    return true;
                }

                return GetEqualityComparerResult(y.GetType(), y, x);
            }

            if (yIsNull)
            {
                if (xIsNull)
                {
                    return true;
                }
            }

            return GetEqualityComparerResult(x.GetType(), x, y);
        }
        public static int GetHashCode(object obj)
        {
#if UNITY_5_6_OR_NEWER
            if (obj is UnityEngine.Object unityObject)
            {
                if (unityObject == null)
                {
                    return 0;
                }
            }
#endif
            return obj.GetHashCode();
        }

        /// <summary>
        /// Used to cache the created <see cref="EqualityComparer{T}"/>s of <see cref="GetEqualityComparerResult(Type, object, object)"/>.
        /// </summary>
        private static readonly Dictionary<Type, (object, MethodInfo)> m_typedEqualityComparers = new Dictionary<Type, (object, MethodInfo)>(128);
        private static readonly object m_typedEqualityComparerLock = new object();
        /// <summary>
        /// An utility method used to get the typed <see cref="EqualityComparer{T}.Default"/>'s comparison result with the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">
        /// Type of the both <paramref name="lhs"/> and <paramref name="rhs"/>.
        /// An <see cref="ArgumentException"/> will be thrown (by the reflection utility) on invocation if the types mismatch.
        /// </param>
        public static bool GetEqualityComparerResult(Type type, object lhs, object rhs)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            (object typedComparer, MethodInfo typedComparerEqualsMethod) values;
            lock (m_typedEqualityComparerLock)
            {
                if (!m_typedEqualityComparers.TryGetValue(type, out values))
                {
                    Type typedComparerType = typeof(EqualityComparer<>).MakeGenericType(type);
                    values.typedComparer = typedComparerType
                        .GetProperty(nameof(EqualityComparer<object>.Default), BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    values.typedComparerEqualsMethod = typedComparerType
                        .GetMethod(nameof(EqualityComparer<object>.Equals), 0, new Type[] { type, type });

                    // add dict value
                    m_typedEqualityComparers.Add(type, values);
                }
                else if (values.typedComparer is null || values.typedComparerEqualsMethod is null)
                {
                    // typedComparer is null, have to fix the dict value
                    Type typedComparerType = typeof(EqualityComparer<>).MakeGenericType(type);
                    // EqualityComparer<type>.Default
                    values.typedComparer = typedComparerType
                        .GetProperty(nameof(EqualityComparer<object>.Default), BindingFlags.Public | BindingFlags.Static)
                        .GetValue(null);
                    // EqualityComparer<type>.Default.Equals(lhs, rhs)
                    values.typedComparerEqualsMethod = typedComparerType
                        .GetMethod(nameof(EqualityComparer<object>.Equals), 0, new Type[] { type, type });

                    // set dict value to fixed tuple
                    m_typedEqualityComparers[type] = values;
                }
            }

            // While invocation may allocate garbage, this is better than just constantly creating an object.
            return (bool)values.typedComparerEqualsMethod.Invoke(values.typedComparer, new object[] { lhs, rhs });
        }
    }
}