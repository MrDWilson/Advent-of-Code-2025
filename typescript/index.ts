import { readFile } from "node:fs/promises";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

import { SOLUTION_CONFIG_KEY, SolutionOptions } from "./Models/SolutionModels.ts";
import { FileLoader } from "./Services/FileLoader.ts";
import { SolutionRunner } from "./Services/SolutionRunner.ts";
import type { ISolution } from "./Solutions/ISolution.ts";
import type { SolutionDependencies } from "./Solutions/SolutionBase.ts";
import { Example } from "./Solutions/Day100/Example.ts";
import { Day01 } from "./Solutions/Day01/Day01.ts";
import { Day02 } from "./Solutions/Day02/Day02.ts";
import { Day03 } from "./Solutions/Day03/Day03.ts";
import { Day04 } from "./Solutions/Day04/Day04.ts";
import { Day05 } from "./Solutions/Day05/Day05.ts";
import { Day06 } from "./Solutions/Day06/Day06.ts";
import { Day07 } from "./Solutions/Day07/Day07.ts";
import { Day08 } from "./Solutions/Day08/Day08.ts";
import { Day09 } from "./Solutions/Day09/Day09.ts";
import { Day10 } from "./Solutions/Day10/Day10.ts";
import { Day11 } from "./Solutions/Day11/Day11.ts";
import { Day12 } from "./Solutions/Day12/Day12.ts";

const __filename = fileURLToPath(import.meta.url);
const __dirname = dirname(__filename);

const stripJsonComments = (input: string): string => {
  let inString = false;
  let stringChar: '"' | "'" | null = null;
  let inSingleLineComment = false;
  let inMultiLineComment = false;
  let output = "";

  for (let i = 0; i < input.length; i += 1) {
    const current = input[i]!;
    const next = input[i + 1];

    if (inSingleLineComment) {
      if (current === "\n") {
        inSingleLineComment = false;
        output += current;
      }
      continue;
    }

    if (inMultiLineComment) {
      if (current === "*" && next === "/") {
        inMultiLineComment = false;
        i += 1;
      }
      continue;
    }

    if (inString) {
      if (current === "\\" && next !== undefined) {
        output += current + next;
        i += 1;
        continue;
      }

      if (current === stringChar) {
        inString = false;
        stringChar = null;
      }

      output += current;
      continue;
    }

    if (current === '"' || current === "'") {
      inString = true;
      stringChar = current;
      output += current;
      continue;
    }

    if (current === "/" && next === "/") {
      inSingleLineComment = true;
      i += 1;
      continue;
    }

    if (current === "/" && next === "*") {
      inMultiLineComment = true;
      i += 1;
      continue;
    }

    output += current;
  }

  return output;
};

async function loadSolutionOptions(): Promise<SolutionOptions> {
  const configPath = join(__dirname, "..", "appsettings.json");
  const raw = await readFile(configPath, { encoding: "utf-8" });
  const sanitized = stripJsonComments(raw);
  const parsed = JSON.parse(sanitized);
  const solutionSection = parsed[SOLUTION_CONFIG_KEY] ?? {};
  return new SolutionOptions(solutionSection);
}

function buildSolutions(dependencies: SolutionDependencies): ISolution[] {
  return [
    new Day01(dependencies),
    new Day02(dependencies),
    new Day03(dependencies),
    new Day04(dependencies),
    new Day05(dependencies),
    new Day06(dependencies),
    new Day07(dependencies),
    new Day08(dependencies),
    new Day09(dependencies),
    new Day10(dependencies),
    new Day11(dependencies),
    new Day12(dependencies),
    new Example(dependencies),
  ];
}

async function main(): Promise<void> {
  const options = await loadSolutionOptions();
  const fileLoader = new FileLoader();
  const dependencies: SolutionDependencies = { fileLoader, options };
  const solutions = buildSolutions(dependencies);
  const runner = new SolutionRunner(solutions, options);

  await runner.run();
}

main().catch((error) => {
  console.error("Failed to execute Advent of Code runner.", error);
  process.exitCode = 1;
});
