namespace AoC_2023;

using MoreLinq;

public class Day_08 : BaseDay
{
    private readonly char[] _directions;
    private readonly IEnumerable<string> _input;
    private Dictionary<string, GraphNode> _nodes;

    public Day_08()
    {
        _input = File.ReadAllLines(InputFilePath);
        _directions = _input.First().ToCharArray();
        _nodes = _input.Skip(2).Select(x => {
            var id = x[..3];
            var leftNode = x[^9..^6];
            var rightNode = x[^4..^1];
            return new GraphNode(id, leftNode, rightNode);
        }).ToDictionary(x => x.Id);
    }

    public override ValueTask<string> Solve_1() {
        var steps = GetStepsFromStartingPositionToCondition(_nodes, _directions, _nodes["AAA"], x => x.Id == "ZZZ");
        
        return new(steps.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var startingIds = _nodes.Where(x => x.Key.EndsWith('A')).Select(x => x.Value).ToArray();
        var steps = Lcm(startingIds.Select(x => GetStepsFromStartingPositionToCondition(_nodes, _directions, x, y => y.Id.EndsWith('Z'))).ToArray());
        return new(steps.ToString());
    }

    static long Lcm(long[] numbers)
    {
        return numbers.Aggregate((x, y) => x * y / Gcd(x, y));
    }

    static long Gcd(long a, long b)
    {
        if (b == 0)
            return a;
        return Gcd(b, a % b);
    }

    public static long GetStepsFromStartingPositionToCondition(Dictionary<string, GraphNode> nodes, char[] directions, GraphNode start, Func<GraphNode, bool> endSelector)
    {
        var currentId = start;
        long steps = 0;
        while (!endSelector(currentId))
        {
            var direction = directions[steps % directions.Length];
            var nextId = direction switch
            {
                'L' => nodes[currentId.LeftId],
                'R' => nodes[currentId.RightId],
                _ => throw new Exception("Invalid direction")
            } ?? throw new Exception($"Invalid path at step {steps}");
            currentId = nextId;
            steps++;
        }
        return steps;
    }
}

public record GraphNode(string Id, string LeftId, string RightId);
