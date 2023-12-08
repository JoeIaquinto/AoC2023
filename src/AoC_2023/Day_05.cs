namespace AoC_2023;

using System.IO;
using MoreLinq;
using MoreLinq.Extensions;

public class AlmanacDict(List<string> almanacMappingLines)
{
    public List<AlmanacMappingLine> almanacMappingLines = [.. almanacMappingLines.Select((x, i) => new AlmanacMappingLine(x, i)).OrderBy(x => x.Source)];


    public long GetResult(long value)
    {
        return almanacMappingLines.Where(x =>
        {
            return value >= x.Source && value < x.Source + x.Length;
        })
        .Select(x => (x.Destination) + (value - x.Source))
        .SingleOrDefault(value);
    }

    private bool Overlap(AlmanacMappingLine x, long start, long end)
    {
        throw new NotImplementedException();
    }
}

public record AlmanacMappingLine
{
    public AlmanacMappingLine(string x, int id)
    {
        var split = x.Split(" ");
        Source = long.Parse(split[1]);
        Destination = long.Parse(split[0]);
        Length = long.Parse(split[2]);
    }
    public int Id { get; set; }
    public long Source { get; set; }
    public long Destination { get; set; }
    public long Length { get; set; }
}

public class Day_05 : BaseDay
{
    private readonly string _input;
    private readonly List<long> _seeds;

    private readonly Dictionary<string, AlmanacDict > keyToAlmanacDict = new Dictionary<string, AlmanacDict>()
    {
        //{ "seed-to-soil map:", new ()},
        //{ "soil-to-fertilizer map:", new ()},
        //{ "fertilizer-to-water map:", new ()},
        //{ "water-to-light map:", new ()},
        //{ "light-to-temperature map:", new ()},
        //{ "temperature-to-humidity map:", new ()},
        //{ "humidity-to-location map:", new ()},
    };

    public Day_05()
    {
        _input = File.ReadAllText(InputFilePath);
        var inputSplit = _input.Split("\n").ToList();
        _seeds = inputSplit[0][6..].Split(" ").Where(x => x.Length > 0).Select(x => long.Parse(x)).ToList();

        List<string> linesInGroup = [];
        var header = string.Empty;
        foreach (var line in inputSplit[2..])
        {
            if (line.Length == 0)
            {
                keyToAlmanacDict[header] = new AlmanacDict(linesInGroup);
                linesInGroup = [];
                continue;
            }
            if (char.IsLetter(line[0]))
            {
                header = line;
            }
            else
            {
                linesInGroup.Add(line);
            }
        }
        keyToAlmanacDict[header] = new AlmanacDict(linesInGroup);

    }


    public override ValueTask<string> Solve_1() {
        var minLocation = _seeds.Min(x =>
        {
            var soil = keyToAlmanacDict["seed-to-soil map:"].GetResult(x);
            var fertilizer = keyToAlmanacDict["soil-to-fertilizer map:"].GetResult(soil);
            var water = keyToAlmanacDict["fertilizer-to-water map:"].GetResult(fertilizer);
            var light = keyToAlmanacDict["water-to-light map:"].GetResult(water);
            var temperature = keyToAlmanacDict["light-to-temperature map:"].GetResult(light);
            var humidity = keyToAlmanacDict["temperature-to-humidity map:"].GetResult(temperature);
            return keyToAlmanacDict["humidity-to-location map:"].GetResult(humidity);
        });
        return new(minLocation.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        throw new NotImplementedException();
    }
}
