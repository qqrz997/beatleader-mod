﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace BeatLeader.Utils {
    public static class CollectionsExtension {
        #region Sorting

        public static void Sort<T>(this IList<T> list, IComparer<T> comparer) {
            QuickSort(list, 0, list.Count - 1, comparer);
        }

        private static void QuickSort<T>(IList<T> list, int left, int right, IComparer<T> comparer) {
            if (left >= right) return;
            var pivotIndex = Partition(list, left, right, comparer);
            QuickSort(list, left, pivotIndex - 1, comparer);
            QuickSort(list, pivotIndex + 1, right, comparer);
        }

        private static int Partition<T>(IList<T> list, int left, int right, IComparer<T> comparer) {
            var pivot = list[right];
            var i = left - 1;

            for (var j = left; j < right; j++) {
                if (comparer.Compare(list[j], pivot) > 0) continue;
                i++;
                Swap(list, i, j);
            }

            Swap(list, i + 1, right);
            return i + 1;
        }

        private static void Swap<T>(IList<T> list, int indexA, int indexB) {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        #endregion

        public static void Reverse<T>(this IList<T> list) {
            var count = list.Count;
            for (var i = 0; i < count / 2; i++) {
                (list[i], list[count - i - 1]) = (list[count - i - 1], list[i]);
            }
        }

        public static LinkedListNode<T>? FindNode<T>(this LinkedList<T> list, Func<LinkedListNode<T>, bool> predicate) {
            var node = list.First;
            while (node is not null) {
                if (predicate(node)) return node;
                node = node.Next;
            }
            return null;
        }

        public static IEnumerable<T> TakeIndexes<T>(this IList<T> collection, IEnumerable<int> indexes) {
            return indexes.Select(index => collection[index]);
        }

        public static IEnumerable<T> TakeIndexes<T>(this IEnumerable<T> collection, IEnumerable<int> indexes) {
            return indexes.Select(collection.ElementAt);
        }

        public static void RemoveValue<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TValue value
        ) {
            var item = dictionary.First(x => x.Value?.Equals(value) ?? false);
            dictionary.Remove(item.Key);
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value) {
            if (!dictionary.TryGetValue(key, out var oValue)) {
                dictionary[key] = value;
                oValue = value;
            }
            return oValue;
        }
        
        public static void EnsureExistsAndDo<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key, TValue value, Action<TValue> action
        ) {
            if (!dictionary.TryGetValue(key, out var oValue)) {
                dictionary[key] = value;
                oValue = value;
            }
            action(oValue);
        }

        public static void DoIfExists<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary,
            TKey key, Action<TValue> action
        ) {
            if (dictionary.TryGetValue(key, out var oValue)) {
                action(oValue);
            }
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            return dictionary as Dictionary<TKey, TValue> ?? new(dictionary);
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) {
            foreach (var item in enumerable) action(item);
        }

        public static void ForEach<T, K>(this IDictionary<T, K> enumerable, Action<T, K> action) {
            foreach (var item in enumerable) action(item.Key, item.Value);
        }

        public static void AddRange<T>(this ICollection<T> list, IEnumerable<T> range) {
            foreach (var item in range) list.Add(item);
        }

        public static void RemoveRange<T>(this ICollection<T> list, IEnumerable<T> range) {
            foreach (var item in range) list.Remove(item);
        }

        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) {
            if (!dictionary.ContainsKey(key)) {
                dictionary.Add(key, value);
                return true;
            } else return false;
        }

        public static bool TryPeek<TValue>(this Stack<TValue> stack, out TValue? value) {
            value = default;
            if (stack.Count >= 1) {
                value = stack.Peek();
                return true;
            } else return false;
        }

        public static bool TryPop<TValue>(this Stack<TValue> stack, out TValue? value) {
            value = default;
            if (stack.Count >= 1) {
                value = stack.Pop();
                return true;
            } else return false;
        }
    }
}