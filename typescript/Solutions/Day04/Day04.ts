import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day04 extends SolutionBase {
  public readonly day = 4;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day04 };