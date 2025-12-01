from __future__ import annotations

from abc import ABC, abstractmethod


class ISolution(ABC):
    @property
    @abstractmethod
    def day(self) -> int:
        ...

    @abstractmethod
    def solve(self) -> str:
        ...

