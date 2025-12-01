import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day10 extends SolutionBase {
  public readonly day = 10;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day10 };