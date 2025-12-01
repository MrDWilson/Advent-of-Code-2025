import { readFile } from "node:fs/promises";
import { join } from "node:path";

import { Grid } from "../Models/Grid.ts";
import { RunType, SolutionType } from "../Models/SolutionModels.ts";

type Matrix<T> = T[][];

interface IFileLoader {
  loadRaw(day: number, solutionType: SolutionType, runType: RunType): Promise<string>;
  loadLines<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<T[]>;
  loadGrid<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<Grid<T>>;
  loadItems<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<Matrix<T>>;
}

class FileLoader implements IFileLoader {
  private readonly solutionsRoot: string;

  public constructor(solutionsRoot: string = join(process.cwd(), "Solutions")) {
    this.solutionsRoot = solutionsRoot;
  }

  public async loadRaw(day: number, solutionType: SolutionType, runType: RunType): Promise<string> {
    const filePath = this.resolveDataFilePath(day, solutionType, runType);
    return readFile(filePath, { encoding: "utf-8" });
  }

  public async loadLines<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<T[]> {
    const raw = await this.loadRaw(day, solutionType, runType);
    return raw
      .split(/\r?\n/)
      .map((line) => line.trimEnd())
      .filter((line) => line.length > 0)
      .map((line) => this.changeType<T>(line));
  }

  public async loadGrid<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<Grid<T>> {
    const lines = await this.loadLines<string>(day, solutionType, runType);
    const grid = lines
      .map((line) => Array.from(line))
      .map((chars) => chars.filter((char) => char.trim().length > 0))
      .map((chars) => chars.map((char) => this.changeType<T>(char)));
    return new Grid(grid);
  }

  public async loadItems<T>(day: number, solutionType: SolutionType, runType: RunType): Promise<Matrix<T>> {
    const lines = await this.loadLines<string>(day, solutionType, runType);
    return lines
      .map((line) => line.split(/[\s\t]+/).filter((value) => value.length > 0))
      .map((entries) => entries.map((entry) => this.changeType<T>(entry)));
  }

  private resolveDataFilePath(day: number, solutionType: SolutionType, runType: RunType): string {
    const dayFolder = `Day${day.toString().padStart(2, "0")}`;
    const fileName = this.resolveFileName(solutionType, runType);
    return join(this.solutionsRoot, dayFolder, "Data", fileName);
  }

  private resolveFileName(solutionType: SolutionType, runType: RunType): string {
    const partIndex = solutionType === SolutionType.First ? "1" : "2";
    const prefix = runType === RunType.Test ? "Test" : "Full";
    return `${prefix}${partIndex}.txt`;
  }

  private changeType<T>(value: string): T {
    const trimmed = value.trim();

    if (/^-?\d+$/.test(trimmed)) {
      const numeric = Number(trimmed);
      if (Number.isSafeInteger(numeric)) {
        return numeric as T;
      }

      return BigInt(trimmed) as T;
    }

    if (/^-?\d+\.\d+$/.test(trimmed)) {
      return Number(trimmed) as T;
    }

    const lowered = trimmed.toLowerCase();
    if (lowered === "true" || lowered === "false") {
      return (lowered === "true") as T;
    }

    return trimmed as T;
  }
}

export type { IFileLoader, Matrix };
export { FileLoader };

