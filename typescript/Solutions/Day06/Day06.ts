import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day06 extends SolutionBase {
  public readonly day = 6;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day06 };