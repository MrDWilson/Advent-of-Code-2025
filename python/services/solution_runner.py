from __future__ import annotations

from typing import Iterable

from models import SolutionOptions, SolutionType
from solutions import ISolution


class SolutionRunner:
    def __init__(self, solutions: Iterable[ISolution], options: SolutionOptions) -> None:
        self._solutions = list(solutions)
        self._options = options

    def run(self) -> None:
        solution = next((s for s in self._solutions if s.day == self._options.day), None)
        if solution is None:
            print(f"Solution for day {self._options.day} not found.")
            return

        print(
            f"Day {self._options.day}, {self._options.solution_type.value} part, {self._options.run_type.value} run"
        )
        print(f"https://adventofcode.com/2025/day/{self._options.day}")
        print(f"Solution: {solution.solve()}")

    def run_all(self) -> None:
        for solution in sorted(self._solutions, key=lambda s: s.day):
            self._options.solution_type = SolutionType.FIRST
            print(f"Day: {solution.day}, Solution 1: {solution.solve()}")

            self._options.solution_type = SolutionType.SECOND
            print(f"Day: {solution.day}, Solution 2: {solution.solve()}")

