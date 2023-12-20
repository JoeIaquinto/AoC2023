using Common;
using SheepTools.Model;
using Coor = Common.Coor<int>;

namespace AoC_2023;

public class Day_16 : BaseDay 
{

    public IEnumerable<string> _input;

    public char[,] _map;
    public Day_16()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
        _map = _input.ParseAsArray();
    }

    public override ValueTask<string> Solve_1()
    {
      var energized = SimulateBeams(_map, new Coor(0, -1), Coor.Right);
      return new(energized.ToString());
    }


    public static int SimulateBeams(char[,] map, Coor start, Coor startDirection) {
      var energized = new bool[map.Height(), map.Width()];
      var used = new bool[map.Height(), map.Width()];

      var beams = new Queue<Beam>([new Beam(start, startDirection)]);
      int result = 0;

      while(beams.TryDequeue(out var beam))
      {
        var next = beam.Position + beam.Direction;
        if (!next.InBoundsOf(map))
        {
          continue;
        }

        if (!energized.Get(next))
        {
          energized.Set(next, true);
          result++;
        }

        switch (map.Get(next))
        {
          case '.':
          {
            beams.Enqueue(new(next, beam.Direction));
            break;
          }
          case '-':
          {
            if (beam.Direction == Coor.Left || beam.Direction == Coor.Right) {
              beams.Enqueue(new(next, beam.Direction));
              break;
            }
            else
            {
              if (used.Get(next))
              {
                break;
              }
              
              beams.Enqueue(new(next, Coor.Left));
              beams.Enqueue(new(next, Coor.Right));
              used.Set(next, true);
              break;
            }
          }
          case '|':
          {
            if (beam.Direction == Coor.Up || beam.Direction == Coor.Down) {
              beams.Enqueue(new(next, beam.Direction));
              break;
            }
            else
            {
              if (used.Get(next))
              {
                break;
              }
              used.Set(next, true);
              beams.Enqueue(new(next, Coor.Up));
              beams.Enqueue(new(next, Coor.Down));
              break;
            }
          }
          case '\\':
          {
            
            beams.Enqueue(new Beam(next, Redirect(beam.Direction,
              (Coor.Up, Coor.Left),
              (Coor.Left, Coor.Up),
              (Coor.Down, Coor.Right),
              (Coor.Right, Coor.Down))));
            break;
          }
          case '/':
          {
            beams.Enqueue(new Beam(next, Redirect(beam.Direction,
              (Coor.Up, Coor.Right),
              (Coor.Right, Coor.Up),
              (Coor.Down, Coor.Left),
              (Coor.Left, Coor.Down))));
            break;
          }
          default:
            throw new NotImplementedException();
        }
      }
      //map.Print(x => energized.Get(x) ? '#' : null);
      return result;
    }

    public override ValueTask<string> Solve_2()
    {
      var height = _map.Height();
      var width = _map.Width();
      int[] allScores =
      [
          .. Enumerable.Range(0, height).Select(row => SimulateBeams(_map, new Coor(row, -1), Coor.Right)),
          .. Enumerable.Range(0, height).Select(row => SimulateBeams(_map, new Coor(row, width), Coor.Left)),
          .. Enumerable.Range(0, width).Select(col => SimulateBeams(_map, new Coor(-1, col), Coor.Down)),
          .. Enumerable.Range(0, width).Select(col => SimulateBeams(_map, new Coor(height, col), Coor.Up)),
      ];
      return new(allScores.Max().ToString());
    }

    static Coor Redirect(Coor coor, params (Coor, Coor)[] transformations)
      => transformations.First(t => t.Item1 == coor).Item2;
}

file record struct Beam(Coor Position, Coor Direction);