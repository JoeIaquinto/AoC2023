namespace AoC_2023;

using System.IO;

public class Day_07 : BaseDay
{
    private readonly string _input;
    private readonly Game _handsPt1;
    private readonly Game _handsPt2;

    public Day_07()
    {
        _input = File.ReadAllText(InputFilePath);
        var inputSplit = _input.Split("\n").ToList();
        _handsPt1 = new Game(inputSplit.Select(x =>
        {
            var split = x.Split(' ').Select(x => x.Trim()).ToArray();
            return new Hand(int.Parse(split[1]), split[0].Select(y => new Card(y.ToString(), false)).ToList(), false);
        }).ToList());
        _handsPt2 = new Game(inputSplit.Select(x =>
        {
            var split = x.Split(' ').Select(x => x.Trim()).ToArray();
            return new Hand(int.Parse(split[1]), split[0].Select(y => new Card(y.ToString(), true)).ToList(), true);
        }).ToList());
    }


    public override ValueTask<string> Solve_1()
    {
        return new(_handsPt1.Winnings.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new(_handsPt2.Winnings.ToString());
    }
}

public record Game(List<Hand> Hands)
{
    public int Winnings
    {
        get
        {
            Hands.Sort(new HandComparer());
            return Hands.Select((x, i) =>
            {
                var winnings = (i + 1) * x.Bid;
                return winnings;
            }).Sum();
        }
    }
}

public class HandComparer : IComparer<Hand>
{
    public int Compare(Hand? x, Hand? y)
    {
        ArgumentNullException.ThrowIfNull(x);
        ArgumentNullException.ThrowIfNull(y);
        var diff = Hand.GetHandOrder(x) - Hand.GetHandOrder(y);
        if (diff > 0) return 1;
        if (diff < 0) return -1;
        if (diff == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                diff = (x.Cards[i] - y.Cards[i]);
                if (diff != 0)
                {
                    if (diff > 0) return 1;
                    if (diff < 0) return -1;
                }
            }
        }
        return 0;
    }
}

public record Hand : IEquatable<Hand>, IComparable<Hand>
{
    public Hand(int bid, List<Card> cards, bool jokersWild)
    {
        Bid = bid;
        Cards = cards;
        JokersWild = jokersWild;
        JokerCount = jokersWild ? Cards.Count(x => x.Value == "J") : 0;
    }

    public int JokerCount { get; private set; }

    public int Bid { get; }
    public List<Card> Cards { get; }
    public bool JokersWild { get; }

    override public string ToString()
    {
        return $"{Bid}: {string.Join(" ", Cards.Select(x => x.ToString()))}";
    }

