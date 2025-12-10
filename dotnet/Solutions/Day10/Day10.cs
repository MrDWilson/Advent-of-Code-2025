using AdventOfCode.Models;
using AdventOfCode.Services;
using Microsoft.Extensions.Options;
using Microsoft.Z3;

namespace AdventOfCode.Solutions;

public class Day10(IFileLoader fileLoader, IOptions<SolutionOptions> options) : SolutionBase(fileLoader, options)
{
    public override int Day => 10;

    public override async Task<string> Solve()
    {
        var lines = await LoadLines<string>();
        var lightLines = lines.Select(x => new string([.. x.TakeWhile(y => y != ' ')]).Trim()).ToList();
        var buttonLines = lines.Select(x => new string([.. x.SkipWhile(y => y != '(').TakeWhile(y => y != '{')]).Trim()).ToList();
        var joltageLines = lines.Select(x => new string([.. x.SkipWhile(y => y != '{')]).Trim()).ToList();

        var lights = lightLines.Select(x => new LightBoard(x)).ToList();
        var buttons = buttonLines.Select(x => x.Split(' ').Select(y => new Button([.. y.Trim('(', ')', ' ').Split(',').Select(int.Parse)])).ToList()).ToList();
        var joltage = joltageLines.Select(x => x.Trim('{', '}', ' ').Split(',').Select(int.Parse).ToList()).ToList();

        var zipped = lights.Zip(buttons.Zip(joltage, (buttons, joltage) => (buttons, joltage)), (light, button) => (Lights: light, Buttons: button.buttons, Joltage: button.joltage));

        long total = 0;
        foreach (var (lightBoard, buttonList, joltageTargets) in zipped)
        {
            total += CurrentOptions.SolutionType is SolutionType.First
                ? SolveLightsWithZ3(lightBoard, buttonList)
                : SolveJoltageWithZ3(joltageTargets, buttonList);
        }

        return total.ToString();
    }

    private static long SolveLightsWithZ3(LightBoard lightBoard, List<Button> buttons)
    {
        var orderedTargets = lightBoard.DesiredLights
            .OrderBy(x => x.Key)
            .Select(x => x.Value ? 1 : 0)
            .ToArray();

        return SolveWithZ3(orderedTargets.Length, buttons, orderedTargets, useParity: true);
    }

    private static long SolveJoltageWithZ3(List<int> joltageTargets, List<Button> buttons)
    {
        return SolveWithZ3(joltageTargets.Count, buttons, joltageTargets, useParity: false);
    }

    private static long SolveWithZ3(int dimension, List<Button> buttons, IReadOnlyList<int> targets, bool useParity)
    {
        if (dimension == 0)
        {
            return targets.All(t => t == 0) ? 0 : -1;
        }

        using var context = new Context();
        var optimiser = context.MkOptimize();

        var buttonVariables = buttons
            .Select((_, index) => context.MkIntConst($"x_{index}"))
            .ToArray();

        foreach (var variable in buttonVariables)
        {
            optimiser.Add(context.MkGe(variable, context.MkInt(0)));
        }

        var modulus = context.MkInt(2);
        for (var position = 0; position < dimension; position++)
        {
            ArithExpr sum = context.MkInt(0);
            var hasTerm = false;

            for (var buttonIndex = 0; buttonIndex < buttons.Count; buttonIndex++)
            {
                var coefficient = buttons[buttonIndex].Positions.Count(slot => slot == position);
                if (coefficient == 0) continue;

                var term = coefficient == 1
                    ? buttonVariables[buttonIndex]
                    : context.MkMul(context.MkInt(coefficient), buttonVariables[buttonIndex]);

                sum = hasTerm ? context.MkAdd(sum, term) : term;
                hasTerm = true;
            }

            if (!hasTerm)
            {
                sum = context.MkInt(0);
            }

            var targetValue = context.MkInt(targets[position]);

            if (useParity)
            {
                optimiser.Add(context.MkEq(context.MkMod((IntExpr)sum, modulus), targetValue));
            }
            else
            {
                optimiser.Add(context.MkEq(sum, targetValue));
            }
        }

        ArithExpr totalPresses = buttonVariables.Length switch
        {
            0 => context.MkInt(0),
            1 => buttonVariables[0],
            _ => context.MkAdd(buttonVariables)
        };

        optimiser.MkMinimize(totalPresses);
        var status = optimiser.Check();
        if (status != Status.SATISFIABLE)
        {
            return -1;
        }

        var model = optimiser.Model;
        if (model is null)
        {
            return -1;
        }

        var evaluated = model.Evaluate(totalPresses);
        return evaluated switch
        {
            IntNum intNum => intNum.Int64,
            RatNum ratNum when ratNum.IsInt => (long)ratNum.BigIntNumerator,
            _ => long.Parse(evaluated.ToString())
        };
    }

    private class LightBoard(string line)
    {
        public Dictionary<int, bool> DesiredLights { get; } = line.Trim('[', ']')
                .Select((value, index) => (value, index))
                .ToDictionary(x => x.index, x => x.value == '#');
    }

    private record Button(List<int> Positions);
}