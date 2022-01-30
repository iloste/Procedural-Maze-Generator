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
    public bool Visted { get; set; } = false;

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

}

