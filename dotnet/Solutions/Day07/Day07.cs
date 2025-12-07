using System.Drawing;
using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day07(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 7;

    private readonly HashSet<Point> _visitedPart1 = [];
    private readonly Dictionary<Point, int> _part1Memo = [];
    private readonly Dictionary<Point, long> _timelineMemo = [];
    private readonly HashSet<Point> _timelineVisiting = [];

    public override async Task<string> Solve()
    {
        var grid = await LoadGrid<char>();
        var start = grid.FindItems('S').First();
        var entryPoint = new Point(start.X + 1, start.Y);

        if (CurrentOptions.SolutionType is SolutionType.First)
        {
            return CountPart1(grid, entryPoint).ToString();
        }

        return CountTimelines(grid, entryPoint).ToString();
    }

    private int CountPart1(Grid<char> grid, Point start)
    {
        if (_visitedPart1.Contains(start))
        {
            return 0;
        }

        if (_part1Memo.TryGetValue(start, out var cached))
        {
            return cached;
        }

        var current = start;

        while (true)
        {
            if (grid.OutOfBounds(current))
            {
                _part1Memo[start] = 0;
                return 0;
            }

            if (!_visitedPart1.Add(current))
            {
                _part1Memo[start] = 0;
                return 0;
            }

            var item = grid[current];

            if (item == '.')
            {
                current = new Point(current.X + 1, current.Y);
                continue;
            }

            if (item != '^')
            {
                _part1Memo[start] = 0;
                return 0;
            }

            var left = new Point(current.X, current.Y - 1);
            var right = new Point(current.X, current.Y + 1);

            var total = 1;

            if (!grid.OutOfBounds(left))
            {
                total += CountPart1(grid, left);
            }

            if (!grid.OutOfBounds(right))
            {
                total += CountPart1(grid, right);
            }

            _part1Memo[start] = total;
            return total;
        }
    }

    private long CountTimelines(Grid<char> grid, Point start)
    {
        if (_timelineMemo.TryGetValue(start, out var cached))
        {
            return cached;
        }

        if (!_timelineVisiting.Add(start))
        {
            return 0;
        }

        var current = start;

        while (true)
        {
            if (grid.OutOfBounds(current))
            {
                _timelineVisiting.Remove(start);
                _timelineMemo[start] = 1;
                return 1;
            }

            var item = grid[current];

            if (item == '.')
            {
                current = new Point(current.X + 1, current.Y);
                continue;
            }

            if (item != '^')
            {
                _timelineVisiting.Remove(start);
                _timelineMemo[start] = 1;
                return 1;
            }

            var left = new Point(current.X, current.Y - 1);
            var right = new Point(current.X, current.Y + 1);

            long total = 0;

            if (!grid.OutOfBounds(left))
            {
                total += CountTimelines(grid, left);
            }

            if (!grid.OutOfBounds(right))
            {
                total += CountTimelines(grid, right);
            }

            _timelineVisiting.Remove(start);
            _timelineMemo[start] = total;
            return total;
        }
    }
}