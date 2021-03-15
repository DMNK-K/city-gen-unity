using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtCollections
{
    public static T RandomElement<T>(this T[] array)
    {
        if (array.Length < 1) { return default(T); }
        return array[Random.Range(0, array.Length)];
    }

    public static T RandomElement<T>(this List<T> l)
    {
        if (l.Count < 1) { return default(T); }
        return l[Random.Range(0, l.Count)];
    }

    public static T RandomElementWithBias<T>(this List<T> l, List<float> bias)
    {
        if (l.Count < 1) { return default(T); }
        if (bias.Count > l.Count)
        {
            bias.RemoveRange(l.Count, bias.Count - l.Count);
        }
        if (bias.Count < l.Count)
        {
            for (int i = 0; i < l.Count - bias.Count; i++)
            {
                bias.Add(bias.Last());
            }
        }

        float n = 0;
        for (int i = 0; i < bias.Count; i++)
        {
            bias[i] = Mathf.Abs(bias[i]);
            n += bias[i];
        }
        float rand = Random.Range(0, n);
        n = 0;
        for (int i = 0; i < l.Count; i++)
        {
            n += bias[i];
            if (rand <= n) { return l[i]; }
        }
        return default(T);
    }

    public static void DistributeEvenly(this List<int> l, int n)
    {
        l.DistributeEvenly(n, 0, l.Count - 1);
    }

    public static void DistributeEvenly(this List<int> l, int n, int from, int to)
    {
        //distributes number n as evenly as possible to all elements of list
        //from index from to index to, both inclusive
        if (n <= 0) { return; }
        if (from > to)
        {
            int temp = to;
            to = from;
            from = temp;
        }
        if (from < 0) { from = 0; }
        if (to >= l.Count) { from = l.Count; }
        int remainder = n % (to - from + 1);
        int to_distribute = n - remainder;
        for (int i = from; i <= to; i++)
        {
            l[i] += to_distribute;
        }
        l[to] += remainder;
    }

    public static float Sum(this List<float> l)
    {
        return l.SumFromTo(0, l.Count - 1);
    }

    public static int Sum(this List<int> l)
    {
        return l.SumFromTo(0, l.Count - 1);
    }

    public static float SumFromTo(this List<float> l, int from, int to)
    {
        //sums elements of list from index "from" to index "to", both inclusive
        if (l == null || l.Count == 0) { return 0; }
        if (from > to)
        {
            int temp = to;
            to = from;
            from = temp;
        }
        if (from < 0) { from = 0; }
        if (to >= l.Count) { from = l.Count; }
        float sum = 0;
        for (int i = from; i <= to; i++)
        {
            sum += l[i];
        }
        return sum;
    }

    public static int SumFromTo(this List<int> l, int from, int to)
    {
        //sums elements of list from index "from" to index "to", both inclusive
        if (l == null || l.Count == 0) { return 0; }
        if (from > to)
        {
            int temp = to;
            to = from;
            from = temp;
        }
        if (from < 0) { from = 0; }
        if (to >= l.Count) { from = l.Count; }
        int sum = 0;
        for (int i = from; i <= to; i++)
        {
            sum += l[i];
        }
        return sum;
    }

    public static List<T> Flatten<T>(this List<List<T>> l)
    {
        //flattens list of lists into 1 list
        List<T> return_list = new List<T>();
        for (int i = 0; i < l.Count; i++)
        {
            return_list.AddRange(l[i]);
        }
        return return_list;
    }

    public static string ToStr<T>(this List<T> list, string sep = " | ")
    {
        string s = "";
        if (list.Count < 1) { return s; }
        s += list[0].ToString();
        for (int i = 1; i < list.Count; i++)
        {
            s += sep;
            s += list[i].ToString();
        }
        return s;
    }

    public static T Last<T>(this List<T> l) { return l[l.Count - 1]; }
    public static T Last<T>(this T[] a) { return a[a.Length - 1]; }

    public static void RemoveLast<T>(this List<T> l, int n = 1)
    {
        l.RemoveRange(l.Count - n, 1);
    }

    public static void PushAndTrim<T>(this List<T> l, T item, int trim_to_size)
    {
        l.Insert(0, item);
        if (l.Count > trim_to_size)
        {
            l.RemoveRange(trim_to_size, l.Count - trim_to_size);
        }
    }

    public static int RandomIndex<T>(this List<T> l)
    {
        if (l != null && l.Count > 0)
        {
            return Random.Range(0, l.Count);
        }
        return -1;
    }

    public static void AddIfAbsent<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
    {
        if (!d.ContainsKey(key)) { d.Add(key, value); }
    }

    public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
    {
        if (!d.ContainsKey(key)) { d.Add(key, value); }
        else { d[key] = value; }
    }

    public static void AddIfAbsent<T>(this List<T> l, T element)
    {
        if (!l.Contains(element))
        {
            l.Add(element);
        }
    }

    public static void AddIfAbsent<T>(this HashSet<T> set, T element)
    {
        if (!set.Contains(element))
        {
            set.Add(element);
        }
    }

    public static bool ContainsAnyOf<T>(this List<T> l, List<T> elements)
    {
        if (l == null || elements == null) { return false; }
        for (int i = 0; i < l.Count; i++)
        {
            for (int q = 0; q < elements.Count; q++)
            {
                if (Compare(l[i], elements[q])) { return true; }
            }
        }
        return false;
    }

    public static bool ContainsAnyOf<T>(this List<T> l, params T[] elements)
    {
        if (l == null || elements.Length <= 0) { return false; }
        for (int i = 0; i < l.Count; i++)
        {
            for (int q = 0; q < elements.Length; q++)
            {
                if (Compare(l[i], elements[q])) { return true; }
            }
        }
        return false;
    }

    public static T[] SubArray<T>(this T[] array, int start, int end)
    {
        //start and end inclusive
        if (start > end || array.Length < 1 || start >= array.Length) { return new T[0] { }; }
        if (start < 0) { start = 0; }
        if (end >= array.Length) { end = array.Length - 1; }
        T[] new_array = new T[end - start + 1];
        for (int i = 0; i < new_array.Length; i++)
        {
            new_array[i] = array[i + start];
        }
        return new_array;
    }

    private static bool Compare<T>(T x, T y)
    {
        return EqualityComparer<T>.Default.Equals(x, y);
    }

    public static string ContentsToString<T>(this List<T> l, string separator = ", ")
    {
        string str = "";
        if (l != null)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (i != 0)
                {
                    str += separator;
                }
                str += l[i].ToString();
            }
        }
        return str;
    }

    public static string ContentsToString<T>(this HashSet<T> set, string separator = ", ")
    {
        string str = "";
        if (set != null)
        {
            bool first = true;
            foreach (T item in set)
            {
                if (!first)
                {
                    str += separator;
                }
                first = false;
                str += item.ToString();
            }
        }
        return str;
    }

    public static int MaxKey<T>(this Dictionary<int, T> dic)
    {
        int max = int.MinValue;
        foreach (int key in dic.Keys)
        {
            if (key > max) { max = key; }
        }
        return max;
    }

    public static float MaxKey<T>(this Dictionary<float, T> dic)
    {
        float max = float.NegativeInfinity;
        foreach (float key in dic.Keys)
        {
            if (key > max) { max = key; }
        }
        return max;
    }

    public static int MinKey<T>(this Dictionary<int, T> dic)
    {
        int min = int.MaxValue;
        foreach (int key in dic.Keys)
        {
            if (key < min) { min = key; }
        }
        return min;
    }

    public static float MinKey<T>(this Dictionary<float, T> dic)
    {
        float min = float.PositiveInfinity;
        foreach (float key in dic.Keys)
        {
            if (key < min) { min = key; }
        }
        return min;
    }
}
