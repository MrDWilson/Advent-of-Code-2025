import { SolutionType } from "../../Models/SolutionModels.ts";
import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Example extends SolutionBase {
  public readonly day = 100;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    const lines = await this.loadItems<number>();
    const listOne = lines.map((parts) => parts[0]!);
    const listTwo = lines.map((parts) => parts[1]!);

    const result =
      this.options.solutionType === SolutionType.First
        ? Example.calculateDistance(listOne, listTwo)
        : Example.calculateSimilarity(listOne, listTwo);

    return result.toString();
  }

  private static calculateDistance(listOne: number[], listTwo: number[]): number {
    const sortedOne = [...listOne].sort((a, b) => a - b);
    const sortedTwo = [...listTwo].sort((a, b) => a - b);

    let total = 0;
    for (let i = 0; i < Math.min(sortedOne.length, sortedTwo.length); i += 1) {
      total += Math.abs(sortedOne[i]! - sortedTwo[i]!);
    }

    return total;
  }

  private static calculateSimilarity(listOne: number[], listTwo: number[]): number {
    const frequency = new Map<number, number>();
    for (const value of listTwo) {
      frequency.set(value, (frequency.get(value) ?? 0) + 1);
    }

    return listOne.reduce((acc, value) => acc + value * (frequency.get(value) ?? 0), 0);
  }
}

export { Example };