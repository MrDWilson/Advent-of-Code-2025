using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public abstract class SolutionBase(IFileLoader fileLoader, IOptions<SolutionOptions> options) : ISolution
{
    protected SolutionOptions CurrentOptions => options.Value;

    protected Task<T> LoadRaw<T>()
    {
        var (day, solutionType, runType) = ResolveContext();
        return fileLoader.LoadRaw<T>(day, solutionType, runType);
    }

    protected Task<List<T>> LoadLines<T>()
    {
        var (day, solutionType, runType) = ResolveContext();
        return fileLoader.LoadLines<T>(day, solutionType, runType);
    }

    protected Task<Grid<T>> LoadGrid<T>()
    {
        var (day, solutionType, runType) = ResolveContext();
        return fileLoader.LoadGrid<T>(day, solutionType, runType);
    }

    protected Task<List<List<T>>> LoadItems<T>()
    {
        var (day, solutionType, runType) = ResolveContext();
        return fileLoader.LoadItems<T>(day, solutionType, runType);
    }

    private (int Day, SolutionType SolutionType, RunType RunType) ResolveContext()
    {
        var options = CurrentOptions;
        return (options.Day, options.SolutionType, options.RunType);
    }

    public abstract int Day { get; }
    public abstract Task<string> Solve();
}