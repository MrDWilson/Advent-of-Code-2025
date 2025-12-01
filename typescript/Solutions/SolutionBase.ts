import type { ISolution } from "./ISolution.ts";
import type { IFileLoader, Matrix } from "../Services/FileLoader.ts";
import type { SolutionOptions } from "../Models/SolutionModels.ts";
import type { Grid } from "../Models/Grid.ts";

type SolutionDependencies = {
  fileLoader: IFileLoader;
  options: SolutionOptions;
};

abstract class SolutionBase implements ISolution {
  protected constructor(private readonly dependencies: SolutionDependencies) {}

  protected get options(): SolutionOptions {
    return this.dependencies.options;
  }

  protected loadRaw(): Promise<string> {
    const { day, solutionType, runType } = this.options;
    return this.dependencies.fileLoader.loadRaw(day, solutionType, runType);
  }

  protected loadLines<T = string>(): Promise<T[]> {
    const { day, solutionType, runType } = this.options;
    return this.dependencies.fileLoader.loadLines<T>(day, solutionType, runType);
  }

  protected loadGrid<T = string>(): Promise<Grid<T>> {
    const { day, solutionType, runType } = this.options;
    return this.dependencies.fileLoader.loadGrid<T>(day, solutionType, runType);
  }

  protected loadItems<T = string>(): Promise<Matrix<T>> {
    const { day, solutionType, runType } = this.options;
    return this.dependencies.fileLoader.loadItems<T>(day, solutionType, runType);
  }

  public abstract readonly day: number;
  public abstract solve(): Promise<string>;
}

export type { SolutionDependencies };
export { SolutionBase };

