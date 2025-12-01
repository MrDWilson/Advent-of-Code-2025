import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day03 extends SolutionBase {
  public readonly day = 3;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day03 };