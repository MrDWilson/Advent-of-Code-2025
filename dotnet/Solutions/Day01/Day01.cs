using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day01(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 1;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var rotations = lines.Select(line => (Direction: line.First(), Count: int.Parse(new string([.. line.Skip(1)]))));

        var countOfZeros = 0;
        var location = 50;
        foreach (var rotation in rotations)
        {
            location = rotation.Direction switch {
                'R' => TurnRight(rotation.Count, location),
                'L' => TurnLeft(rotation.Count, location),
                _ => throw new ArgumentException(nameof(rotation.Direction))
            };

            if (location is 0)
                countOfZeros++;
        }

        return countOfZeros.ToString();
    }

    int TurnLeft(int turn, int location) => turn is 0 ? location : TurnLeft(--turn, location is 0 ? 99 : --location);
    int TurnRight(int turn, int location) => turn is 0 ? location : TurnRight(--turn, location is 99 ? 0 : ++location);
}