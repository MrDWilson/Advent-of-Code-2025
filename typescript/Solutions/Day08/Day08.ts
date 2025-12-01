import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day08 extends SolutionBase {
  public readonly day = 8;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    return "Hello, World!";
  }
}

export { Day08 };