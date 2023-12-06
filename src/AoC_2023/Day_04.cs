namespace AoC_2023;

using System.Text.RegularExpressions;
using MoreLinq;

public class CardInfo
{
    public int CardId { get; set; }
    public HashSet<int> WinningNumbers { get; set; }
    public HashSet<int> HaveNumbers { get; set; }
    public HashSet<int> SharedNumbers { get; set; }

    public CardInfo(int cardId, HashSet<int> winningNums, HashSet<int> haveNums, HashSet<int> sharedNumbers)
    {
        this.CardId = cardId;
        this.WinningNumbers = winningNums;
        this.HaveNumbers = haveNums;
        this.SharedNumbers = sharedNumbers;
    }
}

public partial class Day_04 : BaseDay
{
    private readonly IEnumerable<string> _input;
    private readonly IEnumerable<Tuple<int, HashSet<int>, HashSet<int>, HashSet<int>, int?>> _scratchOffs;
    private readonly Dictionary<int, CardInfo> _cards;
    private readonly Dictionary<int, int?> _treeMemo;

    public Day_04()
    {
        _input = File.ReadAllLines(InputFilePath);
        _cards = _input.Select(x =>
        {
            var cardNum = int.Parse(CardNumRegex().Match(x).Groups[1].Value);
            var split = x.Split(':', '|');
            var winningNums = Regex.Matches(split[1], @"\d+").Select(y => int.Parse(y.Value)).ToHashSet();
            var haveNums = Regex.Matches(split[2], @"\d+").Select(y => int.Parse(y.Value)).ToHashSet();
            var shared = winningNums.Intersect(haveNums).ToHashSet();
            return new CardInfo(cardNum, winningNums, haveNums, shared);
        }).ToDictionary(x => x.CardId, x => x);
    }

    public override ValueTask<string> Solve_1() {
        var sum = _cards.Select(x =>
        {
            var intersect = x.Value.SharedNumbers.Count;
            return Math.Floor(Math.Pow(2, intersect - 1));
        }).Sum();
        return new(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var cardInstances = new int[_cards.Count];
        Array.Fill(cardInstances, 1);
        for (int i = 0; i < _cards.Count; i++)
        {
            var card = _cards[i+1];
            var instances = cardInstances[i];
            var shared = card.SharedNumbers.Count;
            for (var j = 1; j <= shared; j++)
            {
                cardInstances[i + j] += instances;
            }
        }
        return new(cardInstances.Sum().ToString());
    }

    [GeneratedRegex(@"Card\s+(\d+)")]
    private static partial Regex CardNumRegex();
}
