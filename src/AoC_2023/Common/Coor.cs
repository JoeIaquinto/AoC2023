using System.Numerics;

namespace Common;

public record Coor<T>(T Y, T X) where T : INumber<T>
{
    public T Row => Y;
    public T Col => X;

    public static readonly Coor<T> Zero = new(T.Zero, T.Zero);
    public static readonly Coor<T> One = new(T.One, T.One);

    public static readonly Coor<T> Up = new(-T.One, T.Zero);
    public static readonly Coor<T> Down = new(T.One, T.Zero);
    public static readonly Coor<T> Left = new(T.Zero, -T.One);
    public static readonly Coor<T> Right = new(T.Zero, T.One);

    public static Coor<T> operator -(Coor<T> me, Coor<T> other) => new(Y: me.Y - other.Y, X: me.X - other.X);
    public static Coor<T> operator +(Coor<T> me, Coor<T> other) => new(Y: me.Y + other.Y, X: me.X + other.X);


    public static readonly Coor<T>[] Directions =
    [
        Right,
        Down,
        Left,
        Up,
    ];

    public static readonly Coor<T>[] FourWayNeighbours =
    [
        new (-T.One, T.Zero),
        new (T.Zero, -T.One),
        new (T.Zero, T.One),
        new (T.One, T.Zero),
    ];

    public static readonly Coor<T>[] NineWayNeighbours =
    [
        new(-T.One, -T.One),
        new(-T.One, T.Zero),
        new(-T.One, T.One),
        new(T.Zero, -T.One),
        new(T.Zero, T.One),
        new(T.One, -T.One),
        new(T.One, T.Zero),
        new(T.One, T.One),
    ];

    public IEnumerable<Coor<T>> GetFourWayNeighbours()
        => FourWayNeighbours.Select(c => this + c);

    public bool IsOpposite(Coor<T> other) =>
        (this == Coor<T>.Right && other == Coor<T>.Left)
        || (this == Coor<T>.Left && other == Coor<T>.Right)
        || (this == Coor<T>.Up && other == Coor<T>.Down)
        || (this == Coor<T>.Down && other == Coor<T>.Up);

    public static bool operator ==(Coor<T> me, (T, T) other) => new Coor<T>(other.Item1, other.Item2) == me;
    public static bool operator !=(Coor<T> me, (T, T) other) => !(me == other);
    public static Coor<T> operator +(Coor<T> me, (T, T) other) => new(Y: me.Y + other.Item1, X: me.X + other.Item2);
    public static Coor<T> operator -(Coor<T> me, (T, T) other) => new(Y: me.Y - other.Item1, X: me.X - other.Item2);
    public static Coor<T> operator *(Coor<T> me, T distance) => new(Y: me.Y * distance, X: me.X * distance);
    public static Coor<T> operator /(Coor<T> me, T distance) => new(Y: me.Y / distance, X: me.X / distance);

    public static T ManhattanDistance(Coor<T> me, Coor<T> other)
    {
        var y = me.Y - other.Y;
        if (y < T.Zero)
        {
            y *= -T.One;
        }

        var x = me.X - other.X;
        if (x < T.Zero)
        {
            x *= -T.One;
        }

        return x + y;
    }

    public T ManhattanDistance(Coor<T> other) => ManhattanDistance(this, other);

    public override string ToString() => $"[{Y},{X}]";
}

public static class CoorExtensions
{
    public static Coor<T> Rotate90DegLeft<T>(this Coor<T> coor) where T : INumber<T> {
      if (coor == Coor<T>.Right) {
        return Coor<T>.Up;
      }
      if (coor == Coor<T>.Up) {
        return Coor<T>.Left;
      }
      if (coor == Coor<T>.Left) {
        return Coor<T>.Down;
      }
      if (coor == Coor<T>.Down) {
        return Coor<T>.Right;
      }
      throw new Exception("Invalid direction");
    }

    public static Coor<T> Rotate90DegRight<T>(this Coor<T> coor) where T : INumber<T> {
      if (coor == Coor<T>.Right) {
        return Coor<T>.Down;
      }
      if (coor == Coor<T>.Down) {
        return Coor<T>.Left;
      }
      if (coor == Coor<T>.Left) {
        return Coor<T>.Up;
      }
      if (coor == Coor<T>.Up) {
        return Coor<T>.Right;
      }
      throw new Exception("Invalid direction");
    }

    public static bool InBoundsOf<R>(this Coor<int> coor, R[,] array)
        => coor.Y >= 0 && coor.Y < array.Height() && coor.X >= 0 && coor.X < array.Width();

    public static void Visualize(this ICollection<Coor<int>> coors, Coor<int>? min, Coor<int>? max)
    {
        min ??= new Coor<int>(coors.Min(c => c.Y), coors.Min(c => c.X));
        max ??= new Coor<int>(coors.Max(c => c.Y), coors.Max(c => c.X));

        var width = max.X - min.X + 1;
        var height = max.Y - min.Y + 1;

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                Console.Write(coors.Contains(new(y, x)) ? '#' : '.');
            }

            Console.WriteLine();
        }
    }

    public static void Print(this char[,] map, Func<Coor<int>, char?>? printOverride = null)
    {
        for (var y = 0; y < map.Height(); y++)
        {
            for (var x = 0; x < map.Width(); x++)
            {
                var c = printOverride?.Invoke(new(y, x)) ?? map[y, x];
                Console.Write(c);
            }

            Console.WriteLine();
        }
    }

    public static void Print(this int[,] map, Func<Coor<int>, char?>? printOverride = null)
    {
        for (var y = 0; y < map.Height(); y++)
        {
            for (var x = 0; x < map.Width(); x++)
            {
                var c = printOverride?.Invoke(new(y, x)) ?? map[y, x];
                Console.Write(c);
            }

            Console.WriteLine();
        }
    }

    public static T Get<T>(this T[,] items, Coor<int> coor)
        => items[coor.Y, coor.X];
    
    public static T Set<T>(this T[,] items, Coor<int> coor, T value)
        => items[coor.Y, coor.X] = value;
}