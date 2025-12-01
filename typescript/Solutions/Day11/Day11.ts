import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day11 extends SolutionBase {
  public readonly day = 11;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day11 };