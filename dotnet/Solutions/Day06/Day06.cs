using System.Text.RegularExpressions;
using AdventOfCode.Helpers;
using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day06(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 6;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var starts = Regex.Matches(lines[^1], @"\S")
            .Cast<Match>()
            .Select(m => m.Index)
            .ToList();

        var formatted = lines
        .Select(l => starts
            .Select((s, i) =>
            {
                var slice = l[s..(i + 1 < starts.Count ? starts[i + 1] : l.Length)];
                return i + 1 < starts.Count && slice.EndsWith(' ')
                    ? slice[..^1]
                    : slice;
            })
            .ToList())
        .ToList();

        long total = 0;
        var range = CurrentOptions.SolutionType is SolutionType.First
            ? Enumerable.Range(0, formatted.First().Count)
            : Enumerable.Range(0, formatted.First().Count).Reverse();
        foreach (var column in range)
        {
            var columnValues = formatted.Select(line => line[column]).ToList();
            var op = columnValues[^1];
            List<long> values = [];
            if (CurrentOptions.SolutionType is SolutionType.First)
            {
                values = [.. columnValues[..^1].Select(x => long.Parse(x.Trim()))];
            }
            else
            {
                var valueStrings = columnValues[..^1];
                foreach (var col in Enumerable.Range(0, valueStrings.Select(x => x.Length).Max()).Reverse())
                {
                    var colValues = valueStrings
                        .Select(x => x[col]).Where(x => x != ' ').ToList();
                    values.Add(long.Parse(string.Join("", colValues)));
                }
            }

            total += op.Trim() switch {
                "+" => values.Sum(),
                "*" => values.Aggregate((x, y) => x * y),
                _ => throw new ArgumentException(nameof(op))
            };
        }
        
        return total.ToString();
    }
}