import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day07 extends SolutionBase {
  public readonly day = 7;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day07 };