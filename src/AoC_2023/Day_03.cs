namespace AoC_2023;

using System;
using SheepTools.Extensions;

public class Day_03 : BaseDay
{
    private readonly List<string> _input;
    private readonly List<string> _input2;

    public Day_03()
    {
        _input = [.. File.ReadAllLines(InputFilePath)];
        _input2 = [.. File.ReadAllLines(InputFilePath)];
    }

    private IEnumerable<int> GetAdjacentNumbers(List<string> input, int lineNum, int index)
    {
        var validSpaces = new List<Tuple<int, int>>();
        if (lineNum > 0)
        {
            if (index > 0)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum - 1, index - 1));
            }
            validSpaces.Add(new Tuple<int, int>(lineNum - 1, index));
            if (index < input.ElementAt(lineNum - 1).Length - 1)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum - 1, index + 1));
            }
        }
        if (index > 0)
        {
            validSpaces.Add(new Tuple<int, int>(lineNum, index - 1));
        }
        if (index < input.ElementAt(lineNum - 1).Length - 1)
        {
            validSpaces.Add(new Tuple<int, int>(lineNum, index + 1));
        }
        if (lineNum < input.Count - 1)
        {
            if (index > 0)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum + 1, index - 1));
            }
            validSpaces.Add(new Tuple<int, int>(lineNum + 1, index));
            if (index < input.ElementAt(lineNum - 1).Length - 1)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum + 1, index + 1));
            }
        }

        var adjacentNumberSet = new HashSet<int>();
        foreach (var item in validSpaces)
        {
            var number = ParseNumberFromLocation(input, item.Item1, item.Item2);
            if (number != null)
            {
                adjacentNumberSet.Add(number.Value);
            }
        }
        return adjacentNumberSet.ToList();
    }

    private int? ParseNumberFromLocation(List<string> input, int v, int i)
    {
        if (v >= input.Count)
        {
            return null;
        }
        if (i >= input.ElementAt(v).Length)
        {
            return null;
        }
        var charAtLocation = input.ElementAt(v).ElementAt(i);
        if (!char.IsDigit(charAtLocation)) { 
            return null; 
        }
        var line = input.ElementAt(v);
        var right = line[i..].TakeWhile(x => char.IsDigit(x)).ToList();
        var left = line[..i].Reverse().TakeWhile(x => char.IsDigit(x)).Reverse().ToList();
        var s = "";
        var charAtNumber = left.Count > 0 ? left[0] : charAtLocation;
        var index = i - left.Count;
        while (char.IsDigit(charAtNumber))
        {
            s += charAtNumber;
            input[v] = input[v][..index] + '.' + input[v][(index + 1)..];
            index++;
            if (index == line.Length)
            {
                break;
            }
            charAtNumber = input[v][index];
        }
        Console.WriteLine($"Found {s}");
        Console.WriteLine($"New Line ({v}): {input[v]}");
        return int.Parse(s);
    }

    /*
     * .......
     * ..X....
     * ...123.
     * ......X
     */

    private bool hasAdjacentSymbol(int lineNum, int start, int end)
    {
        var validSpaces = new HashSet<Tuple<int, int>>();
        for (var index = start; index <= end; index++)
        {
            // Check lineNum - 1 (above)
            if (lineNum > 0)
            {
                if (index > 0)
                {
                    validSpaces.Add(new Tuple<int, int>(lineNum - 1, index - 1));
                }
                validSpaces.Add(new Tuple<int, int>(lineNum - 1, index));
                if (index < _input.ElementAt(lineNum - 1).Length - 1)
                {
                    validSpaces.Add(new Tuple<int, int>(lineNum - 1, index + 1));
                }
            }
            // Check lineNum (same line) l & r
            if (index > 0)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum, index - 1));
            }
            if (index < _input.ElementAt(lineNum).Length - 1)
            {
                validSpaces.Add(new Tuple<int, int>(lineNum, index + 1));
            }

            // Check lineNum + 1 (below)
            if (lineNum < _input.Count - 1)
            {
                if (index > 0)
                {
                    validSpaces.Add(new Tuple<int, int>(lineNum + 1, index - 1));
                }
                validSpaces.Add(new Tuple<int, int>(lineNum + 1, index));
                if (index < _input.ElementAt(lineNum + 1).Length - 1)
                {
                    validSpaces.Add(new Tuple<int, int>(lineNum + 1, index + 1));
                }
            }
        }
        return validSpaces.Any(x =>
        {
            var charAtLocation = _input.ElementAt(x.Item1).ElementAt(x.Item2);
            return char.IsSymbol(charAtLocation) && charAtLocation != '.';
        });
    }

    public override ValueTask<string> Solve_1() {
        var sum = 0;
        for (int i = 0; i < _input.Count; i++)
        {
            var l = _input[i];
            for (int j = 0; j < l.Length; j++)
            {
                var c = l[j];
                if (!char.IsAsciiLetterOrDigit(c) && c != '.' && !char.IsWhiteSpace(c))
                {
                    Console.WriteLine($"Getting Numbers around {c} ({i},{j})");
                    var adjacentNumbers = GetAdjacentNumbers(_input, i, j);
                    sum += adjacentNumbers.Sum();
                }
            }
        }

        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;
        for (int i = 0; i < _input2.Count; i++)
        {
            var l = _input2[i];
            for (int j = 0; j < l.Length; j++)
            {
                var c = l[j];
                if (c == '*')
                {
                    Console.WriteLine($"Getting Numbers around {c} ({i},{j})");
                    var adjacentNumbers = GetAdjacentNumbers(_input2, i, j);
                    if (adjacentNumbers.Count() == 2)
                    {
                        sum += adjacentNumbers.ElementAt(0) * adjacentNumbers.ElementAt(1);
                    }
                }
            }
        }

        return new(sum.ToString());
    }
}
