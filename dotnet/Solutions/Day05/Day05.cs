using AdventOfCode.Helpers;
using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day05(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 5;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var ranges = lines.TakeWhile(x => x.Contains('-'))
            .Select(x => x.Split('-'))
            .Select(x => (Start: long.Parse(x[0]), End: long.Parse(x[1])))
            .ToHashSet();
        var numbers = lines.SkipWhile(x => x.Contains('-')).Select(long.Parse).ToList();

        if (CurrentOptions.SolutionType is SolutionType.First)
        {
            var result = numbers.Count(number => ranges.Any(range => InRange(number, range)));
            return result.ToString();
        }
        else
        {
            return CollapseRanges(ranges).Select(GetRangeCount).Sum().ToString();
        }
    }

    private static bool InRange(long number, (long Start, long End) range) => number >= range.Start && number <= range.End;

    private static long GetRangeCount((long Start, long End) range) => range.End - range.Start + 1;

    private static IEnumerable<(long Start, long End)> CollapseRanges(IEnumerable<(long Start, long End)> ranges)
    {
        return ranges
            .OrderBy(r => r.Start)
            .Aggregate(new List<(long Start, long End)>(), (acc, r) =>
            {
                if (acc.Count == 0 || r.Start > acc[^1].End + 1)
                    acc.Add(r);
                else
                    acc[^1] = (acc[^1].Start, Math.Max(acc[^1].End, r.End));
                return acc;
            });
    }
}