using System.Collections.Generic;
using System.Drawing;
using AdventOfCode.Helpers;
using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day09(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 9;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var coords = lines
            .Select(line => line.Split(','))
            .Select(parts => new Point(int.Parse(parts[0]), int.Parse(parts[1])))
            .ToList();

        var coordPairs = coords.UniquePairs().ToList();

        if (CurrentOptions.SolutionType is SolutionType.First)
            return SolvePartOne(coordPairs).ToString();

        return SolvePartTwo(coords, coordPairs).ToString();
    }

    private static long SolvePartOne(IEnumerable<(Point A, Point B)> coordPairs)
    {
        long maxArea = 0;
        foreach (var (a, b) in coordPairs)
        {
            var area = GetRectangleArea(a, b);
            if (area > maxArea)
                maxArea = area;
        }
        return maxArea;
    }

    private static long SolvePartTwo(IReadOnlyList<Point> coords, IEnumerable<(Point A, Point B)> coordPairs)
    {

        var xs = coords.SelectMany(p => new[] { p.X, p.X + 1 }).Distinct().OrderBy(x => x).ToList();
        var ys = coords.SelectMany(p => new[] { p.Y, p.Y + 1 }).Distinct().OrderBy(y => y).ToList();
        var xIndex = xs.Select((value, index) => (value, index)).ToDictionary(x => x.value, x => x.index);
        var yIndex = ys.Select((value, index) => (value, index)).ToDictionary(y => y.value, y => y.index);

        var width = xs.Count - 1;
        var height = ys.Count - 1;
        var filled = new bool[width, height];

        foreach (var (start, end) in coords.Zip(coords.Skip(1).Append(coords.First())))
        {
            var minX = Math.Min(start.X, end.X);
            var maxX = Math.Max(start.X, end.X);
            var minY = Math.Min(start.Y, end.Y);
            var maxY = Math.Max(start.Y, end.Y);

            var xStart = xIndex[minX];
            var xEnd = xIndex[maxX + 1];
            var yStart = yIndex[minY];
            var yEnd = yIndex[maxY + 1];

            for (var x = xStart; x < xEnd; x++)
                for (var y = yStart; y < yEnd; y++)
                    filled[x, y] = true;
        }

        FillInterior(filled);
        var prefix = BuildPrefix(filled, xs, ys);

        long maxArea = 0;
        foreach (var (a, b) in coordPairs)
        {
            var area = GetRectangleArea(a, b);
            if (area <= maxArea) continue;

            if (QueryFilledArea(prefix, a, b, xIndex, yIndex) == area)
                maxArea = area;
        }

        return maxArea;
    }

    private static void FillInterior(bool[,] filled)
    {
        var width = filled.GetLength(0);
        var height = filled.GetLength(1);
        var visited = new bool[width, height];
        var queue = new Queue<(int x, int y)>();

        void Enqueue(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= height) return;
            if (filled[x, y] || visited[x, y]) return;
            visited[x, y] = true;
            queue.Enqueue((x, y));
        }

        for (var x = 0; x < width; x++)
        {
            Enqueue(x, 0);
            Enqueue(x, height - 1);
        }

        for (var y = 0; y < height; y++)
        {
            Enqueue(0, y);
            Enqueue(width - 1, y);
        }

        int[] dx = [-1, 1, 0, 0];
        int[] dy = [0, 0, -1, 1];

        while (queue.Count > 0)
        {
            var (cx, cy) = queue.Dequeue();
            for (var dir = 0; dir < 4; dir++)
                Enqueue(cx + dx[dir], cy + dy[dir]);
        }

        for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                if (!filled[x, y] && !visited[x, y])
                    filled[x, y] = true;
    }

    private static long[,] BuildPrefix(bool[,] filled, IList<int> xs, IList<int> ys)
    {
        var width = filled.GetLength(0);
        var height = filled.GetLength(1);
        var prefix = new long[width + 1, height + 1];

        for (var x = 0; x < width; x++)
        {
            var dx = xs[x + 1] - xs[x];
            for (var y = 0; y < height; y++)
            {
                var dy = ys[y + 1] - ys[y];
                var area = filled[x, y] ? (long)dx * dy : 0;
                prefix[x + 1, y + 1] = area + prefix[x, y + 1] + prefix[x + 1, y] - prefix[x, y];
            }
        }

        return prefix;
    }

    private static long QueryFilledArea(long[,] prefix, Point a, Point b, IReadOnlyDictionary<int, int> xIndex, IReadOnlyDictionary<int, int> yIndex)
    {
        var minX = Math.Min(a.X, b.X);
        var maxX = Math.Max(a.X, b.X) + 1;
        var minY = Math.Min(a.Y, b.Y);
        var maxY = Math.Max(a.Y, b.Y) + 1;

        var x1 = xIndex[minX];
        var x2 = xIndex[maxX];
        var y1 = yIndex[minY];
        var y2 = yIndex[maxY];

        return prefix[x2, y2] - prefix[x1, y2] - prefix[x2, y1] + prefix[x1, y1];
    }

    private static long GetRectangleArea(Point a, Point b)
    {
        var minX = Math.Min(a.X, b.X);
        var maxX = Math.Max(a.X, b.X);
        var minY = Math.Min(a.Y, b.Y);
        var maxY = Math.Max(a.Y, b.Y);
        return ((long)maxX - minX + 1) * ((long)maxY - minY + 1);
    }
}