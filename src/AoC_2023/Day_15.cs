using MoreLinq;
using SheepTools.Extensions;

namespace AoC_2023;

public class Day_15 : BaseDay 
{

    public IEnumerable<string> _input;
    public IEnumerable<LensOperation> _operations;
    public Day_15()
    {
        _input = File.ReadAllText(InputFilePath).Where(x => !char.IsWhiteSpace(x)).Split(',').Select(x => string.Concat(x));
        _operations = _input.Select(x =>
        {
          var label = string.Concat(x.TakeWhile(char.IsLetter));
          var operationChar = x[label.Length];
          int? focalLength = operationChar == '=' ? int.Parse(x[label.Length + 1].ToString()) : null;
          return new LensOperation(label, operationChar, focalLength);
        });
    }

    public override ValueTask<string> Solve_1()
    {
        return new(_input.Sum(x => x.Hash()).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
      var boxes = new Dictionary<int, List<(string label, int focalLength)>>(256);
      boxes.AddRange(Enumerable.Range(0, 256).Select(x => new KeyValuePair<int, List<(string, int)>>(x, [])).ToArray());
      foreach (var operation in _operations) {
        var box = boxes[operation.GetHashCode()];
        switch (operation.Operation) {
          case '=':
            if (box.Any(x => x.label == operation.Label))
            {
              box[box.FindIndex(x => x.label == operation.Label)] = (operation.Label, operation.FocalLength!.Value);
            } 
            else 
            {
              box.Add((operation.Label, operation.FocalLength!.Value));
            }
            break;
          case '-':
            box.RemoveAll(x => x.label == operation.Label);
            break;
        }
      }
      var focusingPower = boxes.Sum(x => {
        var boxNum = x.Key + 1;
        
        long mult = x.Value.Select<(string, int), ((string label, int focalLength) lens, int slotIndex)>((v, i) => new (v, i))
          .Aggregate(0, (acc, y) => {
            var (label, focalLength) = y.lens;
            var slotIndex = y.slotIndex;
            var slotValue = (slotIndex + 1) * focalLength * boxNum;
            return acc + slotValue;
        });
        return mult;
      });
      return new(focusingPower.ToString());
    }
}

public record LensOperation
{
    public LensOperation(string label, char operation, int? focalLength)
    {
        Label = label;
        Operation = operation;
        FocalLength = focalLength;
        _hash = (int)Label.Hash();
    }

    public string Label { get; init; }
    public char Operation { get; init; }
    public int? FocalLength { get; init; }
    private readonly int _hash;
    public override int GetHashCode() => _hash;
}

public static class StringExtensions
{
    public static long Hash(this string input)
    {
        return input.Aggregate(0L, (acc, x) => (acc + x)  * 17  % 256);
    }
}