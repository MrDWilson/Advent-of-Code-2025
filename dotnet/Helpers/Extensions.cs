namespace AdventOfCode.Helpers;

public static class Extensions
{
    public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> seq, Func<T, T, bool> condition)
    {
        T prev = seq.First();
        List<T> list = [prev];

        foreach (T item in seq.Skip(1))
        {
            if (condition(prev, item) == false)
            {
                yield return list;
                list = [];
            }
            list.Add(item);
            prev = item;
        }

        yield return list;
    }

    public static IEnumerable<(T, T)> UniquePairs<T>(this ICollection<T> items) where T : notnull
    {
        var unique = items.Distinct().ToArray();

        for (int i = 0; i < unique.Length - 1; i++)
            for (int j = i + 1; j < unique.Length; j++)
                yield return (unique[i], unique[j]);
    }
}