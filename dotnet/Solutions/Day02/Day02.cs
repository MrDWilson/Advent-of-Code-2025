using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day02(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 2;

    public override async Task<string> Solve()
    {
        var content = await LoadRaw<string>();
        var items = content.Split(',').Select(item => item.Split('-')).Select(item => (Start: long.Parse(item[0]), End: long.Parse(item[1]))).ToList();

        HashSet<long> numbers = [];
        foreach (var (start, end) in items)
        {
            foreach (var number in CheckRange(start, end, CurrentOptions.SolutionType is SolutionType.First ? 2 : null))
            {
                numbers.Add(number);
            }
        }

        return numbers.Select(number => long.Parse(number.ToString())).Sum().ToString();
    }

    private static IEnumerable<long> CheckRange(long start, long end, int? matchLimit = null)
    {
        foreach (var number in CreateRange(start, end - start + 1).Where(number => CheckNumber(number, matchLimit)))
        {
            yield return number;
        }
    }

    private static bool CheckNumber(long number, int? matchLimit = null)
    {
        var digits = number.ToString().TrimStart('0');

        foreach (var length in Enumerable.Range(1, digits.Length))
        {
            if (digits.Length % length != 0 || length >= digits.Length) continue;

            var segments = digits.Chunk(length).ToList();
            if ((matchLimit is null || segments.Count == matchLimit) && segments.All(segment => segment.SequenceEqual(segments.First())))
            {
                return true;
            }
        }

        return false;
    }

    private static IEnumerable<long> CreateRange(long start, long count)
    {
        var limit = start + count;

        while (start < limit)
        {
            yield return start;
            start++;
        }
    }
}