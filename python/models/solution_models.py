from __future__ import annotations

from dataclasses import dataclass
from enum import Enum
from typing import Any, Mapping

SOLUTION_CONFIG_KEY = "Solution"


class SolutionType(str, Enum):
    FIRST = "First"
    SECOND = "Second"


class RunType(str, Enum):
    TEST = "Test"
    FULL = "Full"


@dataclass
class SolutionOptions:
    day: int = 1
    solution_type: SolutionType = SolutionType.FIRST
    run_type: RunType = RunType.TEST

    @staticmethod
    def from_dict(data: Mapping[str, Any]) -> "SolutionOptions":
        options = SolutionOptions()
        options.apply(data)
        return options

    def apply(self, data: Mapping[str, Any]) -> None:
        if "Day" in data or "day" in data:
            day_value = data.get("Day", data.get("day"))
            if isinstance(day_value, str):
                day_value = day_value.strip()
                if day_value.isdigit():
                    self.day = int(day_value)
            elif isinstance(day_value, (int, float)):
                self.day = int(day_value)

        if "SolutionType" in data or "solutionType" in data:
            raw_value = data.get("SolutionType", data.get("solutionType"))
            parsed = self._parse_enum(raw_value, SolutionType, self.solution_type)
            self.solution_type = parsed

        if "RunType" in data or "runType" in data:
            raw_value = data.get("RunType", data.get("runType"))
            parsed = self._parse_enum(raw_value, RunType, self.run_type)
            self.run_type = parsed

    @staticmethod
    def _parse_enum(value: Any, enum_type: type[Enum], fallback: Enum) -> Enum:
        if isinstance(value, enum_type):
            return value
        if isinstance(value, str):
            normalized = value.strip()
            for member in enum_type:
                if member.name.lower() == normalized.lower():
                    return member
                if member.value.lower() == normalized.lower():
                    return member
        return fallback


__all__ = ["SolutionOptions", "SolutionType", "RunType", "SOLUTION_CONFIG_KEY"]

