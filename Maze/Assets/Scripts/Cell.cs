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
    public int Mask { get; set; }
    public Tile Tile { get; set; }
    public int GridNum { get; set; }
    public bool InRoom { get; set; }

    public Cell(int column, int row)
    {
        this.Row = row;
        this.Column = column;
        Links = new List<Cell>();
    }


    public Cell(int row, int column, int gridNum) : this(row, column)
    {
        GridNum = gridNum;
    }


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




    public void SetNeighour(Cell cell, Direction direction)
    {
        if (cell == null)
        {
            return;
        }
        else if (cell.Mask == -1 || Mask == -1)
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


    public void SetNeighour(Cell cell, Direction direction1, Direction direction2)
    {
        if (cell == null)
        {
            return;
        }
        else if (cell.Mask == -1 || Mask == -1)
        {
            return;
        }

        if (!neighbours.ContainsKey(direction1) && !cell.neighbours.ContainsKey(direction2))
        {
            neighbours.Add(direction1, cell);
            cell.neighbours.Add(direction2, this);
        }
        else
        {
            Debug.LogError("Neighbour Exists");
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


    /// <summary>
    /// Returns a neighbour in the given mask. Returns a random neighbour from any mask if the given mask is 0.
    /// Returns null if no suitable neighbours.
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public Cell GetRandomNeighbour(int mask = 0)
    {
        if (mask == 0)
        {
            Cell neighbour = neighbours.ElementAt(Random.Range(0, neighbours.Count)).Value;
            return neighbour;
        }
        else
        {
            List<Cell> validNeighbours = new List<Cell>();

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours.ElementAt(i).Value.Mask == mask)
                {
                    validNeighbours.Add(neighbours.ElementAt(i).Value);
                }
            }

            if (validNeighbours.Count > 0)
            {
                return validNeighbours.ElementAt(Random.Range(0, validNeighbours.Count));
            }
            else
            {
                return null;
            }
        }
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
    public Cell RandomUnvisitedNeighbour(int mask = 0)
    {
        List<Cell> unvisitedNeighbours = new List<Cell>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
            {
                if (!neighbours.ElementAt(i).Value.Visited)
                {
                    unvisitedNeighbours.Add(neighbours.ElementAt(i).Value);
                }
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
    public Cell GetRandomVisitedNeighbour(int mask)
    {
        List<Cell> visitedNeighbours = new List<Cell>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
            {
                if (neighbours.ElementAt(i).Value.Visited)
                {
                    visitedNeighbours.Add(neighbours.ElementAt(i).Value);
                }
            }
        }

        if (visitedNeighbours.Count == 0)
        {
            return null;
        }

        return visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
    }


    /// <summary>
    /// Returns a list of all the neighbours on the given mask.
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public List<Cell> GetNeighbours(int mask = 0)
    {
        List<Cell> validNeighbours = new List<Cell>();

        for (int i = 0; i < neighbours.Count; i++)
        {
            if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
            {
                validNeighbours.Add(neighbours.ElementAt(i).Value);
            }
        }

        return validNeighbours;
    }

}

