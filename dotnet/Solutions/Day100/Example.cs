using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Example(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 100;

    public override async Task<string> Solve()
    {
        var lines = await LoadItems<int>();
        var listOne = lines.Select(parts => parts.First()).ToList();
        var listTwo = lines.Select(parts => parts.Last()).ToList();
        var result = CurrentOptions.SolutionType switch
        {
            SolutionType.First => listOne.Order().Zip(listTwo.Order(), (x, y) => Math.Abs(x - y)).Sum(),
            SolutionType.Second => listOne.Sum(x => x * listTwo.Count(y => y == x)),
            _ => throw new ArgumentOutOfRangeException(nameof(CurrentOptions.SolutionType))
        };

        return result.ToString();
    }
}