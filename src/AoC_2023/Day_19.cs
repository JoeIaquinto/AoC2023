

using Common;
using MoreLinq;
using MoreLinq.Extensions;

namespace AoC_2023;

public class Day_19 : BaseDay
{
  public IEnumerable<string> _input;
  public Dictionary<string, Workflow> _workflows;
  public IEnumerable<Part> _parts;

  public Day_19()
  {
    _input = File.ReadAllLines(InputFilePath).ToList();
    _workflows = _input.TakeWhile(x => x != "").Select(x => Workflow.Parse(x)).ToDictionary(x => x.Id.ToString(), x => x);
    _parts = _input.SkipWhile(x => x != "").Skip(1).Select(x => {
      var parts = x.Split(',');
      return new Part(int.Parse(parts[0][3..]), int.Parse(parts[1][2..]), int.Parse(parts[2][2..]), int.Parse(parts[3][2..^1]));
    });
  }

  public override ValueTask<string> Solve_1()
  {
    List<Part> acceptedParts = [];
    foreach (var part in _parts) {
      var workflow = _workflows["in"];
      var result = workflow.ProcessPart(part);
      while (!result.IsAccept && !result.IsReject) {
        workflow = _workflows[result.Result];
        result = workflow.ProcessPart(part);
      }
      if (result.IsAccept) {
        acceptedParts.Add(part);
      } else {
        continue;
      }
    }
    return new(acceptedParts.Sum(x => x.X + x.M + x.A + x.S).ToString());
  }

  public override ValueTask<string> Solve_2()
  {
    DictMultiRange<char> startRanges = new()
    {
      Ranges = new()
      {
        {'x', new(1, 4000) },
        {'m', new(1, 4000) },
        {'a', new(1, 4000) },
        {'s', new(1, 4000) }
      }
    };
    return new(GetRangeLengths(startRanges, _workflows["in"]).ToString());
  }

  private long GetRangeLengths(DictMultiRange<char> ranges, Workflow startFlow)
  {
      long validCombos = 0;
      foreach (var step in startFlow.Rules)
      {
          DictMultiRange<char> nR = new(ranges);

          if (step.Operator == '>')
          {

              if (ranges.Ranges[step.OperatesOn].End > step.Comparator) //Do we have any valid points
              {
                  //Send the valid values off to their new home
                  nR.Ranges[step.OperatesOn].Start = Math.Max(nR.Ranges[step.OperatesOn].Start, step.Comparator + 1);
                  if (step.IfTrue.IsAccept) validCombos += nR.Len;
                  else if (!step.IfTrue.IsReject) validCombos += GetRangeLengths(nR, _workflows[step.IfTrue.Result]);

                  //Take the invalid values and pass them to the next step in the workflow.
                  ranges.Ranges[step.OperatesOn].End = step.Comparator;
              }

          }
          if (step.Operator == '<')
          {
              if (ranges.Ranges[step.OperatesOn].Start < step.Comparator) //Do we have any valid points
              {
                  //Send the valid values off to their new home
                  nR.Ranges[step.OperatesOn].End = Math.Min(nR.Ranges[step.OperatesOn].End, step.Comparator - 1);
                  if (step.IfTrue.IsAccept) validCombos += nR.Len;
                  else if (!step.IfTrue.IsReject) validCombos += GetRangeLengths(nR, _workflows[step.IfTrue.Result]);

                  //Take the invalid values and pass them to the next step in the workflow.
                  ranges.Ranges[step.OperatesOn].Start = step.Comparator;
              }

          }
      }

      if (startFlow.Rules[^1].IfTrue.IsAccept)
      {
          validCombos += ranges.Len;
      }
      else if (!startFlow.Rules[^1].IfTrue.IsReject)
      {
          validCombos += GetRangeLengths(ranges, _workflows[startFlow.Rules[^1].IfTrue.Result]);
      }

      return validCombos;
  }
}

public record Workflow {

  public string Id { get; init; }
  public string Pattern { get; init; }
  public Rule[] Rules { get; init; }

  public Workflow(string id, string pattern) {
    Id = id;
    Pattern = pattern;
    Rules = Rule.Parse(pattern);
  }

  public static Workflow Parse(string input) {
    var parts = input.Split("{");
    return new Workflow(parts[0], parts[1][..^1]);
  }
};

public record Rule(char OperatesOn, char Operator, int Comparator, WorkflowResult IfTrue, WorkflowResult? IfFalse = null) {
  public static Rule[] Parse(string input) {
    var parts = input.Split(',');
    var rules = new List<Rule>();
    foreach (var part in parts) {
      if (part.All(char.IsAsciiLetter)) {
        rules.Add(new Rule(part[0], part[0], 0, new WorkflowResult(part)));
        break;
      }
      char oper = part[0];
      char op = part[1];
      var indexOfColon = part.IndexOf(':');
      int comp = int.Parse(part[2..indexOfColon]);
      var ifTrue = part[(indexOfColon + 1)..];

      var rule = new Rule(oper, op, comp, new WorkflowResult(ifTrue));
      rules.Add(rule);
    }
    return [.. rules];
  }
};

public readonly record struct WorkflowResult {
  public string Result { get; init; }
  public WorkflowResult(string result) {
    Result = result;
    IsAccept = result == "A";
    IsReject = result == "R";
  }
    public bool IsAccept { get; init; }
    public bool IsReject { get; init; }
};

public static class RuleExtensions {
  public static WorkflowResult Apply(this Rule[] rules, Part part) {
    for(var i = 0; i < rules.Length - 1; i++) {
      var rule = rules[i];

      var ruleSubject = rule.OperatesOn switch {
        'x' => part.X,
        'm' => part.M,
        'a' => part.A,
        's' => part.S,
        _ => throw new Exception("Invalid rule subject")
      };
      var ruleResult = rule.Operator switch {
        '=' => ruleSubject == rule.Comparator,
        '<' => ruleSubject < rule.Comparator,
        '>' => ruleSubject > rule.Comparator,
        _ => throw new Exception("Invalid rule operator")
      };
      if (ruleResult) {
        return rule.IfTrue;
      }
    }
    return rules[^1].IfTrue;
  }

  public static WorkflowResult ProcessPart(this Workflow workflow, Part part) {
    return workflow.Rules.Apply(part);
  }
};

public readonly record struct Part(int X, int M, int A, int S);