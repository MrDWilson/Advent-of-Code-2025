from __future__ import annotations

from dataclasses import dataclass
from typing import Generic, Iterable, Iterator, List, Sequence, Tuple, TypeVar

T = TypeVar("T")


@dataclass(frozen=True)
class Point:
    x: int
    y: int


_CARDINAL_OFFSETS: Tuple[Point, ...] = (
    Point(-1, 0),
    Point(1, 0),
    Point(0, -1),
    Point(0, 1),
)


class Grid(Generic[T]):
    def __init__(self, data: Sequence[Sequence[T]]) -> None:
        if not data or not data[0]:
            raise ValueError("Grid must have at least one row and column.")
        self._data: List[List[T]] = [list(row) for row in data]

    def get(self, point: Point) -> T | None:
        if self.out_of_bounds(point):
            return None
        return self._data[point.x][point.y]

    def set(self, point: Point, value: T) -> None:
        if self.out_of_bounds(point):
            raise IndexError("Point is outside grid bounds.")
        self._data[point.x][point.y] = value

    def copy(self) -> "Grid[T]":
        return Grid(self._data)

    def get_unique_items(self) -> list[T]:
        seen = set()
        unique: list[T] = []
        for row in self._data:
            for value in row:
                if value not in seen:
                    seen.add(value)
                    unique.append(value)
        return unique

    def find_items(self, target: T) -> list[Point]:
        matches: list[Point] = []
        for x, row in enumerate(self._data):
            for y, value in enumerate(row):
                if value == target:
                    matches.append(Point(x, y))
        return matches

    def find_connected_region(self, start: Point) -> list[Point]:
        start_value = self.get(start)
        if start_value is None:
            raise ValueError("Start point must be within the grid.")

        visited = {start}
        queue: list[Point] = [start]
        region: list[Point] = []

        while queue:
            current = queue.pop(0)
            region.append(current)

            for offset in _CARDINAL_OFFSETS:
                neighbor = Point(current.x + offset.x, current.y + offset.y)
                if neighbor in visited or self.out_of_bounds(neighbor):
                    continue

                value = self.get(neighbor)
                if value == start_value:
                    visited.add(neighbor)
                    queue.append(neighbor)

        return region

    def calculate_region_perimeter(self, points: Iterable[Point]) -> list[Point]:
        point_set = {point for point in points}
        perimeter: list[Point] = []

        for point in point_set:
            for offset in _CARDINAL_OFFSETS:
                neighbor = Point(point.x + offset.x, point.y + offset.y)
                if self.out_of_bounds(neighbor) or neighbor not in point_set:
                    perimeter.append(neighbor)

        return perimeter

    def swap_items(self, point_a: Point, point_b: Point) -> None:
        if self.out_of_bounds(point_a) or self.out_of_bounds(point_b):
            raise IndexError("Cannot swap items outside grid bounds.")

        self._data[point_a.x][point_a.y], self._data[point_b.x][point_b.y] = (
            self._data[point_b.x][point_b.y],
            self._data[point_a.x][point_a.y],
        )

    def get_adjacent_item(self, point: Point, offset: Point) -> tuple[Point, T | None]:
        neighbor = Point(point.x + offset.x, point.y + offset.y)
        return neighbor, self.get(neighbor)

    def get_surrounding_items(self, point: Point) -> list[Point]:
        neighbors = []
        for offset in _CARDINAL_OFFSETS:
            neighbor = Point(point.x + offset.x, point.y + offset.y)
            if not self.out_of_bounds(neighbor):
                neighbors.append(neighbor)
        return neighbors

    def out_of_bounds(self, point: Point) -> bool:
        return (
            point.x < 0
            or point.y < 0
            or point.x >= len(self._data)
            or point.y >= len(self._data[point.x])
        )

    def to_strings(self, mapper: callable | None = None) -> list[str]:
        mapper = mapper or (lambda value: str(value))
        return ["".join(mapper(value) for value in row) for row in self._data]

    def __iter__(self) -> Iterator[list[T]]:
        return iter(self._data)

