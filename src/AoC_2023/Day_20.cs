using MoreLinq;

namespace AoC_2023;

public class Day_20 : BaseDay
{
  public IEnumerable<string> _input;
  public DefaultDictionary<string, IModule> _modules;
  public DefaultDictionary<string, IModule> _modules2;

  public Day_20()
    {
        _input = File.ReadAllLines(InputFilePath).ToList();
        _modules = SetModules();
        _modules2 = SetModules();

        DefaultDictionary<string, IModule> SetModules()
        {
            var modules = new DefaultDictionary<string, IModule>(x => new Output(x, []));
            foreach (var line in _input)
            {
                var parts = line.Split("->");
                var type = parts[0][0];
                var id = type == 'b' ? parts[0].Trim() : parts[0][1..].Trim();
                var outputs = parts[1].Trim().Split(',').Select(x => x.Trim()).ToArray();
                modules[id] = type switch
                {
                    '%' => new FlipFlop(id, outputs),
                    '&' => new Conjunction(id, outputs),
                    'b' => new Broadcaster(id, outputs),
                    _ => throw new Exception("Unknown module type"),
                };
            }
            var moduleKeys = modules.Keys.ToList();
            foreach (var moduleKey in moduleKeys)
            {
                var module = modules[moduleKey];
                foreach (var output in module.Outputs)
                {
                    // Initialize Conjunction module
                    if (modules.TryGetValue(output, out var value) && value.Type == ModuleType.Conjunction)
                    {
                        var conjunction = (Conjunction)value;
                        conjunction.Inputs[moduleKey] = false;
                    }
                }
            }
            modules["button"] = new Button("button", ["broadcaster"]);
            return modules;
        }
    }

    public override ValueTask<string> Solve_1()
  {
    long lowPulses = 0;
    long highPulses = 0; 
    var button = (Button)_modules["button"];
    var globalQueue = new Queue<Pulse>();
    for(int i = 0; i < 1000; i++) {
      button.Press().ForEach(globalQueue.Enqueue);
      lowPulses += 1;
    }
    var pulseQueue = new Queue<Pulse>();
    while (globalQueue.TryDequeue(out var buttonPulse)) {
      pulseQueue.Enqueue(buttonPulse);
      while(pulseQueue.TryDequeue(out var pulse)) {
        var module = _modules[pulse.Destination];
        var newPulses = module.ProcessPulse(pulse);
        var newHighPulses = newPulses.Count(x => x.IsHigh);
        var newLowPulses = newPulses.Count(x => !x.IsHigh);
        highPulses += newHighPulses;
        lowPulses += newLowPulses;
        //newPulses.ForEach(x => Console.WriteLine($"{x.Source} -{(x.IsHigh ? "High" : "Low")}-> {x.Destination}"));
        newPulses.ForEach(pulseQueue.Enqueue);
      }
    }


    return new((lowPulses * highPulses).ToString());
  }

  public override ValueTask<string> Solve_2()
  {
    long buttonPulseCount = 0;
    var button = (Button)_modules2["button"];
    var pulseQueue = new Queue<Pulse>();

    var parentsOfRx = _modules.Where(x => x.Value.Outputs.Contains("rx")).Select(x => x.Key).ToList();
    var grandParentsOfRx = _modules.Where(x => parentsOfRx.Any(y => x.Value.Outputs.Contains(y))).Select(x => x.Key).ToList();

    Dictionary<string, long> cycles = [];
    
    while (true) {
      var buttonPulse = button.Press();
      buttonPulseCount += 1;
      pulseQueue.Enqueue(buttonPulse[0]);
      while(pulseQueue.TryDequeue(out var pulse)) {

        // Track cycles of high pulces to parents of rx
        if (parentsOfRx.Contains(pulse.Destination) && pulse.IsHigh) {
          cycles.TryAdd(pulse.Source, buttonPulseCount);
          // If all grandparents of RX are in cycles, we have all of the cycles it takes to calculate the resultant cycle count, by multiplying them together
          if (grandParentsOfRx.All(cycles.ContainsKey)) {
            return new (cycles.Values.Aggregate(1L, (acc, val) => acc * val).ToString());
          }
        }

        var module = _modules2[pulse.Destination];
        var newPulses = module.ProcessPulse(pulse);
        var newHighPulses = newPulses.Count(x => x.IsHigh);
        var newLowPulses = newPulses.Count(x => !x.IsHigh);

        //newPulses.ForEach(x => Console.WriteLine($"{x.Source} -{(x.IsHigh ? "High" : "Low")}-> {x.Destination}"));

        newPulses.ForEach(pulseQueue.Enqueue);
      }
    }
  }
}

public enum ModuleType {
  FlipFlop,
  Conjunction,
  Broadcaster,
  Output,
  Button,
}

public interface IModule {
  public string Id { get; init; }
  public ModuleType Type { get; }
  public string[] Outputs { get; init; }

  public Pulse[] ProcessPulse(Pulse pulse);
}

public record struct FlipFlop(string Id, string[] Outputs) : IModule {
  public readonly ModuleType Type => ModuleType.FlipFlop;

  public readonly string Id { get; init; } = Id;
  public readonly string[] Outputs { get; init; } = Outputs;

  private bool _state = false;
  public readonly bool State => _state;

  public Pulse[] ProcessPulse(Pulse pulse) {
    if(!pulse.IsHigh) {
      var newState = !_state;
      _state = newState;
      var source = Id;
      return Outputs.Select(x => new Pulse(newState, source, x)).ToArray();
    }
    return [];
  }
}

public readonly struct Conjunction(string Id, string[] Outputs) : IModule {
    public readonly ModuleType Type => ModuleType.Conjunction;

    public readonly string Id { get; init; } = Id;
    public readonly string[] Outputs { get; init; } = Outputs;
    public readonly bool IsHigh => !Inputs.All(x => x.Value);
    public readonly DefaultDictionary<string, bool> Inputs = new(x => false);

  public Pulse[] ProcessPulse(Pulse pulse) {
    Inputs[pulse.Source] = pulse.IsHigh;
    var sendPulse = IsHigh;
    var source = Id;
    return Outputs.Select(x => new Pulse(sendPulse, source, x)).ToArray();
  }
}

public readonly struct Broadcaster(string Id, string[] Outputs) : IModule {
  public readonly ModuleType Type => ModuleType.Broadcaster;

  public readonly string Id { get; init; } = Id;
  public readonly string[] Outputs { get; init; } = Outputs;

  public Pulse[] ProcessPulse(Pulse pulse) {
    var source = Id;
    return Outputs.Select(x => new Pulse(pulse.IsHigh, source, x)).ToArray();
  }
}

public readonly struct Button(string Id, string[] Outputs) : IModule {
  public readonly ModuleType Type => ModuleType.Button;

  public readonly string Id { get; init; } = Id;
  public readonly string[] Outputs { get; init; } = Outputs;

  public Pulse[] ProcessPulse(Pulse pulse) {
    throw new NotImplementedException();
  }

  public Pulse[] Press() {
    var source = Id;
    return Outputs.Select(x => new Pulse(false, source, x)).ToArray();
  }
}

public readonly struct Output(string Id, string[] Outputs) : IModule {
  public readonly ModuleType Type => ModuleType.Output;

  public readonly string Id { get; init; } = Id;
  public readonly string[] Outputs { get; init; } = Outputs;

  public Pulse[] ProcessPulse(Pulse pulse) {
    return [];
  }
}

public readonly record struct Pulse(bool IsHigh, string Source, string Destination);