
using System;
using System.Collections.Immutable;
using System.Linq;
using MoreLinq;
using Cache = System.Collections.Generic.Dictionary<(string, System.Collections.Immutable.ImmutableStack<int>), long>;
namespace AoC_2023;

public readonly struct Springs {
    public readonly string springMap;
    public readonly int[] springGroups;

    public Springs(string springMap, int[] springGroups) : this()
    {
        this.springMap = springMap;
        this.springGroups = springGroups;
    }
}

public class Day_12 : BaseDay
{
    private readonly string[] _input;
    private readonly Springs[] _springs;
    private readonly Springs[] _springsPt2;

    public Day_12()
    {
        _input = [.. File.ReadAllLines(InputFilePath)];
        _springs = _input.Select(x =>
        {
            var split = x.Split(' ');
            return new Springs(split[0], split[1].Split(',').Select(int.Parse).ToArray());
        }).ToArray();
        _springsPt2 = _input.Select(x =>
        {
            var split = x.Split(' ');
            return new Springs(Repeat(split[0], 5, '?'), Repeat(split[1], 5, ',').Split(',').Select(int.Parse).ToArray());
        }).ToArray();
    }

    private static string Repeat(string s, int n, char separator) => string.Join(separator, Enumerable.Repeat(s, n));

    public override ValueTask<string> Solve_1()
    {
        long sum = _springs.Sum(x =>
        {
            return Compute(x.springMap, ImmutableStack.CreateRange(x.springGroups.Reverse().ToArray()), []);
        });
        return new(sum.ToString());
    }


    public override ValueTask<string> Solve_2()
    {
        long sum = _springsPt2.Sum(x =>
        {
            var values = Compute(x.springMap, ImmutableStack.CreateRange(x.springGroups.Reverse().ToArray()), []);
            return values;
        });
        return new(sum.ToString());
    }

    private static long Compute(string pattern, ImmutableStack<int> nums, Cache cache)
    {
        if (!cache.ContainsKey((pattern, nums)))
        {
            cache[(pattern, nums)] = Dispatch(pattern, nums, cache);
        }
        return cache[(pattern, nums)];
    }

    private static long Dispatch(string pattern, ImmutableStack<int> nums, Cache cache) {
        return pattern.FirstOrDefault() switch {
            '.' => ProcessDot(pattern, nums, cache),
            '?' => ProcessQuestion(pattern, nums, cache),
            '#' => ProcessHash(pattern, nums, cache),
            _ => ProcessEnd(pattern, nums, cache),
        };
    }

    private static long ProcessEnd(string _, ImmutableStack<int> nums, Cache __) {
        // the good case is when there are no numbers left at the end of the pattern
        if (nums.IsEmpty) {
            return 1;
        }
        return 0;
    }

    private static long ProcessDot(string pattern, ImmutableStack<int> nums, Cache cache) {
        // consume one spring and recurse
        return Compute(pattern[1..], nums, cache);
    }

    private static long ProcessQuestion(string pattern, ImmutableStack<int> nums, Cache cache) {
        // recurse both ways
        return Compute("." + pattern[1..], nums, cache) + Compute("#" + pattern[1..], nums, cache);
    }

    private static long ProcessHash(string pattern, ImmutableStack<int> nums, Cache cache) {
        // take the first number and consume that many dead springs, recurse

        if (nums.IsEmpty) {
            return 0; // no more numbers left, this is no good
        }

        var n = nums.Peek();
        nums = nums.Pop();

        var potentiallyDead = pattern.TakeWhile(s => s == '#' || s == '?').Count();

        if (potentiallyDead < n) {
            return 0; // not enough dead springs 
        } else if (pattern.Length == n) {
            return Compute("", nums, cache);
        } else if (pattern[n] == '#') {
            return 0; // dead spring follows the range -> not good
        } else {
            return Compute(pattern[(n + 1)..], nums, cache);
        }
    }
}
