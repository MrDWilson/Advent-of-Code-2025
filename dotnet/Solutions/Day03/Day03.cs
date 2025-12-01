using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day03(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 3;

    public override async Task<string> Solve()
    {
        return "Hello, World!";
    }
}