const SOLUTION_CONFIG_KEY = "Solution" as const;

enum SolutionType {
  First = "First",
  Second = "Second",
}

enum RunType {
  Test = "Test",
  Full = "Full",
}

type SolutionOptionsInit = {
  day?: number | string;
  Day?: number | string;
  solutionType?: SolutionType | keyof typeof SolutionType | string;
  SolutionType?: SolutionType | keyof typeof SolutionType | string;
  runType?: RunType | keyof typeof RunType | string;
  RunType?: RunType | keyof typeof RunType | string;
};

class SolutionOptions {
  public day = 1;
  public solutionType: SolutionType = SolutionType.First;
  public runType: RunType = RunType.Test;

  public constructor(init?: SolutionOptionsInit) {
    if (init) {
      this.apply(init);
    }
  }

  public apply(init: SolutionOptionsInit): void {
    const desiredDay = init.day ?? init.Day;
    if (desiredDay !== undefined) {
      const parsed = Number(desiredDay);
      this.day = Number.isNaN(parsed) ? this.day : parsed;
    }

    const desiredSolutionType = init.solutionType ?? init.SolutionType;
    if (desiredSolutionType !== undefined) {
      this.solutionType = this.parseSolutionType(desiredSolutionType);
    }

    const desiredRunType = init.runType ?? init.RunType;
    if (desiredRunType !== undefined) {
      this.runType = this.parseRunType(desiredRunType);
    }
  }

  private parseSolutionType(value: SolutionOptionsInit["solutionType"]): SolutionType {
    if (typeof value === "string") {
      const normalized = value.trim();
      if (normalized in SolutionType) {
        return SolutionType[normalized as keyof typeof SolutionType];
      }

      const match = (Object.values(SolutionType) as string[]).find(
        (entry) => entry.toLowerCase() === normalized.toLowerCase(),
      );

      if (match) {
        return match as SolutionType;
      }
    } else if (typeof value === "number") {
      const enumValues = Object.values(SolutionType);
      return (enumValues[value] as SolutionType) ?? this.solutionType;
    } else if (value) {
      return value;
    }

    return this.solutionType;
  }

  private parseRunType(value: SolutionOptionsInit["runType"]): RunType {
    if (typeof value === "string") {
      const normalized = value.trim();
      if (normalized in RunType) {
        return RunType[normalized as keyof typeof RunType];
      }

      const match = (Object.values(RunType) as string[]).find(
        (entry) => entry.toLowerCase() === normalized.toLowerCase(),
      );

      if (match) {
        return match as RunType;
      }
    } else if (typeof value === "number") {
      const enumValues = Object.values(RunType);
      return (enumValues[value] as RunType) ?? this.runType;
    } else if (value) {
      return value;
    }

    return this.runType;
  }
}

export { RunType, SolutionOptions, SolutionType, SOLUTION_CONFIG_KEY };
