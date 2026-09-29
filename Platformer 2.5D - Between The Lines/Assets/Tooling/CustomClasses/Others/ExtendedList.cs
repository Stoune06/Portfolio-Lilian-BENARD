using System;
using System.Collections.Generic;
using System.Linq;

namespace Tooling
{
    public static class ExtendedList
    {
        private static Random _Rand = new Random();

        /// <summary>
        /// Shuffle the elements contained in your list
        /// </summary>
        public static void Shuffle<T>(this IList<T> tList)
        {
            for (int k = tList.Count() - 1; k > 1; k--) tList.Swap(k, _Rand.Next(k));
        }

        //// <summary>
        /// Swap the Position of the chosen element with their index. this function does not alters the references
        /// </summary>
        public static void Swap<T>(this IList<T> tList, int pInt, int pIntA)
        {
            T lT = tList[pInt];
            tList[pInt] = tList[pIntA];
            tList[pIntA] = lT;
        }

        /// <summary>
        /// Swap the Position of the chosen element . this function does not alters the references
        /// </summary>
        public static void Swap<T>(this IList<T> tList, T pT, T pTA) where T : class
        {
            tList.Swap(tList.IndexOf(pT), tList.IndexOf(pTA));
        }

        public static void Swap<T>(this IList<T> tList, IList<T> pList, int pInt, int pIntA) where T : class
        {
            T lT = tList[pInt];
            tList[pInt] = pList[pIntA];
            pList[pIntA] = lT;
        }
        public static void Transfer<T>(this IList<T> tList, IList<T> pList, int pIndex)
        {
            pList.Add(tList[pIndex]);
            tList.RemoveAt(pIndex);
        }

        public static void Transfer<T>(this IList<T> tList, IList<T> pList, T pT)
        {
            pList.Add(pT);
            tList.Remove(pT);
        }

        /// <summary>
        /// Return the first occurence from your List . Weirdly return the key and the value for dictionaries
        /// </summary>
        public static T GetFirst<T>(this IEnumerable<T> tEnumerable) => tEnumerable.ElementAt(0);

        /// <summary>
        /// Return the last occurence from your List . Weirdly return the key and the value for dictionaries
        /// </summary>
        public static T GetLast<T>(this IEnumerable<T> tEnumerable) => tEnumerable.ElementAt(tEnumerable.Count() - 1);

        /// <summary>
        /// Return a random occurence from your List . Weirdly return the key and the value for dictionaries 
        /// </summary>
        public static T GetRandomValue<T>(this IEnumerable<T> tEnumerable) => tEnumerable.ElementAt(_Rand.Next(tEnumerable.Count()));

        /// <summary>
        /// 
        /// </summary>
        public static void MultipleAdd<T>(this ICollection<T> tCollection, params T[] pArray)
        {
            for (int i = 0; i < pArray.Length; i++)
                tCollection.Add(pArray[i]);
        }

        public static ICollection<T> SubList<T>(this ICollection<T> tList, int pIndexStart, int pIndexEnd)
        {
            List<T> lList = new List<T>();
            for (int i = pIndexStart; i < pIndexEnd + 1; i++) lList.Add(tList.ElementAt(i));
            return lList;
        }

        public static bool IsEmpty<T>(this IEnumerable<T> tList) => tList.Count() == 0;

        /// <summary>
        /// This will Remove every object typed as pType in your Collection
        /// </summary>
        public static void RemoveByType<T>(this ICollection<T> tList, Type pType)
        {
            T lT;
            for (int i = tList.Count - 1; i >= 0; i--)
            {
                lT = tList.ElementAt(i);
                if (lT?.GetType() == pType) tList.Remove(lT);
            }
        }

        /// <summary>
        /// This will Remove every object typed differently than pType in your Collection
        /// </summary>
        public static void RemoveAllExceptType<T>(this ICollection<T> tList, Type pType)
        {
            T lT;
            for (int i = tList.Count - 1; i >= 0; i--)
            {
                lT = tList.ElementAt(i);
                if (lT?.GetType() != pType) tList.Remove(lT);
            }
        }

        /// <summary>
        /// This will Create a new IEnumerable containing all of the objects of pType
        /// </summary>
        public static IEnumerable<T> ExtrudeByType<T>(this IEnumerable<T> tEnumerable, Type pType)
        {
            foreach (T item in tEnumerable)
            {
                if (item?.GetType() == pType) yield return item;
            }
        }

        /// <summary>
        /// This will Create a new IEnumerable containing all of the objects of pType
        /// </summary>
        public static IEnumerable<T> ExtrudeAllExceptType<T>(this IEnumerable<T> tEnumerable, Type pType)
        {
            foreach (T item in tEnumerable)
            {
                if (item?.GetType() != pType) yield return item;
            }
        }
    }
}