    public static bool operator <(Hand left, Hand right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Hand left, Hand right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Hand left, Hand right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Hand left, Hand right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    public int CompareTo(Hand? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        var diff = GetHandOrder(this) - GetHandOrder(other);
        if (diff > 0) return 1;
        if (diff  < 0) return -1;
        if (diff == 0)
        {
            for (int i = 0; i < 5; i++)
            {
                diff = (this.Cards[i] - other.Cards[i]);
                if (diff != 0)
                {
                    if (diff > 0) return 1;
                    if (diff < 0) return -1;
                }
            }
        }
        return 0;
    }

    public static int GetHandOrder(Hand hand)
    {
        if (!hand.JokersWild)
        {
            var distinctCards = hand.Cards.Distinct().Count();
            var grouped = hand.Cards.GroupBy(x => x.Value);
            switch (distinctCards)
            {
                case 1:
                    return 10; // 5 of a kind
                case 2:
                    {
                        if (grouped.Any(x => x.Count() == 4))
                        {
                            return 9; // 4 of a kind
                        }
                        return 8; // Full House
                    }

                case 3:
                    {
                        if (grouped.Any(x => x.Count() == 3))
                        {
                            return 7; // 3 of a kind
                        }
                        return 6; // 2 pair
                    }
                case 4:
                    return 5; // 1 pair
                case 5:
                    return 4; // high card
                default:
                    throw new ArgumentException("Hand too big");
            }
        }
        else
        {
            var grouped = hand.Cards.GroupBy(x => x.Value);
            switch (grouped.Count())
            {
                case 1:
                    {
                        return 6;
                    }
                case 2:
                    {
                        // 2 distinct cards
                        // possible combos:
                          // 5 of a kind (e.g. K K K K J, K K K J J, K K J J J, K J J J J, J J J J J) (any jokers in the chat?)
                          // 4 of a kind (e.g. K K K K T)
                          // Full House  (e.g. K K T T T)
                        if (hand.JokerCount > 0)
                        {
                            return 6; // five of a kind
                        }
                        if (grouped.Any(group => group.Count() == 4))
                        {
                            return 5; // 4 of a kind (e.g. K K K K T)
                        }
                        return 4; // Full House  (e.g. K K T T T)
                    }
                case 3:
                    {
                        // 3 distinct cards
                        // possible combos:
                          // 4 of a kind (e.g. K K K J T, K K J J T, K J J J T)
                          // Full House  (e.g. K K J T T)
                          // 3 of a kind (e.g. K K K 1 2)
                          // 2 pair      (e.g. K K 1 T T)
                        switch (hand.JokerCount)
                        {
                            case 0:
                                {
                                    if (grouped.Any(group => group.Count() == 3))
                                    {
                                        return 3; // 3 of a kind (e.g. K K K 1 2)
                                    }
                                    return 2; // 2 pair (e.g. K K 1 T T)
                                }
                            case 1:
                                {
                                    if (grouped.Any(group => group.Count() == 3))
                                    {
                                        return 5; // 4 of a kind (e.g. K K K J T)
                                    }
                                    var pairCount = grouped.Count(group => group.Count() == 2);
                                    if (pairCount == 2)
                                    {
                                        return 4; // Full House (e.g. K K J T T)
                                    }
                                    throw new Exception("Shouldn't be here");
                                }
                            default:
                                {
                                    return 5; // 4 of a kind (e.g. K K J J T, K J J J T)
                                }
                        }
                    }
                case 4:
                    {
                        // 4 distinct cards
                        // At least 1 pair, could be 3 of a kind if there are jokers e.g. K J J Q T or  K K J T Q
                        // Otherwise K K 1 2 3
                        if (hand.JokerCount != 0)
                        {
                            return 3; // 3 of a kind
                        }
                        return 1; // One Pair
                    }
                case 5:
                    {
                        if (hand.JokerCount != 0)
                        {
                            return 1; // One Pair
                        }
                        return 0; // High Card
                    }
                default:
                    throw new ArgumentException("Hand too big");
            }
        }
    }
}
public class Card(string value, bool jokersWild) : IEquatable<Card>, IComparable<Card>
{
    override public string ToString()
    {
        return Value;
    }

    public string Value { get; set; } = value;
    public int NumericValue => Value switch
    {
        "T" => 10,
        "J" => jokersWild ? 1 : 11,
        "Q" => 12,
        "K" => 13,
        "A" => 14,
        _ => int.Parse(Value)
    };

    public int CompareTo(Card? other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return NumericValue.CompareTo(other.NumericValue);
    }

    public bool Equals(Card? other)
    {
        return other?.NumericValue == NumericValue;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is null)
        {
            return false;
        }

        throw new NotImplementedException();
    }

    public override int GetHashCode()
    {
        return NumericValue;
    }

    public static bool operator ==(Card left, Card right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Card left, Card right)
    {
        return !(left == right);
    }

    public static bool operator <(Card left, Card right)
    {
        return left is null ? right is not null : left.CompareTo(right) < 0;
    }

    public static bool operator <=(Card left, Card right)
    {
        return left is null || left.CompareTo(right) <= 0;
    }

    public static bool operator >(Card left, Card right)
    {
        return left is not null && left.CompareTo(right) > 0;
    }

    public static bool operator >=(Card left, Card right)
    {
        return left is null ? right is null : left.CompareTo(right) >= 0;
    }

    public static int operator -(Card left, Card right)
    {
        return left.NumericValue - right.NumericValue;
    }
}

