namespace AoC_2023;

using System.Text.RegularExpressions;
using MoreLinq;
using MoreLinq.Extensions;

public class Day_02 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day_02()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1() {
        var gameIdsSum = _input.Sum(x =>
        {
            var gameId = Regex.Match(x, @"Game\s(\d+)").Groups[1].Value;
            var blues = Regex.Matches(x, @"(\d+) blue").Select(b => int.Parse(b.Groups[1].Value));
            var red = Regex.Matches(x, @"(\d+) red").Select(b => int.Parse(b.Groups[1].Value));
            var green = Regex.Matches(x, @"(\d+) green").Select(b => int.Parse(b.Groups[1].Value));
            var isPossible = blues.All(b => b <= 14) && red.All(b => b <= 12) && green.All(b => b <= 13);
            if (isPossible)
            {
                return int.Parse(gameId!);
            }
            return 0;
        });
        return new(gameIdsSum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var powersSum = _input.Sum(x =>
        {
            var pulls = x[x.IndexOf(":")..].Split(";");
            var colorsByPull = pulls.Select(p =>
            {
                var blues = Regex.Matches(p, @"(\d+) blue").Select(b => int.Parse(b.Groups[1].Value)).Sum();
                var reds = Regex.Matches(p, @"(\d+) red").Select(b => int.Parse(b.Groups[1].Value)).Sum();
                var greens = Regex.Matches(p, @"(\d+) green").Select(b => int.Parse(b.Groups[1].Value)).Sum();
                return (blues, reds, greens);
            });
            var minGreens = colorsByPull.Max(p => p.greens);
            var minBlues = colorsByPull.Max(p => p.blues);
            var minReds = colorsByPull.Max(p => p.reds);
            return minGreens * minBlues * minReds;

        });
        return new(powersSum.ToString());
    }
}
