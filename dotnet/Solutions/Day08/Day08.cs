using System.Numerics;
using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day08(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 8;

    public override async Task<string> Solve()
    {
        var rawCoords = await LoadLines<string>();
        var coords = rawCoords
            .Select(x => x.Split(','))
            .Select(x => new Vector3(float.Parse(x[0]), float.Parse(x[1]), float.Parse(x[2])))
            .ToHashSet();
        var coordPairs = UniquePairs(coords);
        var alreadyMatched = new HashSet<(Vector3, Vector3)>();

        if (CurrentOptions.SolutionType is SolutionType.First)
        {
            var matchCount = CurrentOptions.RunType is RunType.Test ? 10 : 1000;

            foreach (var _ in Enumerable.Range(0, matchCount))
            {
                var shortestDistance = coordPairs.Where(x => !alreadyMatched.Contains(x)).Min(x => Vector3.Distance(x.Item1, x.Item2));
                var closestPair = coordPairs.Where(x => !alreadyMatched.Contains(x)).First(x => Vector3.Distance(x.Item1, x.Item2) == shortestDistance);
                alreadyMatched.Add(closestPair);
            }

            var regionSizes = RegionSizes(alreadyMatched);
            var topThree = regionSizes.OrderByDescending(x => x).Take(3);
            var result = topThree.Aggregate(1, (acc, x) => acc * x);

            return result.ToString();
        }
        else
        {
            (Vector3, Vector3) lastPair = (Vector3.Zero, Vector3.Zero);
            var allDistances = coordPairs
                .Select(x => (Coords: x, Distance: Vector3.Distance(x.Item1, x.Item2)))
                .OrderByDescending(x => x.Distance)
                .ToList();
            var distances = new Stack<((Vector3, Vector3) Coords, float Distance)>(allDistances);
            var linkedItems = new HashSet<Vector3>();
            while (true)
            {
                if (coords.SetEquals(linkedItems))
                {
                    return ((long)lastPair.Item1.X * (long)lastPair.Item2.X).ToString();
                }

                var (Coords, Distance) = distances.Pop();

                if (linkedItems.Contains(Coords.Item1) && linkedItems.Contains(Coords.Item2))
                {
                    continue;
                }

                linkedItems.Add(Coords.Item1);
                linkedItems.Add(Coords.Item2);
                lastPair = Coords;
            }
        }
    }

    public static IEnumerable<(T, T)> UniquePairs<T>(HashSet<T> items) where T : notnull
    {
        var unique = items.Distinct().ToArray();

        for (int i = 0; i < unique.Length - 1; i++)
            for (int j = i + 1; j < unique.Length; j++)
                yield return (unique[i], unique[j]);
    }

    public static List<int> RegionSizes(IEnumerable<(Vector3 A, Vector3 B)> edges)
    {
        var parent = new Dictionary<Vector3, Vector3>();

        Vector3 Find(Vector3 x) =>
            parent.TryGetValue(x, out var p) && !p.Equals(x)
                ? parent[x] = Find(p)
                : parent[x] = x;

        void Union(Vector3 a, Vector3 b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (!ra.Equals(rb)) parent[ra] = rb;
        }

        foreach (var (a, b) in edges)
            Union(a, b);

        return [.. parent.Keys
            .GroupBy(Find)
            .Select(g => g.Distinct().Count())];
    }
}