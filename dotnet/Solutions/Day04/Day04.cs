using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day04(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 4;

    public override async Task<string> Solve()
    {
        var grid = await LoadGrid<char>();

        if (CurrentOptions.SolutionType is SolutionType.First)
        {
            var pointCount = grid.FindItems('@')
                .Select(grid.GetSurroundingItemsDiagonally)
                .Where(points => points.Select(point => grid[point]).Where(x => x == '@').Count() < 4)
                .Count();
            return pointCount.ToString();
        }
        else
        {
            var pointsRemoved = 0;
            var totalPointsRemoved = 0;
            do
            {
                var pointCount = grid.FindItems('@')
                    .Select(point => (point, grid.GetSurroundingItemsDiagonally(point)))
                    .Where(points => points.Item2.Select(point => grid[point]).Where(x => x == '@').Count() < 4)
                    .ToList();

                pointsRemoved = pointCount.Count;
                totalPointsRemoved += pointsRemoved;

                foreach (var point in pointCount)
                {
                    grid[point.point] = '.';
                }
            } while (pointsRemoved > 0);

            return totalPointsRemoved.ToString();
        }
    }
}