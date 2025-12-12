using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day12(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 12;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var presents = lines.TakeWhile(x => !x.Contains('x'))
            .Where(x => !x.Contains(':'))
            .Chunk(3)
            .Select(x => x.ToList())
            .ToList();
        var spaces = lines.SkipWhile(x => !x.Contains('x'))
            .Select(x => x.Split(":", StringSplitOptions.RemoveEmptyEntries))
            .Select(x => (Size: x[0].Trim(), Presents: x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList()))
            .ToList();

        var presentSizes = presents.Select(x => x.Sum(y => y.Count(c => c == '#'))).ToList();

        int fitCount = 0;
        foreach (var (Size, Presents) in spaces)
        {
            var totalSize = Presents.Select((p, i) => p * presentSizes[i]).Sum();
            if (totalSize <= Size.Split('x').Select(int.Parse).Aggregate((a, b) => a * b))
            {
                fitCount++;
            }
        }

        return fitCount.ToString();
    }
}