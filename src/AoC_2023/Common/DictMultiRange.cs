namespace Common;

public class Range
{
  public long Start;
  public long End;
  public long Len => End - Start + 1;

  public Range(long Start, long End) 
  {
    this.Start = Start;
    this.End = End;
  }

  //Forced Deep Copy
  public Range(Range other)
  {
    Start = other.Start;
    End = other.End;
  }

  public override string ToString()
  {
    return $"[{Start}, {End}] ({Len})";
  }
}

public class MultiRange
{
  public List<Range> Ranges = [];

  public MultiRange() { }

  public MultiRange(IEnumerable<Range> Ranges)
  {
    this.Ranges = new(Ranges);
  }

  public MultiRange(MultiRange other)
  {
    foreach (var r in other.Ranges)
    {
      Range n = new(r);
      Ranges.Add(n);
    }
  }

  public long len => Ranges.Aggregate(1L, (a, b) => a *= b.Len);
}

public class DictMultiRange<T> where T : notnull
{
  public Dictionary<T, Range> Ranges = [];

  public DictMultiRange() { }

  public DictMultiRange(DictMultiRange<T> other)
  {
      foreach (var r in other.Ranges)
      {
          Range n = new(r.Value);
          Ranges[r.Key] = n;
      }
  }

  public long Len => Ranges.Aggregate(1L, (a, b) => a *= b.Value.Len);
}