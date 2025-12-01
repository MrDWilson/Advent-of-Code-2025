from __future__ import annotations

import json
from pathlib import Path

from models import SOLUTION_CONFIG_KEY, SolutionOptions
from services import FileLoader, SolutionRunner
from solutions import ISolution
from solutions.day01.day01 import Day01
from solutions.day02.day02 import Day02
from solutions.day03.day03 import Day03
from solutions.day04.day04 import Day04
from solutions.day05.day05 import Day05
from solutions.day06.day06 import Day06
from solutions.day07.day07 import Day07
from solutions.day08.day08 import Day08
from solutions.day09.day09 import Day09
from solutions.day10.day10 import Day10
from solutions.day11.day11 import Day11
from solutions.day12.day12 import Day12
from solutions.day100.example import Example


def strip_json_comments(content: str) -> str:
    result = []
    in_string = False
    escape = False
    i = 0

    while i < len(content):
        char = content[i]
        next_char = content[i + 1] if i + 1 < len(content) else ""

        if escape:
            result.append(char)
            escape = False
            i += 1
            continue

        if char == "\\" and in_string:
            result.append(char)
            escape = True
            i += 1
            continue

        if char in {'"', "'"}:
            in_string = not in_string
            result.append(char)
            i += 1
            continue

        if not in_string and char == "/" and next_char == "/":
            i = content.find("\n", i)
            if i == -1:
                break
            continue

        if not in_string and char == "/" and next_char == "*":
            end_index = content.find("*/", i + 2)
            i = len(content) if end_index == -1 else end_index + 2
            continue

        result.append(char)
        i += 1

    return "".join(result)


def load_solution_options() -> SolutionOptions:
    root = Path(__file__).resolve().parent.parent
    config_path = root / "appsettings.json"
    raw = config_path.read_text(encoding="utf-8")
    parsed = json.loads(strip_json_comments(raw))
    section = parsed.get(SOLUTION_CONFIG_KEY, {})
    return SolutionOptions.from_dict(section)


def build_solutions(file_loader: FileLoader, options: SolutionOptions) -> list[ISolution]:
    return [
        Day01(file_loader, options),
        Day02(file_loader, options),
        Day03(file_loader, options),
        Day04(file_loader, options),
        Day05(file_loader, options),
        Day06(file_loader, options),
        Day07(file_loader, options),
        Day08(file_loader, options),
        Day09(file_loader, options),
        Day10(file_loader, options),
        Day11(file_loader, options),
        Day12(file_loader, options),
        Example(file_loader, options),
    ]


def main() -> None:
    options = load_solution_options()
    file_loader = FileLoader()
    solutions = build_solutions(file_loader, options)
    runner = SolutionRunner(solutions, options)
    runner.run()


if __name__ == "__main__":
    main()

