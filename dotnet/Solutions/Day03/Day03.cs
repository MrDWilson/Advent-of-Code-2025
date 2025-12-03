using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;

namespace AdventOfCode.Solutions;

public class Day03(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 3;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var banks = lines.Select(line => line.Select(char.GetNumericValue).Where(x => x >= 0).ToList()).ToList();
        var joltage = CurrentOptions.SolutionType is SolutionType.First 
            ? banks.Select(GetJoltage).Sum() 
            : banks.Select(GetFullJoltage).Sum();
        return joltage.ToString();
    }

    private static long GetJoltage(List<double> bank)
    {
        var bankValues = bank.OrderDescending().Distinct().ToList();
        var biggestBatteryIndex = bank.IndexOf(bankValues.First());
 
        if (biggestBatteryIndex == bank.Count - 1)
            biggestBatteryIndex = bank.IndexOf(bankValues.Skip(1).First());

        var secondBattery = bank.Skip(biggestBatteryIndex + 1).Max();
        var biggestBattery = bank[biggestBatteryIndex];

        return long.Parse(biggestBattery.ToString() + secondBattery.ToString());
    }

    private static long GetFullJoltage(List<double> bank)
    {
        var batteries = new List<double>();
        var batteryAim = 12;
        var lastBatteryIndex = -1;
        while (batteries.Count < batteryAim)
        {
            var potentialBatteries = bank.Skip(lastBatteryIndex + 1).ToList();
            var batteriesNeeded = batteryAim - batteries.Count;
            var availableBatteryCount = potentialBatteries.Count - batteriesNeeded;
            var availableBatteries = potentialBatteries.Take(availableBatteryCount + 1).ToList();
            var indexOfBiggestBattery = availableBatteries.IndexOf(availableBatteries.Max());
            var biggestBattery = availableBatteries[indexOfBiggestBattery];
            batteries.Add(biggestBattery);
            lastBatteryIndex = lastBatteryIndex + 1 + indexOfBiggestBattery;
        }

        var result = long.Parse(batteries.Select(x => x.ToString()).Aggregate((a, b) => a + b));
        return result;
    }
}