
using Common;
using Coor = Common.Coor<long>;

namespace AoC_2023;

public class Day_18 : BaseDay {
  public IEnumerable<DigInstruction> _instructions;

  public Day_18()
  {
    _instructions = File.ReadAllLines(InputFilePath).Select(x => x.Split()).Select(x => {
      return new DigInstruction(x[0][0], int.Parse(x[1]), x[2].TrimStart("(#".ToCharArray()).TrimEnd(")".ToCharArray()));
    });
  }

  public override ValueTask<string> Solve_1()
  {
    return new(_instructions.CalcArea().ToString());
  }

  public override ValueTask<string> Solve_2()
  {
    return new(_instructions.Select(x => x.GetPart2Instruction()).CalcArea().ToString());
  }
}

public readonly record struct DigInstruction(char Direction, int Distance, string Color) {

  public DigInstruction GetPart2Instruction() {
    var distanceAsHex = Color[..^1];
    var distance = int.Parse(distanceAsHex, System.Globalization.NumberStyles.HexNumber);
    var direction = Color[^1] switch {
      '0' => 'R',
      '1' => 'D',
      '2' => 'L',
      _   => 'U'
    };

    return new(direction, distance, Color);
  }
};
public readonly record struct DugBlock(Coor Pos, string Color);

public static class AreaExtensions {
  public static long CalcArea(this IEnumerable<DigInstruction> instrs)
  {
      Coor pt = new(0, 0);
      List<Coor> pts = [new (0, 0)];
      long perimeter = 0;

      foreach (var p in instrs)
      {
          pt = p.Direction switch
          {
              'R' => pt with { X = pt.X + p.Distance},
              'L' => pt with { X = pt.X - p.Distance},
              'U' => pt with { Y = pt.Y + p.Distance},
              _ => pt with { Y = pt.Y - p.Distance}
          };

          perimeter += p.Distance;
          pts.Add(pt);
      }

      // The shoelace formula sums the determinants of the points in sequence. These are the interior
      // squares but we need to add the perimiter and divide by 2 + 1 because we have chonky ASCII lines
      long area = 0;
      for (int j = 0; j < pts.Count - 1; j++)
            area += Determinant(pts[j].X, pts[j].Y, pts[j + 1].X, pts[j + 1].Y);
      area = Math.Abs(area);
          
      return (area + perimeter) / 2 + 1;
  }

    private static long Determinant(long x1, long y1, long x2, long y2)
    {
        return x1 * y2 - x2 * y1;
    }
}