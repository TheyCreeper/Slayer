using iTunesSearch.Library.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SlayerApp.utils
{
    public static class QueueListManager
    {
        /// <summary>
        /// Shuffles any given list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public static void Shuffle<T>(ref List<T> list)
        {
            var rng = Random.Shared;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        public static void Shuffle<T>(ref ObservableCollection<T> list)
        {
            var rng = Random.Shared;
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }

        /// <summary>
        /// Formats time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string FormatTime(TimeSpan time)
        {
            return time.TotalHours >= 1
                ? $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}"
                : $"{time.Minutes}:{time.Seconds:D2}";
        }
    }
}
