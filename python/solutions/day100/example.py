from __future__ import annotations

from models import SolutionType
from solutions import SolutionBase


class Example(SolutionBase):
    @property
    def day(self) -> int:
        return 100

    def solve(self) -> str:
        lines = self.load_items(int)
        list_one = [parts[0] for parts in lines if parts]
        list_two = [parts[1] for parts in lines if len(parts) > 1]

        if self.current_options.solution_type == SolutionType.FIRST:
            result = self._calculate_distance(list_one, list_two)
        else:
            result = self._calculate_similarity(list_one, list_two)

        return str(result)

    @staticmethod
    def _calculate_distance(list_one: list[int], list_two: list[int]) -> int:
        sorted_one = sorted(list_one)
        sorted_two = sorted(list_two)
        total = 0
        for left, right in zip(sorted_one, sorted_two):
            total += abs(left - right)
        return total

    @staticmethod
    def _calculate_similarity(list_one: list[int], list_two: list[int]) -> int:
        frequencies: dict[int, int] = {}
        for value in list_two:
            frequencies[value] = frequencies.get(value, 0) + 1
        return sum(value * frequencies.get(value, 0) for value in list_one)

