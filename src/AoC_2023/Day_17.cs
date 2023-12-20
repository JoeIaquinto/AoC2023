using System.Collections;
using Common;
using Coor = Common.Coor<int>;

namespace AoC_2023;

public readonly record struct State(Coor Pos, Coor Face, int Count);

public class Day_17 : BaseDay 
{

    public IEnumerable<string> _input;

    public int[,] _map;
    public Day_17()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
        _map = _input.ParseAsArray(x => int.Parse(x.ToString()));
    }

    public override ValueTask<string> Solve_1()
    {
      var start = new Coor(0, 0);
      var dest = new Coor(_map.Height() - 1, _map.Width() - 1);
      
      var heatLost = FindPath(_map, start, dest, 1, 3);
      return new(heatLost.ToString());
    }

    // Utilize A* algorithm to find path from start to dest with lowest cost (heat lost)
    // Heat lost is the sum of all values in the path
    // The path may not travel more than 3 times in the same direction
    public static int FindPath(int[,] map, Coor start, Coor dest, int constraintMin, int constraintMax) {
      var queue = new PriorityQueue<State, int>();
      var cost = new DefaultDictionary<State, int>(defaultValue: int.MaxValue / 2);
      var startRight = new State(start, Coor.Right, 0);
      queue.Enqueue(startRight, 0);
      cost[startRight] = 0;
      var startDown = new State(start, Coor.Down, 0);
      queue.Enqueue(startDown, 0);
      cost[startDown] = 0;

      while (queue.TryDequeue(out State current, out var distance))
      {
        if (current.Pos == dest && current.Count >= constraintMin) {
          return cost[current];
        }

        foreach (var neighbor in GetAdjacent(current, constraintMin, constraintMax)) {
          if (!neighbor.Pos.InBoundsOf(map)) {
            continue;
          }
          if (cost[current] + map[neighbor.Pos.Y, neighbor.Pos.X] < cost[neighbor]) {
            cost[neighbor] = cost[current] + map[neighbor.Pos.Y, neighbor.Pos.X];
            queue.Enqueue(neighbor, cost[neighbor]);
          }
        }
      }
      throw new Exception("No path found");
    }

    private static IEnumerable<State> GetAdjacent(State state, int constraintMin, int constraintMax)
    {
        if (state.Count >= constraintMin)
        {
            var l = state.Face.Rotate90DegLeft();
            var r = state.Face.Rotate90DegRight();
            
            yield return new State(Pos: state.Pos + l, Face: l, Count: 1);
            yield return new State(Pos: state.Pos + r, Face: r, Count: 1);
        }
        
        if (state.Count < constraintMax)
        {
            yield return new State(Pos: state.Pos + state.Face, Face: state.Face, Count: state.Count + 1);
        }
    }

    public override ValueTask<string> Solve_2()
    {
      var start = new Coor(0, 0);
      var dest = new Coor(_map.Height() - 1, _map.Width() - 1);
      
      var heatLost = FindPath(_map, start, dest, 4, 10);
      return new(heatLost.ToString());
    }

}

public class DefaultDictionary<TKey, TValue>(Func<TKey, TValue> defaultSelector) : IDictionary<TKey, TValue>
    where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _dictionary = new();

    public bool IsReadOnly => false;
    public int Count => _dictionary.Count;
    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;
    
    public TValue this[TKey key]
    {
        get => IndexGetInternal(key);
        set => IndexSetInternal(key, value);
    }

    public DefaultDictionary(TValue defaultValue) : this(defaultSelector: _ => defaultValue)
    {
    }

    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
    }
    
    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _dictionary[item.Key] = item.Value;
    }
    
    public bool Remove(TKey key)
    {
        return _dictionary.Remove(key);
    }
    
    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Remove(item.Key);
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }
    
    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        value = this[key];
        return true;
    }
    
    public void Clear()
    {
        _dictionary.Clear();
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, arrayIndex);
    }
    
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _dictionary.GetEnumerator();
    }

    private void IndexSetInternal(TKey key, TValue value)
    {
        _dictionary[key] = value;
    }

    private TValue IndexGetInternal(TKey key)
    {
        if (_dictionary.TryGetValue(key, out var value))
        {
            return value;
        }

        _dictionary[key] = defaultSelector.Invoke(key);
        return _dictionary[key];
    }
    
    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_dictionary).GetEnumerator();
    }
}