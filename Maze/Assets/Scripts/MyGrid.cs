using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    protected int rows, columns;
    protected Cell[,] grid;
    //private bool[,] cellValidity;
    private int[] cellCounts;

    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }


    public MyGrid()
    {
        cellCounts = new int[9];
    }

    public MyGrid(int rows, int columns) : this()
    {
        this.rows = rows;
        this.columns = columns;
        cellCounts[0] = rows * columns;

        grid = new Cell[rows, columns];

        PrepareGrid();
        SetupMask();
        ConfigureGrid();
    }

    public MyGrid(string path) : this()
    {
        string[] mask = System.IO.File.ReadAllLines(path);
        rows = mask.Length;
        columns = mask[0].Length;
        cellCounts[0] = rows * columns;
        grid = new Cell[rows, columns];

        PrepareGrid();
        SetupMask(mask);
        ConfigureGrid();
    }

    public int GetCellCount(int mask = 0)
    {
        if (mask < 0 || mask > cellCounts.Length - 1)
        {
            return 0;
        }
        else
        {
            return cellCounts[mask];
        }
    }


    /// <summary>
    /// Finds and returns the first valid, unvisited cell found. Returns null if there are none.
    /// </summary>
    /// <returns></returns>
    public Cell GetUnvisitedCell(bool withVisitedNeighbour, int mask = 0)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (mask == 0 || mask == grid[row, column].Mask)
                // if (cellValidity[row, column])
                {
                    if (!grid[row, column].Visited)
                    {
                        if (!withVisitedNeighbour)
                        {
                            return grid[row, column];
                        }
                        else if (grid[row, column].HasVisitedNeighbour())
                        {
                            return grid[row, column];
                        }
                    }
                }
            }
        }

        return null;
    }


    /// <summary>
    /// Finds and resturns all of the cells with only 1 link.
    /// </summary>
    /// <returns></returns>
    public List<Cell> GetDeadEnds()
    {
        List<Cell> deadEnds = new List<Cell>();

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (grid[row, column] != null)
                {
                    if (grid[row, column].Links.Count == 1)
                    {
                        deadEnds.Add(grid[row, column]);
                    }
                }
            }
        }

        return deadEnds;
    }


    /// <summary>
    /// Finds the dead ends of the grid and links them to other cells so they're no longer deadends.
    /// </summary>
    /// <param name="percentage">The percent of dead ends that will be linked.</param>
    public void BraidMaze(int percentage)
    {
        List<Cell> deadEnds = GetDeadEnds();
        int cellsRemaining = (int)(deadEnds.Count * (percentage / 100f));

        while (cellsRemaining > 0)
        {
            Cell currentCell = deadEnds[Random.Range(0, deadEnds.Count - 1)];

            while (true)
            {
                Cell neighbour = currentCell.DeadEndNeighbour();

                // to do: see if one of the neighbours is also a dead end and merge with that first. If not, get a random neighbour.

                if (neighbour == null)
                {
                    neighbour = currentCell.GetRandomNeighbour();
                }

                if (neighbour != null && neighbour != currentCell.Links[0])
                {
                    currentCell.LinkCell(neighbour, true);
                    deadEnds.Remove(currentCell);

                    if (deadEnds.Remove(neighbour))
                    {
                        cellsRemaining--;
                    }

                    cellsRemaining--;

                    break;
                }
            }
        }
    }


    public void SetupMask(string[] map)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int mask;

                if (int.TryParse(map[row][column].ToString(), out mask))
                {
                    cellCounts[mask]++;
                }
                else
                {
                    mask = -1;
                }

                grid[row, column].Mask = mask;
            }
        }
    }


    public void SetupMask()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int mask = 1;
                grid[row, column].Mask = mask;
                cellCounts[mask]++;
            }
        }
    }


    public bool CellValid(Cell cell, int mask = 0)
    {
        if (cell == null)
        {
            throw new System.Exception("Null cell");
        }

        if (cell.Mask == -1)
        {
            return false;
        }
        else if (mask == 0)
        {
            return true;
        }
        else if (cell.Mask == mask)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CellValid(int row, int column, int mask = 0)
    {
        Cell cell = grid[row, column];
        return CellValid(cell, mask);
    }


    /// <summary>
    /// Returns the given cell on the given mask. If not on the mask or the cell is otherwise invalid, returns null
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="mask">If 0, returns the cell regardless of it's mask</param>
    /// <returns></returns>
    public Cell GetCell(int row, int column, int mask = 0)
    {
        if (row >= 0 && row < rows)
        {
            if (column >= 0 && column < columns)
            {
                if (CellValid(row, column, mask))
                {
                    return grid[row, column];
                }
                else
                {
                    return null;
                }
            }
        }

        return null;
    }


    protected virtual void PrepareGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                grid[row, column] = new Cell(row, column);
            }
        }
    }


    protected virtual void ConfigureGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                grid[row, column].SetNeighour(GetCell(row - 1, column), Cell.Direction.North);
                grid[row, column].SetNeighour(GetCell(row + 1, column), Cell.Direction.South);
                grid[row, column].SetNeighour(GetCell(row, column + 1), Cell.Direction.East);
                grid[row, column].SetNeighour(GetCell(row, column - 1), Cell.Direction.West);
            }
        }
    }


    /// <summary>
    /// returns a random cell of the given mask
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public Cell GetRandomCell(int mask = 0)
    {
        //To do: make a list of each cell in each mask so you can make this more efficient.
        // You could store them as just the coordinates in a list of vector2.
        int row, column;
        Cell cell;

        do
        {
            row = Random.Range(0, Rows);
            column = Random.Range(0, columns);
            cell = GetCell(row, column);

            if (cell.Mask == -1 || (mask != 0 && cell.Mask != mask))
            {
                cell = null;
            }

        } while (cell == null);

        return GetCell(row, column);
    }
}
