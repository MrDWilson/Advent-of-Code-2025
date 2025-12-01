from __future__ import annotations

from abc import ABC, abstractmethod
from typing import Callable, TypeVar

from models import Grid, RunType, SolutionOptions, SolutionType
from services import IFileLoader

from .isolution import ISolution

T = TypeVar("T")


class SolutionBase(ISolution, ABC):
    def __init__(self, file_loader: IFileLoader, options: SolutionOptions) -> None:
        self._file_loader = file_loader
        self._options = options

    @property
    def current_options(self) -> SolutionOptions:
        return self._options

    def load_raw(self) -> str:
        return self._file_loader.load_raw(
            self._options.day, self._options.solution_type, self._options.run_type
        )

    def load_lines(self, converter: Callable[[str], T] | None = None) -> list[T]:
        return self._file_loader.load_lines(
            self._options.day,
            self._options.solution_type,
            self._options.run_type,
            converter,
        )

    def load_grid(self, converter: Callable[[str], T] | None = None) -> Grid[T]:
        return self._file_loader.load_grid(
            self._options.day,
            self._options.solution_type,
            self._options.run_type,
            converter,
        )

    def load_items(self, converter: Callable[[str], T] | None = None) -> list[list[T]]:
        return self._file_loader.load_items(
            self._options.day,
            self._options.solution_type,
            self._options.run_type,
            converter,
        )

    @property
    @abstractmethod
    def day(self) -> int:
        ...

    @abstractmethod
    def solve(self) -> str:
        ...

