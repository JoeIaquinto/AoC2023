namespace AoC_2023;

using System.IO;
using System.Text.RegularExpressions;
using MoreLinq;
using MoreLinq.Extensions;

public class Day_06 : BaseDay
{
    private readonly string _input;
    private readonly List<(long, long)> _races = [];

    public Day_06()
    {
        _input = File.ReadAllText(InputFilePath);
        var inputSplit = _input.Split("\n").ToList();
        var times = Regex.Matches(inputSplit[0], @"\d+").Select(x => long.Parse(x.Value)).ToList();
        var distances = Regex.Matches(inputSplit[1], @"\d+").Select(x => long.Parse(x.Value)).ToList();
        for ( var i = 0; i < times.Count; i++)
        {
            _races.Add((times[i], distances[i]));
        }
    }


    public override ValueTask<string> Solve_1()
    {
        var results = _races.Select(race =>
        {
            return Enumerable.Range(0, (int)race.Item1).Where(x => WillBeatDistance(x, race.Item1, race.Item2)).Count();
        });
        var ret = results.Aggregate(1, (a, b) => a * b);
        return new(ret.ToString());
    }

    private static bool WillBeatDistance(long timeHeld, long totalTime, long distance) {
        var timeRemaining = totalTime - timeHeld;
        var speed = timeHeld;
        var distanceTraveled = timeRemaining * speed;
        return distanceTraveled > distance;
    }
       

    public override ValueTask<string> Solve_2()
    {
        var race = (
            52947594,
            426137412791216
            );
        var range = Enumerable.Range(0, race.Item1);
        var ret = range.Where(x => WillBeatDistance(x, race.Item1, race.Item2)).Count();
        return new(ret.ToString());
    }
}
