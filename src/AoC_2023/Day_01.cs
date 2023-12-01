namespace AoC_2023;

using MoreLinq;

public class Day_01 : BaseDay
{
    private readonly IEnumerable<string> _input;

    public Day_01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private List<string> valid = new List<string>() { 
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine"
    };

    private int parseFirstDigit(string input)
    {
        var i = 0;
        foreach (var letter in input)
        {
            if (char.IsDigit(letter))
            {
                return int.Parse(letter.ToString());
            }
            if (char.IsLetter(letter))
            {
                var restOfLine = input[i..];
                var spelledOut = valid.FirstOrDefault(x => restOfLine.StartsWith(x));
                if (spelledOut != null)
                {
                    return valid.IndexOf(spelledOut) + 1;
                }
            }
            i++;
        }
        throw new ArgumentException();
    }

    private int parseLastDigit(string input)
    {
        for (int j = input.Length - 1; j >= 0; j--)
        {
            var letter = input[j];
            if (char.IsDigit(letter))
            {
                return int.Parse(letter.ToString());
            }
            if (char.IsLetter(letter))
            {
                var restOfLine = input[j..];
                var spelledOut = valid.FirstOrDefault(x => restOfLine.StartsWith(x));
                if (spelledOut != null)
                {
                    return valid.IndexOf(spelledOut) + 1;
                }
            }
        }
        throw new ArgumentException();
    }

    public override ValueTask<string> Solve_1() {
        var select = _input.Sum(x =>
        {
            var tens = int.Parse(x.First(y =>  char.IsDigit(y)).ToString()) * 10;
            var ones = int.Parse(x.Last(y => char.IsDigit(y)).ToString());
            return tens + ones;
        });
        return new(select.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var select = _input.Sum(x =>
        {
            var tens = parseFirstDigit(x) * 10;
            var ones = parseLastDigit(x);
            return tens + ones;
        });
        return new(select.ToString());
    }
}
