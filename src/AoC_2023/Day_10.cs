﻿global using Coordinate = (int Y, int X);

using AoC_2023;

namespace AoC_2023;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using MoreLinq;
using Spectre.Console;


public enum Direction
{
    NORTH = 0, WEST = 1, SOUTH = 2, EAST = 3
}

public enum PipeChar
{
    NS = '|',
    EW = '-',
    NE = 'L',
    NW = 'J',
    SW = '7',
    SE = 'F',
    GROUND = '.',
    START = 'S'
}

public class Day_10 : BaseDay
{
    private readonly string[] _input;

    private readonly Dictionary<string, Pipe> _pipeTypes = new Dictionary<string, Pipe>
    {
        ["|"] = new Pipe(0, -1, 0, 1),
        ["-"] = new Pipe(-1, 0, 1, 0),
        ["L"] = new Pipe(0, -1, 1, 0),
        ["J"] = new Pipe(0, -1, -1, 0),
        ["7"] = new Pipe(-1, 0, 0, 1),
        ["F"] = new Pipe(1, 0, 0, 1),
        ["."] = new Pipe(0, 0, 0, 0),
    };

    public Day_10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    static int BFS(int[,] grid)
    {
        int height = grid.GetLength(0);
        int width = grid.GetLength(1);

        var visited = new bool[height, width];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                visited[y, x] = grid[y, x] == 1;
            }
        }

        Queue<Point> queue = new Queue<Point>();
        queue.Enqueue(new Point(0, 0));
        queue.Enqueue(new Point(0, height - 1));
        queue.Enqueue(new Point(width - 1, 0));
        queue.Enqueue(new Point(width - 1, height - 1));

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (current.X < 0 || current.X >= width || current.Y < 0 || current.Y >= height || visited[current.Y, current.X])
            {
                continue;
            }

            visited[current.Y, current.X] = true;
            queue.Enqueue(new Point(current.X - 1, current.Y));
            queue.Enqueue(new Point(current.X + 1, current.Y));
            queue.Enqueue(new Point(current.X, current.Y - 1));
            queue.Enqueue(new Point(current.X, current.Y + 1));
        }

        var totalUnvisited = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var cellVisited = visited[y, x];

                if (!cellVisited)
                {
                    totalUnvisited++;
                }

                grid[y, x] = cellVisited ? 1 : 0;

            }
        }

        return totalUnvisited;
    }

    static int[,] ScaleUp(string[,] input, int resizeFactor, Dictionary<string, Pipe> pipeTypes)
    {
        var resized = new int[input.GetLength(0) * resizeFactor, input.GetLength(1) * resizeFactor];

        for (int x = 0; x < resized.GetLength(0); x++)
        {
            for (int y = 0; y < resized.GetLength(1); y++)
            {
                resized[x, y] = 0;
            }
        }

        for (int x = 0; x < input.GetLength(0); x++)
        {
            for (int y = 0; y < input.GetLength(1); y++)
            {
                if (input[x, y] == null) continue;

                var pipe = pipeTypes[input[x, y]];

                for (int i = 0; i < resizeFactor; i++)
                {
                    resized[(x * resizeFactor) + (pipe.AY * i), (y * resizeFactor) + (pipe.AX * i)] = 1;
                }

                for (int i = 0; i < resizeFactor; i++)
                {
                    resized[(x * resizeFactor) + (pipe.BY * i), (y * resizeFactor) + (pipe.BX * i)] = 1;
                }
            }
        }

        return resized;
    }

    static int[,] ScaleDown(int[,] input, int shrinkFactor)
    {
        var resized = new int[input.GetLength(0) / shrinkFactor, input.GetLength(1) / shrinkFactor];

        for (int x = 0; x < resized.GetLength(0); x++)
        {
            for (int y = 0; y < resized.GetLength(1); y++)
            {
                resized[x, y] = 0;
            }
        }

        for (int x = 0; x < resized.GetLength(0); x++)
        {
            for (int y = 0; y < resized.GetLength(1); y++)
            {
                resized[x, y] = input[x * shrinkFactor, y * shrinkFactor];
            }
        }

        return resized;
    }

    static bool CanConnect(int originX, int originY, int targetX, int targetY, Pipe pipe)
    {
        var sideACanConnect = (targetX + pipe.AX) == originX && (targetY + pipe.AY) == originY;
        var sideBCanConnect = (targetX + pipe.BX) == originX && (targetY + pipe.BY) == originY;
        return sideACanConnect || sideBCanConnect;
    }

    public override ValueTask<string> Solve_1()
    {
        var grid = new string[_input.Length, _input[0].Length];
        var start = new Point();
        for (int y = 0; y < _input.Length; y++)
        {
            var line = _input[y].ToCharArray().Select(char.ToString).ToArray();

            for (int x = 0; x < line.Length; x++)
            {
                grid[y, x] = line[x];

                if (line[x] == "S")
                {
                    start = new Point(x, y);
                }
            }
        }

        var potentials = new List<Point>
        {
            new(start.X - 1, start.Y),
            new(start.X + 1, start.Y),
            new(start.X, start.Y - 1),
            new(start.X, start.Y + 1)
        };

        var current = potentials.First(x => CanConnect(start.X, start.Y, x.X, x.Y, _pipeTypes[grid[x.X, x.Y]]));

        var previous = start;
        long steps = 0;

        var path = new int[grid.GetLength(0), grid.GetLength(1)];
        var pathStr = new string[grid.GetLength(0), grid.GetLength(1)];

        path[start.Y, start.X] = 1;
        pathStr[start.Y, start.X] = "7";

        while (current.X != start.X || current.Y != start.Y)
        {
            var pipe = _pipeTypes[grid[current.Y, current.X]];

            path[current.Y, current.X] = 1;
            pathStr[current.Y, current.X] = grid[current.Y, current.X];

            Point next;

            if ((current.X + pipe.AX) == previous.X && (current.Y + pipe.AY) == previous.Y)
            {
                next = new Point(current.X + pipe.BX, current.Y + pipe.BY);
            }
            else
            {
                next = new Point(current.X + pipe.AX, current.Y + pipe.AY);
            }

            previous = current;

            current = next;

            steps++;
        }
        return new(Math.Ceiling((float)steps / 2).ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var grid = new string[_input.Length, _input[0].Length];
        var start = new Point();
        for (int y = 0; y < _input.Length; y++)
        {
            var line = _input[y].ToCharArray().Select(char.ToString).ToArray();

            for (int x = 0; x < line.Length; x++)
            {
                grid[y, x] = line[x];

                if (line[x] == "S")
                {
                    start = new Point(x, y);
                }
            }
        }

        var potentials = new List<Point>
        {
            new(start.X - 1, start.Y),
            new(start.X + 1, start.Y),
            new(start.X, start.Y - 1),
            new(start.X, start.Y + 1)
        };

        Point current = new Point();
        for (int i = 0; i < potentials.Count; i++)
        {
            var potential = potentials[i];
            if (CanConnect(start.X, start.Y, potential.X, potential.Y, _pipeTypes[grid[potential.Y, potential.X]]))
            {
                current = potential;
                break;
            }
        }

        var previous = start;
        long steps = 0;

        var path = new int[grid.GetLength(0), grid.GetLength(1)];
        var pathStr = new string[grid.GetLength(0), grid.GetLength(1)];

        path[start.Y, start.X] = 1;
        pathStr[start.Y, start.X] = "7";

        while (current.X != start.X || current.Y != start.Y)
        {
            var pipe = _pipeTypes[grid[current.Y, current.X]];

            path[current.Y, current.X] = 1;
            pathStr[current.Y, current.X] = grid[current.Y, current.X];

            Point next;

            if ((current.X + pipe.AX) == previous.X && (current.Y + pipe.AY) == previous.Y)
            {
                next = new Point(current.X + pipe.BX, current.Y + pipe.BY);
            }
            else
            {
                next = new Point(current.X + pipe.AX, current.Y + pipe.AY);
            }

            previous = current;

            current = next;

            steps++;
        }
        var resizeFactor = 3;
        var resizedGrid = ScaleUp(pathStr, resizeFactor, _pipeTypes);


        var totalContained = BFS(resizedGrid) / resizeFactor;

        var scaledDown = ScaleDown(resizedGrid, resizeFactor);

        var totalUnvisited = 0;

        for (int y = 0; y < scaledDown.GetLength(0); y++)
        {
            for (int x = 0; x < scaledDown.GetLength(1); x++)
            {
                if (scaledDown[y, x] == 0)
                {
                    totalUnvisited++;
                }
            }
        }
        Console.WriteLine($"Steps: {Math.Ceiling((float)steps / 2)}");
        Console.WriteLine($"Total Contained: {totalUnvisited}");
        return new(totalUnvisited.ToString());
    }

}

public struct Pipe
{
    public readonly int AX;
    public readonly int AY;
    public readonly int BX;
    public readonly int BY;

    public Pipe(int ax, int ay, int bx, int by)
    {
        AX = ax;
        AY = ay;
        BX = bx;
        BY = by;
    }
}
