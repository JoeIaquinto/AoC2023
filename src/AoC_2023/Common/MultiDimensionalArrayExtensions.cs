using System.Text;

namespace Common;
public static class MultiDimensionalArrayExtensions
{
    public static int Height<T>(this T[,] array) => array.GetLength(0);
    public static int Width<T>(this T[,] array) => array.GetLength(1);

    public static T[,] InitializeWith<T>(this T[,] array, T value)
    {
        var h = array.Height();
        var w = array.Width();
        for (int i = 0; i < h * w; i++)
        {
            array[i % h, i / h] = value;
        }

        return array;
    }

    public static IEnumerable<Coor<int>> AllCoordinates<T>(this T[,] array)
    {
        for (int i = 0; i < array.Height() * array.Width(); i++)
        {
            yield return new Coor<int>(i % array.Height(), i / array.Height());
        }
    }

    public static void ForEach<T>(this T[,] array, Action<int, int, T> action)
    {
        for (int row = 0; row < array.Height(); row++)
            for (int col = 0; col < array.Width(); col++)
                action(row, col, array[row, col]);
    }

    public static string ToFlatString(this char[,] array)
    {
        var h = array.Height();
        var w = array.Width();
        var result = new StringBuilder(h * w);
        for (int i = 0; i < h * w; i++)
        {
            result.Append(array[i % h, i / h]);
        }

        return result.ToString();
    }

    public static T[,] ParseAsArray<T>(this IEnumerable<string> input, Func<char, T> parser)
    {
      var list = input.ToList();
      var columnCount = list.First().Length;
      var result = new T[list.Count, columnCount];

      for (int row = 0; row < list.Count; row++)
      for (int column = 0; column < columnCount; column++)
      {
          result[row, column] = parser(list[row][column]);
      }

      return result;
    }
    public static char[,] ParseAsArray(this IEnumerable<string> input)
    => ParseAsArray(input, c => c);
}