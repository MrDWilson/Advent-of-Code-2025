import { SolutionBase, type SolutionDependencies } from "../SolutionBase.ts";

class Day01 extends SolutionBase {
  public readonly day = 1;

  public constructor(dependencies: SolutionDependencies) {
    super(dependencies);
  }

  public async solve(): Promise<string> {
    const lines = await this.loadLines<string>();
    return `Loaded ${lines.length} line(s).`;
  }
}

export { Day01 };