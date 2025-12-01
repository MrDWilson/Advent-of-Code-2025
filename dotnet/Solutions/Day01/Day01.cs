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

        var countOfZerosPartOne = 0;
        var countOfZerosPartTwo = 0;
        var location = 50;
        foreach (var rotation in rotations)
        {
            (location, int zeroCount) = rotation.Direction switch {
                'R' => TurnRight(rotation.Count, location),
                'L' => TurnLeft(rotation.Count, location),
                _ => throw new ArgumentException(nameof(rotation.Direction))
            };

            if (location is 0)
                countOfZerosPartOne++;

            countOfZerosPartTwo += zeroCount;
        }

        return CurrentOptions.SolutionType is SolutionType.First ? countOfZerosPartOne.ToString() : countOfZerosPartTwo.ToString();
    }

    static (int Location, int ZeroCount) TurnLeft(int turn, int location, int countZeros = 0) 
    {
        if (location is 0 && turn is not 0)
            countZeros++;

        return turn is 0 ? (location, countZeros) : TurnLeft(--turn, location is 0 ? 99 : --location, countZeros);
    }

    static (int Location, int ZeroCount) TurnRight(int turn, int location, int countZeros = 0) 
    {
        if (location is 0 && turn is not 0)
            countZeros++;

        return turn is 0 ? (location, countZeros) : TurnRight(--turn, location is 99 ? 0 : ++location, countZeros);
    }
}