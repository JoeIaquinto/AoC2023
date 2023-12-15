namespace AoC_2023;

using MoreLinq;

public class Day_09 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day_09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public static long ExtrapolateSequenceForwards(IEnumerable<long> input)
    {
        if (input.All(x => x == 0))
        {
            return 0;
        }
        var chunked = input.WindowLeft(2).Where(x => x.Count > 1);
        var diffs = chunked.Select(x => x.Last() - x.First()).ToArray();
        var result = ExtrapolateSequenceForwards(diffs);
        return input.Last() + result;
    }

    public static long ExtrapolateSequenceBackwards(IEnumerable<long> input)
    {
        if (input.All(x => x == 0))
        {
            return 0;
        }
        var chunked = input.WindowLeft(2).Where(x => x.Count > 1);
        var diffs = chunked.Select(x => x.Last() - x.First()).ToArray();
        var result = ExtrapolateSequenceBackwards(diffs);
        return input.First() - result;
    }

    public override ValueTask<string> Solve_1() {
        var sum = _input.Select(x => x.Split(' ').Select(y => long.Parse(y))).ToList().Select(x => ExtrapolateSequenceForwards(x)).Sum();
        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = _input.Select(x => x.Split(' ').Select(y => long.Parse(y))).ToList().Select(x => ExtrapolateSequenceBackwards(x)).Sum();
        return new(sum.ToString());
    }
}
