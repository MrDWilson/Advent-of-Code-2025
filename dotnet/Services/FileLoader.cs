using AdventOfCode.Models;

namespace AdventOfCode.Services;

public interface IFileLoader
{
    Task<T> LoadRaw<T>(int day, SolutionType solutionType, RunType runType);
    Task<List<T>> LoadLines<T>(int day, SolutionType solutionType, RunType runType);
    Task<Grid<T>> LoadGrid<T>(int day, SolutionType solutionType, RunType runType);
    Task<List<List<T>>> LoadItems<T>(int day, SolutionType solutionType, RunType runType);
}

public class FileLoader(string? solutionsRoot = null) : IFileLoader
{
    private readonly string _solutionsRoot = solutionsRoot ?? Path.Combine(Directory.GetCurrentDirectory(), "Solutions");

    public async Task<T> LoadRaw<T>(int day, SolutionType solutionType, RunType runType)
    {
        var filePath = ResolveDataFilePath(day, solutionType, runType);
        return ChangeType<T>(await File.ReadAllTextAsync(filePath));
    }

    public async Task<List<T>> LoadLines<T>(int day, SolutionType solutionType, RunType runType)
    {
        var raw = await LoadRaw<string>(day, solutionType, runType);
        return [.. raw.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(ChangeType<T>)];
    }

    public async Task<Grid<T>> LoadGrid<T>(int day, SolutionType solutionType, RunType runType)
    {
        var lines = await LoadLines<string>(day, solutionType, runType);
        List<List<T>> gridRows = [.. lines
            .Select(x => x.Select(y => y.ToString())
            .Where(y => !string.IsNullOrWhiteSpace(y))
            .Select(y => ChangeType<T>(y)).ToList())
        ];
        return new Grid<T>(gridRows);
    }

    public async Task<List<List<T>>> LoadItems<T>(int day, SolutionType solutionType, RunType runType)
    {
        var lines = await LoadLines<string>(day, solutionType, runType);
        var lineItems = lines.Select(line => line.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries));
        return [.. lineItems.Select(x => x.Select(y => ChangeType<T>(y)).ToList())];
    }

    private static T ChangeType<T>(object obj)
    {
        return (T)Convert.ChangeType(obj, typeof(T));
    }

    private static string BuildDayFolder(int day) => $"Day{day:D2}";

    private static string BuildFileName(SolutionType solutionType, RunType runType)
    {
        var partSuffix = solutionType == SolutionType.First ? "1" : "2";
        var prefix = runType == RunType.Test ? "Test" : "Full";
        return $"{prefix}{partSuffix}.txt";
    }

    private string ResolveDataFilePath(int day, SolutionType solutionType, RunType runType)
    {
        return Path.Combine(_solutionsRoot, BuildDayFolder(day), "Data", BuildFileName(solutionType, runType));
    }
}