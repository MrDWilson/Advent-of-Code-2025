import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day12 extends SolutionBase {
  public readonly day = 12;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day12 };