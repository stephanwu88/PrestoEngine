﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace Engine.WpfBase
{
    public static class ObservableExtension
    {
        public static void Sort<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        public static void Sort<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector, bool isdesc)
        {
            List<TSource> sortedList = isdesc ? source.OrderByDescending(keySelector).ToList() : source.OrderBy(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        
        public static void SortDesc<TSource, TKey>(this Collection<TSource> source, Func<TSource, TKey> keySelector)
        {
            List<TSource> sortedList = source.OrderByDescending(keySelector).ToList();
            source.Clear();
            foreach (var sortedItem in sortedList)
                source.Add(sortedItem);
        }

        /// <summary> 排序 </summary>
        public static void Sort<T>(this ObservableCollection<T> collection) where T : IComparable
        {
            List<T> sortedList = collection.OrderBy(x => x).ToList();
            for (int i = 0; i < sortedList.Count(); i++)
            {
                collection.Move(collection.IndexOf(sortedList[i]), i);
            }
        }

        /// <summary> 转成 ObservableCollection 集合 </summary>
        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> collection)
        {

            //ObservableCollection<T> result = new ObservableCollection<T>();
            //foreach (var item in collection)
            //{
            //    result.Add(item);
            //}
            return new ObservableCollection<T>(collection);

        }
        /// <summary> 调用主线程执行Action </summary>
        public static void Invoke<T>(this ObservableCollection<T> collection, Action<ObservableCollection<T>> action)
        {
            Application.Current.Dispatcher.Invoke(() => action(collection));

        }
        /// <summary> 调用主线程执行Action </summary>
        public static void BeginInvoke<T>(this ObservableCollection<T> collection, Action<ObservableCollection<T>> action)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle,new Action(() => action(collection)));

        }

        /// <summary> 更新集合 通知UI </summary>
        public static void Refresh<T>(this ObservableCollection<T> collection)
        {

            //ObservableCollection<T> result = new ObservableCollection<T>();

            //foreach (var item in collection)
            //{
            //    result.Add(item);
            //}

            //collection = result;

            collection = new ObservableCollection<T>(collection);
        }

        /// <summary> 对集合中的 每一项执行Action </summary>
        public static void Foreach<T>(this ObservableCollection<T> collection, Action<T> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
        /// <summary> 随机排序 </summary>
        public static ObservableCollection<T> Random<T>(this ObservableCollection<T> collection)
        {
            Random random = new Random();
            T temp;

            for (int i = 0; i < collection.Count; i++)
            {
                int index = random.Next(i, collection.Count - 1);

                temp = collection[i];

                collection[i] = collection[index];

                collection[index] = temp;
            }

            return collection;
        }
    }
}
