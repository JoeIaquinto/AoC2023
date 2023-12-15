using System.Collections;
using System.Net.Http.Headers;
using AoC_2023;
using MoreLinq;

namespace AoC_2023;

public class Day_11 : BaseDay
{
    private readonly string[] _input;
    private List<(int row, int col)> _galaxies;
    private HashSet<int> _emptyRows = [];
    private HashSet<int> _emptyCols = [];
    private readonly long EmptySpaceModifierPart2 = 1_000_000;

    public Day_11()
    {
        _input = File.ReadAllLines(InputFilePath).ToArray();
        const char GalaxyMarker = '#';
        int rowCount = _input.Length;
        int colCount = _input[0].Length;
        _galaxies = _input
            .Select((x, i) => (value: x, index: i))
            .SelectMany(row => row.value.Select((col, i) => (value: col, index: i, row.index))
                .Where(col => col.value == GalaxyMarker)
                .Select(col => (row.index, col.index))
            )
            .ToList();
        // Map out empty rows & cols for expansion
        _emptyRows = _input.Select((x, i) => (x, i)).Where(x => x.x.All(x => x == '.')).Select(x => x.i).ToHashSet();
        for (int i = 0; i < colCount; i++)
        {
            if (_input.All(x => x[i] == '.'))
            {
                _emptyCols.Add(i);
            }
        }
    }

    public override ValueTask<string> Solve_1()
    {
        var galaxyPaths = _galaxies.Subsets(2);
        var distance = galaxyPaths.Sum(x => {
            var pair = x.Select(y => (y.row, y.col)).ToArray();
            var (topLeft, bottomRight) = GetBoundingBox(pair[0], pair[1]);
            var padding = GetPadding(topLeft, bottomRight);
            var distance = Distance(pair[0], pair[1]);
            return distance + padding;
        });
        return new(distance.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var galaxyPaths = _galaxies.Subsets(2);
        var distance = galaxyPaths.Sum(x => {
            var pair = x.Select(y => (y.row, y.col)).ToArray();
            var (topLeft, bottomRight) = GetBoundingBox(pair[0], pair[1]);
            var padding = GetPadding(topLeft, bottomRight) * (EmptySpaceModifierPart2 - 1);
            var distance = Distance(pair[0], pair[1]);
            return distance + padding;
        });
        return new(distance.ToString());
    }

    private long GetPadding((int row, int col) topLeft, (int row, int col) bottomRight)
        => GetColPadding(topLeft, bottomRight) + GetRowPadding(topLeft, bottomRight);

    private long GetRowPadding((int row, int col) topLeft, (int row, int col) bottomRight)
        => _emptyRows.Count(x => x >= topLeft.row && x <= bottomRight.row);

    private long GetColPadding((int row, int col) topLeft, (int row, int col) bottomRight)
        => _emptyCols.Count(x => x >= topLeft.col && x <= bottomRight.col);

    private static long Distance((int row, int col) a, (int row, int col) b) => (Math.Abs(a.row - b.row) + Math.Abs(a.col - b.col));

    private static ((int row, int col) topLeft, (int row, int col) bottomRight) GetBoundingBox((int row, int col) a, (int row, int col) b)
        => ((Math.Min(a.row, b.row), Math.Min(a.col, b.col)), (Math.Max(a.row, b.row), Math.Max(a.col, b.col)));
}
