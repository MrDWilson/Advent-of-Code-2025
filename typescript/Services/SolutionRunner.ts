import type { ISolution } from "../Solutions/ISolution.ts";
import { SolutionOptions, SolutionType } from "../Models/SolutionModels.ts";

interface ISolutionRunner {
  run(): Promise<void>;
  runAll(): Promise<void>;
}

class SolutionRunner implements ISolutionRunner {
  public constructor(
    private readonly solutions: ISolution[],
    private readonly options: SolutionOptions,
  ) {}

  public async run(): Promise<void> {
    const solution = this.solutions.find((entry) => entry.day === this.options.day);

    if (!solution) {
      console.log(`Solution for day ${this.options.day} not found.`);
      return;
    }

    console.log(
      `Day ${this.options.day}, ${this.options.solutionType} part, ${this.options.runType} run`,
    );
    console.log(`https://adventofcode.com/2025/day/${this.options.day}`);
    console.log(`Solution: ${await solution.solve()}`);
  }

  public async runAll(): Promise<void> {
    const solutionsByDay = [...this.solutions].sort((a, b) => a.day - b.day);

    for (const solution of solutionsByDay) {
      this.options.solutionType = SolutionType.First;
      console.log(`Day: ${solution.day}, Solution 1: ${await solution.solve()}`);

      this.options.solutionType = SolutionType.Second;
      console.log(`Day: ${solution.day}, Solution 2: ${await solution.solve()}`);
    }
  }
}

export type { ISolutionRunner };
export { SolutionRunner };

