type Point = {
  x: number;
  y: number;
};

enum Direction {
  Up = "Up",
  Down = "Down",
  Left = "Left",
  Right = "Right",
}

const CARDINAL_DIRECTIONS: readonly Point[] = [
  { x: -1, y: 0 },
  { x: 1, y: 0 },
  { x: 0, y: -1 },
  { x: 0, y: 1 },
];

class Grid<T> {
  private readonly data: T[][];

  public constructor(data: T[][]) {
    if (data.length === 0 || data[0]?.length === 0) {
      throw new Error("Grid must have at least one row and column.");
    }

    this.data = data.map((row) => [...row]);
  }

  public get(point: Point): T | null {
    if (this.outOfBounds(point)) {
      return null;
    }

    const row = this.data[point.x]!;
    const value = row[point.y];
    return value ?? null;
  }

  public set(point: Point, value: T): void {
    if (this.outOfBounds(point)) {
      throw new RangeError("Point is outside grid bounds.");
    }

    this.data[point.x]![point.y] = value;
  }

  public copy(): Grid<T> {
    return new Grid(this.data.map((row) => [...row]));
  }

  public getUniqueItems(): T[] {
    return Array.from(new Set(this.data.flat()));
  }

  public findItems(target: T): Point[] {
    const points: Point[] = [];
    const comparer = (value: T): boolean => Object.is(value, target);

    for (let x = 0; x < this.data.length; x += 1) {
      const row = this.data[x]!;
      for (let y = 0; y < row.length; y += 1) {
        const value = row[y];
        if (value !== undefined && comparer(value)) {
          points.push({ x, y });
        }
      }
    }

    return points;
  }

  public findConnectedRegion(start: Point): Point[] {
    const startValue = this.get(start);
    if (startValue === null) {
      throw new RangeError("Start point must be within the grid.");
    }

    const visited = new Set<string>();
    const queue: Point[] = [start];
    const region: Point[] = [];
    visited.add(Grid.pointKey(start));

    while (queue.length > 0) {
      const point = queue.shift()!;
      region.push(point);

      for (const offset of CARDINAL_DIRECTIONS) {
        const neighbor = { x: point.x + offset.x, y: point.y + offset.y };

        if (this.outOfBounds(neighbor)) {
          continue;
        }

        const key = Grid.pointKey(neighbor);
        if (visited.has(key)) {
          continue;
        }

        const neighborValue = this.get(neighbor);
        if (neighborValue !== null && Object.is(neighborValue, startValue)) {
          visited.add(key);
          queue.push(neighbor);
        }
      }
    }

    return region;
  }

  public calculateRegionPerimeter(points: Iterable<Point>): Point[] {
    const pointSet = new Set<string>();
    const result: Point[] = [];

    for (const point of points) {
      pointSet.add(Grid.pointKey(point));
    }

    for (const pointKey of pointSet) {
      const point = Grid.parsePointKey(pointKey);

      for (const offset of CARDINAL_DIRECTIONS) {
        const neighbor = { x: point.x + offset.x, y: point.y + offset.y };
        if (this.outOfBounds(neighbor) || !pointSet.has(Grid.pointKey(neighbor))) {
          result.push(neighbor);
        }
      }
    }

    return result;
  }

  public swapItems(pointA: Point, pointB: Point): void {
    if (this.outOfBounds(pointA) || this.outOfBounds(pointB)) {
      throw new RangeError("Cannot swap items outside grid bounds.");
    }

    const rowA = this.data[pointA.x]!;
    const rowB = this.data[pointB.x]!;

    const temp = rowA[pointA.y];
    rowA[pointA.y] = rowB[pointB.y]!;
    rowB[pointB.y] = temp!;
  }

  public getAdjacentItem(point: Point, direction: Direction): [Point, T | null] {
    const neighbor: Point = (() => {
      switch (direction) {
        case Direction.Up:
          return { x: point.x - 1, y: point.y };
        case Direction.Down:
          return { x: point.x + 1, y: point.y };
        case Direction.Left:
          return { x: point.x, y: point.y - 1 };
        case Direction.Right:
          return { x: point.x, y: point.y + 1 };
        default:
          throw new RangeError("Unsupported direction");
      }
    })();

    return [neighbor, this.get(neighbor)];
  }

  public getSurroundingItems(point: Point): Point[] {
    return CARDINAL_DIRECTIONS.map((offset) => ({ x: point.x + offset.x, y: point.y + offset.y })).filter(
      (neighbor) => !this.outOfBounds(neighbor),
    );
  }

  public outOfBounds(point: Point): boolean {
    if (point.x < 0 || point.y < 0 || point.x >= this.data.length) {
      return true;
    }

    const row = this.data[point.x];
    return row === undefined || point.y >= row.length;
  }

  public toString(mapper: (value: T) => string): string[] {
    return this.data.map((row) => row.map(mapper).join(""));
  }

  private static pointKey(point: Point): string {
    return `${point.x}:${point.y}`;
  }

  private static parsePointKey(key: string): Point {
    const [rawX, rawY] = key.split(":");
    const x = Number(rawX);
    const y = Number(rawY);

    if (Number.isNaN(x) || Number.isNaN(y)) {
      throw new Error(`Invalid point key: ${key}`);
    }

    return { x, y };
  }
}

export { Direction, Grid };
export type { Point };

