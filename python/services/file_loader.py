from __future__ import annotations

from pathlib import Path
from typing import Callable, Iterable, List, Protocol, Sequence, TypeVar

from models import Grid, RunType, SolutionType

T = TypeVar("T")


class IFileLoader(Protocol):
    def load_raw(self, day: int, solution_type: SolutionType, run_type: RunType) -> str: ...

    def load_lines(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> list[T]: ...

    def load_grid(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> Grid[T]: ...

    def load_items(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> list[list[T]]: ...


class FileLoader(IFileLoader):
    def __init__(self, solutions_root: Path | None = None) -> None:
        if solutions_root:
            root = solutions_root
        else:
            candidates = [
                Path(__file__).resolve().parent.parent / "data",
                Path(__file__).resolve().parent.parent / "Solutions",
                Path(__file__).resolve().parent.parent / "solutions",
                Path(__file__).resolve().parent.parent.parent / "dotnet" / "Solutions",
                Path(__file__).resolve().parent.parent.parent
                / "typescript"
                / "Solutions",
            ]
            root = next((candidate for candidate in candidates if candidate.exists()), candidates[0])
        self._solutions_root = root

    def load_raw(self, day: int, solution_type: SolutionType, run_type: RunType) -> str:
        file_path = self._resolve_data_file_path(day, solution_type, run_type)
        return file_path.read_text(encoding="utf-8")

    def load_lines(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> list[T]:
        converter = converter or self._default_converter
        raw = self.load_raw(day, solution_type, run_type)
        lines = [line.rstrip("\n") for line in raw.splitlines() if line.strip()]
        return [converter(line) for line in lines]

    def load_grid(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> Grid[T]:
        converter = converter or self._default_converter
        lines = self.load_lines(day, solution_type, run_type, lambda value: value)
        rows: list[list[T]] = []
        for line in lines:
            row = [converter(char) for char in line if not char.isspace()]
            rows.append(row)
        return Grid(rows)

    def load_items(
        self,
        day: int,
        solution_type: SolutionType,
        run_type: RunType,
        converter: Callable[[str], T] | None = None,
    ) -> list[list[T]]:
        converter = converter or self._default_converter
        raw_lines = self.load_lines(day, solution_type, run_type, lambda value: value)
        result: list[list[T]] = []
        for line in raw_lines:
            entries = [converter(token) for token in line.split() if token]
            if entries:
                result.append(entries)
        return result

    def _resolve_data_file_path(
        self, day: int, solution_type: SolutionType, run_type: RunType
    ) -> Path:
        day_folder = f"Day{day:02d}"
        file_name = self._build_file_name(solution_type, run_type)
        return self._solutions_root / day_folder / "Data" / file_name

    @staticmethod
    def _build_file_name(solution_type: SolutionType, run_type: RunType) -> str:
        part_suffix = "1" if solution_type == SolutionType.FIRST else "2"
        prefix = "Test" if run_type == RunType.TEST else "Full"
        return f"{prefix}{part_suffix}.txt"

    @staticmethod
    def _default_converter(value: str) -> T:
        stripped = value.strip()
        if not stripped:
            return stripped  # type: ignore[return-value]
        if stripped.isdigit() or (stripped.startswith("-") and stripped[1:].isdigit()):
            return int(stripped)  # type: ignore[return-value]
        try:
            return float(stripped)  # type: ignore[return-value]
        except ValueError:
            pass
        lowered = stripped.lower()
        if lowered in {"true", "false"}:
            return (lowered == "true")  # type: ignore[return-value]
        return stripped  # type: ignore[return-value]

