using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace L2NPCManager.IO.Extensions
{
    public static class ObservableCollectionExtensions
    {
        public static void AddAll<T>(this ObservableCollection<T> collection, IList items) {
            int c = items.Count;
            for (int i = 0; i < c; i++) {
                collection.Add((T)items[i]);
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> collection, IList items) {
            for (int i = collection.Count - 1; i >= 0; i--) {
                if (items.Contains(collection[i])) collection.RemoveAt(i);
            }
        }

        public static void RemoveAll<T>(this ObservableCollection<T> collection, Func<T, bool> condition) {
            for (int i = collection.Count - 1; i >= 0; i--) {
                if (condition(collection[i])) collection.RemoveAt(i);
            }
        }
    }
}