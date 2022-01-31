using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Cell
{
    public enum Direction
    {
        Default,
        North,
        South,
        East,
        West,
    }

    public int Row { get; private set; }
    public int Column { get; private set; }
    public Dictionary<Direction, Cell> neighbours = new Dictionary<Direction, Cell>();
    public List<Cell> Links { get; private set; }
    public bool Visited { get; set; } = false;

    public Tile Tile { get; set; }

    public bool IsLinked(Cell cell)
    {
        if (cell != null)
        {
            foreach (Cell c in Links)
            {
                if (c != null)
                {
                    if (c == cell)
                    {
                        return true;
                    }
                }
            }

        }

        return false;
    }

    public bool IsLinked(Direction direction)
    {
        if (direction != Direction.Default)
        {
            if (neighbours.ContainsKey(direction))
            {
                if (Links.Contains(neighbours[direction]))
                {
                    return true;
                }
            }
        }

        return false;
    }


    public Cell(int row, int column)
    {
        this.Row = row;
        this.Column = column;
        Links = new List<Cell>();
    }

    public void SetNeighour(Cell cell, Direction direction)
    {
        if (cell == null)
        {
            return;
        }

        if (!neighbours.ContainsKey(direction))
        {
            switch (direction)
            {
                case Direction.Default:
                    throw new System.Exception("no direction");
                case Direction.North:
                    neighbours.Add(Direction.North, cell);
                    cell.neighbours.Add(Direction.South, this);
                    break;
                case Direction.South:
                    neighbours.Add(Direction.South, cell);
                    cell.neighbours.Add(Direction.North, this);
                    break;
                case Direction.East:
                    neighbours.Add(Direction.East, cell);
                    cell.neighbours.Add(Direction.West, this);
                    break;
                case Direction.West:
                    neighbours.Add(Direction.West, cell);
                    cell.neighbours.Add(Direction.East, this);
                    break;
                default:
                    break;
            }
        }
    }

    public void RemoveNeighbour(Direction direction)
    {
        switch (direction)
        {
            case Direction.Default:
                throw new System.Exception("no direction");
            case Direction.North:
                neighbours[Direction.North].neighbours.Remove(Direction.South);
                neighbours.Remove(Direction.North);
                break;
            case Direction.South:
                neighbours[Direction.South].neighbours.Remove(Direction.North);
                neighbours.Remove(Direction.South);
                break;
            case Direction.East:
                neighbours[Direction.East].neighbours.Remove(Direction.West);
                neighbours.Remove(Direction.East);
                break;
            case Direction.West:
                neighbours[Direction.West].neighbours.Remove(Direction.East);
                neighbours.Remove(Direction.West);
                break;
            default:
                break;
        }
    }

    public void LinkCell(Cell cell, bool bidi)
    {
        if (cell != null)
        {
            Links.Add(cell);

            if (bidi)
            {
                cell.LinkCell(this, false);
            }
        }
    }

    public void UnlinkCell(Cell cell, bool bidi)
    {
        if (cell != null)
        {
            Links.Remove(cell);

            if (bidi)
            {
                cell.LinkCell(this, false);
            }
        }
    }

    public Cell RandomNeighbour()
    {
        Cell neighbour = neighbours.ElementAt(Random.Range(0, neighbours.Count)).Value;
        return neighbour;
    }


    /// <summary>
    /// returns the first dead end neighbour. Returns null if there aren't any.
    /// </summary>
    /// <returns></returns>
    public Cell DeadEndNeighbour()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours.ElementAt(i).Value.Links.Count == 1)
            {
                return neighbours.ElementAt(i).Value;
            }
        }

        return null;
    }


    /// <summary>
    /// Returns a random unvisited neighbour. Returns null if there are none.
    /// </summary>
    /// <returns></returns>
    public Cell RandomUnvisitedNeighbour()
    {
        List<Cell> unvisitedNeighbours = new List<Cell>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (!neighbours.ElementAt(i).Value.Visited)
            {
                unvisitedNeighbours.Add(neighbours.ElementAt(i).Value);
            }
        }

        if (unvisitedNeighbours.Count == 0)
        {
            return null;
        }

        return unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
    }


    /// <summary>
    /// Returns true if at least one of the neighbours have been visited.
    /// </summary>
    /// <returns></returns>
    public bool HasVisitedNeighbour()
    {
        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours.ElementAt(i).Value.Visited)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// Returns a random visited neighbour. Returns null if there aren't any.
    /// </summary>
    /// <returns></returns>
    public Cell GetRandomVisitedNeighbour()
    {
        List<Cell> visitedNeighbours = new List<Cell>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (neighbours.ElementAt(i).Value.Visited)
            {
                visitedNeighbours.Add(neighbours.ElementAt(i).Value);
            }
        }

        if (visitedNeighbours.Count == 0)
        {
            return null;
        }

        return visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
    }

}

