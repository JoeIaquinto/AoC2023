using System.Diagnostics;

namespace AoC_2023;
public class Day_14 : BaseDay
{
  private readonly char[][] _input;

  private readonly char[][] _testCaseSlide = [
    ['O', 'O', 'O', 'O', '.', '#', '.', 'O', '.', '.', ],
    ['O', 'O', '.', '.', '#', '.', '.', '.', '.', '#', ],
    ['O', 'O', '.', '.', 'O', '#', '#', '.', '.', 'O', ],
    ['O', '.', '.', '#', '.', 'O', 'O', '.', '.', '.', ],
    ['.', '.', '.', '.', '.', '.', '.', '.', '#', '.', ],
    ['.', '.', '#', '.', '.', '.', '.', '#', '.', '#', ],
    ['.', '.', 'O', '.', '.', '#', '.', 'O', '.', 'O', ],
    ['.', '.', 'O', '.', '.', '.', '.', '.', '.', '.', ],
    ['#', '.', '.', '.', '.', '#', '#', '#', '.', '.', ],
    ['#', '.', '.', '.', '.', '#', '.', '.', '.', '.', ],
  ];

  private readonly long testCaseLoad = 136;

  public Day_14()
  {
    _input = File.ReadAllLines(InputFilePath).Select(x => x.ToCharArray()).ToArray();
  }

  public override ValueTask<string> Solve_1()
  {
    var slideNorth = _input.SlideNorth();
    long sum = slideNorth.CalculateLoad();
    return new (sum.ToString());
  }

  public override ValueTask<string> Solve_2()
  {
    if (_input.Clone() is not char[][] cycle)
    {
        throw new Exception("Failed to clone grid");
    }
    
    var seenStates = new Dictionary<string, int>();
    var cycle_count = 1000000000;
    for (int i = 0; i < cycle_count; i++) {
      if (i % 1000 == 0) {
        Console.WriteLine($"Cycle {i}");
      }
      cycle = cycle.Cycle();
      var h = string.Join("|", cycle.Select(x => string.Join("", x)));
      if (seenStates.TryGetValue(h, out var lastTime))
      {
        var currentCycle = i - lastTime;
        var increaseIBy = (cycle_count - i) / currentCycle * currentCycle;
        i += increaseIBy;
      }
      seenStates[h] = i;
    }
    long sum = cycle.CalculateLoad();
    return new (sum.ToString());
  }
}

public static class GridUtilities {
  public static char[][] SlideNorth(this char[][] grid) {
    if (grid.Clone() is not char[][] newGrid)
    {
        throw new Exception("Failed to clone grid");
    }

    int[] highestEmptyAbove = new int[grid[0].Length];
    for (int x = 0; x < grid[0].Length; x++)
    {
        highestEmptyAbove[x] = -1;
    }

    for (int y = 0; y < grid.Length; y++) {
        for (int x = 0; x < grid[y].Length; x++) {
            var element = grid[y][x];
            if (element == '.' && highestEmptyAbove[x] == -1) {
                highestEmptyAbove[x] = y;
            } else if (element == '#'){
                highestEmptyAbove[x] = -1;
            } else if (element == 'O' && highestEmptyAbove[x] != -1) {
                var moveToY = highestEmptyAbove[x];
                newGrid[moveToY][x] = 'O';
                newGrid[y][x] = '.';
                highestEmptyAbove[x]++;
            }
        }
    }
    return newGrid;
  }

  public static char[][] SlideSouth(this char[][] grid) {
    if (grid.Clone() is not char[][] newGrid)
    {
        throw new Exception("Failed to clone grid");
    }

    int[] lowestEmptyBelow = new int[grid[0].Length];
    for (int x = 0; x < grid[0].Length; x++)
    {
        lowestEmptyBelow[x] = grid.Length;
    }

    for (int y = grid.Length - 1; y >= 0; y--) {
      for (int x = 0; x < grid[y].Length; x++) {
        var element = grid[y][x];
        if (element == '.' && lowestEmptyBelow[x] == grid.Length) {
            lowestEmptyBelow[x] = y;
        } else if (element == '#'){
            lowestEmptyBelow[x] = grid.Length;
        } else if (element == 'O' && lowestEmptyBelow[x] != grid.Length) {
            var moveToY = lowestEmptyBelow[x];
            newGrid[moveToY][x] = 'O';
            newGrid[y][x] = '.';
            lowestEmptyBelow[x]--;
        }
      }
    }
    return newGrid;
  }

  public static char[][] SlideWest(this char[][] grid) {
    if (grid.Clone() is not char[][] newGrid)
    {
        throw new Exception("Failed to clone grid");
    }
    int[] leftmostEmpty = new int[grid.Length];
    for (int x = 0; x < grid.Length; x++)
    {
        leftmostEmpty[x] = -1;
    }

    for (int y = 0; y < grid.Length; y++) {
      for (int x = 0; x < grid[y].Length; x++) {
        var element = grid[y][x];
        if (element == '.' && leftmostEmpty[y] == -1) {
            leftmostEmpty[y] = x;
        } else if (element == '#'){
            leftmostEmpty[y] =  -1;
        } else if (element == 'O' && leftmostEmpty[y] != -1) {
            var moveToX = leftmostEmpty[y];
            newGrid[y][moveToX] = 'O';
            newGrid[y][x] = '.';
            leftmostEmpty[y]++;
        }
      }
    }
    return newGrid;
  }

  public static char[][] SlideEast(this char[][] grid) {
    if (grid.Clone() is not char[][] newGrid)
    {
        throw new Exception("Failed to clone grid");
    }

    int[] rightmostEmpty = new int[grid.Length];
    for (int x = 0; x < grid.Length; x++)
    {
        rightmostEmpty[x] = grid[0].Length;
    }

    for (int y = 0; y < grid.Length; y++) {
      for (int x = grid[y].Length - 1; x >= 0; x--) {

        var element = grid[y][x];
        if (element == '.' && rightmostEmpty[y] == grid[0].Length) {
            rightmostEmpty[y] = x;
        } else if (element == '#'){
            rightmostEmpty[y] =  grid[0].Length;
        } else if (element == 'O' && rightmostEmpty[y] != grid[0].Length) {
            var moveToX = rightmostEmpty[y];
            newGrid[y][moveToX] = 'O';
            newGrid[y][x] = '.';
            rightmostEmpty[y]--;
        }
      }
    }
    return newGrid;
  }

  public static char[][] Cycle(this char[][] grid)
  {
    return grid.SlideNorth().SlideWest().SlideSouth().SlideEast();
  }

  public static long CalculateLoad(this char[][] grid) {
    long sum = 0;
    for (int y = 0; y < grid.Length; y++) {
      for (int x = 0; x < grid[y].Length; x++) {
        var element = grid[y][x];
        if (element != 'O') {
          continue;
        }
        sum += grid.Length - y;
      }
    }
    return sum;
  }
}