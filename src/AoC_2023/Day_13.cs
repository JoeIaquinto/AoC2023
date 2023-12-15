
using System.Diagnostics;
using MoreLinq;
using SheepTools.Extensions;

namespace AoC_2023;
public class Day_13 : BaseDay
{
  private readonly IEnumerable<string> _input;
  private readonly IEnumerable<char[][]> _mirrorSets;

  public Day_13()
  {
    _input = File.ReadAllLines(InputFilePath);
    _mirrorSets = _input.Split(x => x.IsEmpty()).Select(x => x.Select(y => y.ToCharArray()).ToArray());
  }

  public override ValueTask<string> Solve_1()
  {
    long sum = _mirrorSets.Sum(x => {
      (var isValidPivot, var pivotIndex) = FindMirrorRowIndex(x.Pivot());
      (var isValidRow, var rowIndex) = FindMirrorRowIndex(x);
      return pivotIndex + (rowIndex*100);
    });
    return new (sum.ToString());
  }

  public override ValueTask<string> Solve_2()
  {
    long sum = _mirrorSets.Sum(mirror => {

      (var isValidPivot, var pivotIndex) = FindMirrorRowIndex(mirror.Pivot());
      (var isValidRow, var rowIndex) = FindMirrorRowIndex(mirror);
      if (!(isValidPivot || isValidRow)) {
        return 0;
      }

      var flippedMirror = mirror.Clone() as char[][];
      foreach ((char[] topRow, int y) in mirror.Select<char[], (char[], int)>((row, index) => new(row, index))) 
      {
        foreach ((char el, int x) in topRow.Select<char, (char, int)>((ch, jIndex) => new(ch, jIndex)))
          {
            
            flippedMirror![y][x] = el == '#' ? '.' : '#';

            (bool isValidRowFlipped, var flippedRowIndex) = FindMirrorRowIndex(flippedMirror, isValidRow ? rowIndex - 1 : null);
            if (isValidRowFlipped && flippedRowIndex != rowIndex) {
              return flippedRowIndex*100;
            }
            (bool isValidPivotFlipped, var flippedPivotIndex) = FindMirrorRowIndex(flippedMirror.Pivot(), isValidPivot ? pivotIndex - 1 : null);
            if (isValidPivotFlipped && flippedPivotIndex != pivotIndex){
              return flippedPivotIndex;
            }
            flippedMirror[y][x] = el;
          }
      }
      throw new Exception("No flip found");
    });
    return new (sum.ToString());
  }

  public static (bool, int) FindMirrorRowIndex(char[][] map, int? originalIndex = null)
  {
    var height = map.Length;
    // Track a top mirror and a bottom mirror, return i if they are equal
    // the mirrors must be the same length, and the bottom mirror follows the top mirror
    for (int i = 0; i < height - 1; i++)
    {
      if (originalIndex.HasValue && i == originalIndex.Value) {
        continue;
      }
      var reflectionPoint = i + 1;
      var topMirrorStart = Math.Max(0, reflectionPoint * 2 - height); // 0 until halfway, then increase along 1, 3, 5, 7, 9, 11, 13, 15, 17, 19
      var bottomMirrorEnd = Math.Min(reflectionPoint * 2, height); // reflectionPoint * 2 until halfway, then height
      char[][] topMirror = map[topMirrorStart..reflectionPoint];
      char[][] bottomMirror = map[reflectionPoint..bottomMirrorEnd].Reverse().ToArray();

      if(topMirror.Select((x, i) => x.SequenceEqual(bottomMirror[i])).All(x => x)) {
        return (true, i + 1);
      }
    }
    return (false, 0);
  }
  public static void PrintMap(char[][] map, string mapName = "") {
    Console.WriteLine(mapName);
    Console.WriteLine("".PadLeft(map[0].Length, '-'));
    foreach (var row in map) {
      Console.WriteLine(row);
    }
    Console.WriteLine("".PadLeft(map[0].Length, '-'));
  }
}

public static class ArrayExtensions {
  public static char[][] Pivot(this char[][] map) {
    return map.Transpose().Select(x => x.ToArray()).ToArray();
  }
}