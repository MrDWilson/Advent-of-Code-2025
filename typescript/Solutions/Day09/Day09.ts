import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day09 extends SolutionBase {
  public readonly day = 9;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day09 };