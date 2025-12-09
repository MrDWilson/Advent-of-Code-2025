using System.Drawing;

namespace AdventOfCode.Models;

public enum Direction { Up, Down, Left, Right }

public class Grid<T>(List<List<T>> _data)
{
    public int RowCount => _data.Count;
    public int ColumnCount => _data[0].Count;

    public T? this[Point point]
    {
        get
        {
            if (OutOfBounds(point)) return default;
            return _data[point.X][point.Y];
        }

        set 
        {
            if (OutOfBounds(point)) throw new ArgumentOutOfRangeException(nameof(point));
            if (value is null) throw new ArgumentNullException(nameof(value));

            _data[point.X][point.Y] = value;
        }
    }

    public Grid<T> Copy()
    {
        return new(_data.Select(x => x.ToList()).ToList());
    }

    public IEnumerable<T> GetUniqueItems()
    {
        return _data.SelectMany(x => x).Distinct();
    }

    public IEnumerable<Point> FindItems(T item)
    {
        return _data.Index()
            .SelectMany(x => x.Item.Index().Select(y => new Point(x.Index, y.Index)))
            .Where(x => this[x] is T t && t.Equals(item));
    }

    public IEnumerable<Point> FindConnectedRegion(Point start)
    {
        T targetValue = this[start] ?? throw new ArgumentOutOfRangeException(nameof(start));

        List<Point> visited = [];

        var directions = new Point[] 
        {
            new(-1, 0), // up
            new(1, 0),  // down
            new(0, -1), // left
            new(0, 1)   // right
        };

        var queue = new Queue<Point>();
        queue.Enqueue(start);
        visited.Add(start);

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            yield return point;

            var neighbors = directions
                .Select(d => new Point(point.X + d.X, point.Y + d.Y))
                .Where(n => !OutOfBounds(n));

            foreach (var neighbour in neighbors)
            {
                if (!visited.Contains(neighbour) && (this[neighbour]?.Equals(targetValue) ?? false))
                {
                    visited.Add(neighbour);
                    queue.Enqueue(neighbour);
                }
            }
        }
    }

    public IEnumerable<Point> CalculateRegionPerimeter(IEnumerable<Point> points)
    {
        var pointsSet = new HashSet<Point>(points);

        var directions = new Point[] 
        {
            new(-1, 0), // up
            new(1, 0),  // down
            new(0, -1), // left
            new(0, 1)   // right
        };

        foreach (var point in pointsSet)
        {
            foreach (var direction in directions)
            {
                var neighbour = new Point(point.X + direction.X, point.Y + direction.Y);
                if (OutOfBounds(neighbour) || !pointsSet.Contains(neighbour))
                {
                    yield return neighbour;
                }
            }
        }
    }

    public void SwapItems(Point point1, Point point2)
    {
        (_data[point1.X][point1.Y], _data[point2.X][point2.Y]) = (_data[point2.X][point2.Y], _data[point1.X][point1.Y]);
    }

    public (Point, T?) GetAdjacentItem(Point point, Direction direction)
    {
        Point location = direction switch
        {
            Direction.Up => new(point.X - 1, point.Y), 
            Direction.Down => new(point.X + 1, point.Y), 
            Direction.Left => new(point.X, point.Y - 1), 
            Direction.Right => new(point.X, point.Y + 1),
            _ => throw new ArgumentOutOfRangeException(nameof(direction))
        };

        return (location, this[location]);
    }

    public IEnumerable<Point> GetSurroundingItems(Point point)
    {
        Point[] locations = 
        [
            new Point(point.X - 1, point.Y), 
            new Point(point.X + 1, point.Y), 
            new Point(point.X, point.Y - 1), 
            new Point(point.X, point.Y + 1)
        ];

        return locations.Where(x => !OutOfBounds(x));
    }

    public IEnumerable<Point> GetSurroundingItemsDiagonally(Point point)
    {
        Point[] locations = 
        [
            new Point(point.X - 1, point.Y - 1),
            new Point(point.X - 1, point.Y + 1),
            new Point(point.X + 1, point.Y - 1),
            new Point(point.X + 1, point.Y + 1),
            new Point(point.X - 1, point.Y), 
            new Point(point.X + 1, point.Y), 
            new Point(point.X, point.Y - 1), 
            new Point(point.X, point.Y + 1)
        ];

        return locations.Where(x => !OutOfBounds(x));
    }

    public long GetArea(Point corner1, Point corner2)
    {
        var minX = Math.Max(0, Math.Min(corner1.X, corner2.X));
        var maxX = Math.Min(_data.Count - 1, Math.Max(corner1.X, corner2.X));
        var minY = Math.Max(0, Math.Min(corner1.Y, corner2.Y));
        var maxY = Math.Min(_data[0].Count - 1, Math.Max(corner1.Y, corner2.Y));

        if (minX > maxX || minY > maxY) return 0;

        return (long)(maxX - minX + 1) * (maxY - minY + 1);
    }

    public IEnumerable<Point> GetAreaPoints(Point corner1, Point corner2)
    {
        var minX = Math.Max(0, Math.Min(corner1.X, corner2.X));
        var maxX = Math.Min(_data.Count - 1, Math.Max(corner1.X, corner2.X));
        var minY = Math.Max(0, Math.Min(corner1.Y, corner2.Y));
        var maxY = Math.Min(_data[0].Count - 1, Math.Max(corner1.Y, corner2.Y));
        
        for (var x = minX; x <= maxX; x++)
            for (var y = minY; y <= maxY; y++)
                yield return new Point(x, y);
    }

    public bool AllInArea(Point corner1, Point corner2, Func<T?, bool> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        var minX = Math.Max(0, Math.Min(corner1.X, corner2.X));
        var maxX = Math.Min(RowCount - 1, Math.Max(corner1.X, corner2.X));
        var minY = Math.Max(0, Math.Min(corner1.Y, corner2.Y));
        var maxY = Math.Min(ColumnCount - 1, Math.Max(corner1.Y, corner2.Y));

        for (var x = minX; x <= maxX; x++)
        {
            var row = _data[x];
            for (var y = minY; y <= maxY; y++)
            {
                if (!predicate(row[y]))
                    return false;
            }
        }

        return true;
    }

    public void FillEnclosedRegions(Func<T?, bool> isFillCandidate, T fillValue)
    {
        ArgumentNullException.ThrowIfNull(isFillCandidate);
        ArgumentNullException.ThrowIfNull(fillValue);

        var height = RowCount;
        var width = ColumnCount;
        var fillable = new bool[height, width];
        var outside = new bool[height, width];
        var queue = new Queue<Point>();
        Point[] directions =
        [
            new(-1, 0),
            new(1, 0),
            new(0, -1),
            new(0, 1)
        ];

        for (int x = 0; x < height; x++)
        {
            var row = _data[x];
            for (int y = 0; y < width; y++)
                fillable[x, y] = isFillCandidate(row[y]);
        }

        void EnqueueIfFillable(int x, int y)
        {
            if (x < 0 || y < 0 || x >= height || y >= width) return;
            if (!fillable[x, y] || outside[x, y]) return;
            outside[x, y] = true;
            queue.Enqueue(new Point(x, y));
        }

        for (int x = 0; x < height; x++)
        {
            EnqueueIfFillable(x, 0);
            EnqueueIfFillable(x, width - 1);
        }

        for (int y = 0; y < width; y++)
        {
            EnqueueIfFillable(0, y);
            EnqueueIfFillable(height - 1, y);
        }

        while (queue.Count > 0)
        {
            var point = queue.Dequeue();
            foreach (var direction in directions)
                EnqueueIfFillable(point.X + direction.X, point.Y + direction.Y);
        }

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                if (fillable[x, y] && !outside[x, y])
                    _data[x][y] = fillValue;
            }
        }
    }

    public bool OutOfBounds(Point coords)
        => coords.X < 0 || coords.Y < 0 || coords.X >= _data.Count || coords.Y >= _data.First().Count;

    public string[] ToString(Func<T, string> func)
    {
        return [.. _data.Select(x => string.Join("", x.Select(func)))];
    }
}