from __future__ import annotations

from collections.abc import Iterable
from typing import Any


def _flatten(values: Iterable[Any], separator: str) -> str:
    return separator.join(str(value) for value in values)


def write_line(values: Iterable[Any]) -> None:
    collected = list(values)
    if not collected:
        print("Empty")
        return

    if all(isinstance(item, str) for item in collected):
        print(_flatten(collected, "\n"))
        return

    print(_flatten(collected, ", "))


def write_nested(values: Iterable[Iterable[Any]]) -> None:
    rows = [_flatten(row, ", ") for row in values]
    write_line(rows)


__all__ = ["write_line", "write_nested"]

