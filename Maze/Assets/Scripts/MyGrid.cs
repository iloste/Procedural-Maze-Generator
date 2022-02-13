using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGrid
{
    private int rows, columns;
    private Cell[,] grid;
    protected int[] cellCounts;

    public int Rows { get { return rows; } }
    public int Columns { get { return columns; } }


    public MyGrid()
    {
        cellCounts = new int[9];
    }

    public MyGrid(int columns, int rows) : this()
    {
        this.rows = rows;
        this.columns = columns;
        cellCounts[0] = columns * rows;

        grid = new Cell[columns, rows];

        PrepareGrid();
        SetupMask();
        ConfigureGrid();
    }

    //public MyGrid(string path) : this()
    //{
    //    string[] mask = System.IO.File.ReadAllLines(path);
    //    rows = mask.Length;
    //    columns = mask[0].Length;
    //    cellCounts[0] = rows * columns;
    //    grid = new Cell[columns, rows];

    //    PrepareGrid();
    //    SetupMask(mask);
    //    ConfigureGrid();
    //}

    public MyGrid(Color[] bitmap, Color[] layerColours, int columns, int rows) : this()
    {
        this.columns = columns;
        this.rows = rows;
        cellCounts[0] = columns * rows;
        grid = new Cell[columns, rows];

        Color[,] mask = new Color[columns, rows];

        int c = 0;
        int r = 0;

        for (int i = 0; i < bitmap.Length; i++)
        {
            c = i % columns;
            r = i / columns;
            mask[c, r] = bitmap[i];
        }

        PrepareGrid();
        SetupMask(mask, layerColours);
        ConfigureGrid();
    }

    public virtual int GetCellCount(int mask = 0)
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
    public virtual Cell GetUnvisitedCell(bool withVisitedNeighbour, int mask = 0)
    {
        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (mask == 0 || mask == grid[column, row].Mask)
                // if (cellValidity[row, column])
                {
                    if (!grid[column, row].Visited)
                    {
                        if (!withVisitedNeighbour)
                        {
                            return grid[column, row];
                        }
                        else if (grid[column, row].HasVisitedNeighbour())
                        {
                            return grid[column, row];
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
    public virtual List<Cell> GetDeadEnds()
    {
        List<Cell> deadEnds = new List<Cell>();

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                if (grid[column, row] != null)
                {
                    if (grid[column, row].Links.Count == 1)
                    {
                        deadEnds.Add(grid[column, row]);
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
    public virtual void BraidMaze(int percentage)
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


    //protected virtual void SetupMask(string[] map)
    //{
    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            int mask;

    //            if (int.TryParse(map[column][row].ToString(), out mask))
    //            {
    //                cellCounts[mask]++;
    //            }
    //            else
    //            {
    //                mask = -1;
    //                cellCounts[0]--;
    //            }

    //            grid[column, row].Mask = mask;
    //        }
    //    }
    //}


    /// <summary>
    /// sets the layermask of the cells.
    /// </summary>
    /// <param name="map">The 2D Color array representing each cell</param>
    /// <param name="layerColours">The layer that each colour belongs to</param>
    protected virtual void SetupMask(Color[,] map, Color[] layerColours)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int mask = 0;

                if (map[column, row] == layerColours[0])
                {
                    mask = -1;
                    grid[column, row].Mask = mask;
                    cellCounts[0]--;
                }
                else
                {
                    for (int i = 1; i < layerColours.Length; i++)
                    {
                        if (map[column, row] == layerColours[i])
                        {
                            mask = i;
                            grid[column, row].Mask = mask;
                            cellCounts[mask]++;
                            break;
                        }
                    }
                }
            }
        }
    }


    protected virtual void SetupMask()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                int mask = 1;
                grid[column, row].Mask = mask;
                cellCounts[mask]++;
            }
        }
    }


    public virtual bool CellValid(Cell cell, int mask = 0)
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

    public virtual bool CellValid(int column, int row, int mask = 0)
    {
        Cell cell = grid[column, row];
        return CellValid(cell, mask);
    }


    /// <summary>
    /// Returns the given cell on the given mask. If not on the mask or the cell is otherwise invalid, returns null
    /// </summary>
    /// <param name="row"></param>
    /// <param name="column"></param>
    /// <param name="mask">If 0, returns the cell regardless of it's mask</param>
    /// <returns></returns>
    public Cell GetCell(int column, int row, int mask = 0)
    {
        if (row >= 0 && row < rows)
        {
            if (column >= 0 && column < columns)
            {
                if (CellValid(column, row, mask))
                {
                    return grid[column, row];
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
                grid[column, row] = new Cell(column, row);
            }
        }
    }


    /// <summary>
    /// Sets the Neighbours of each cell
    /// </summary>
    protected virtual void ConfigureGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                grid[column, row].SetNeighour(GetCell(column, row + 1), Cell.Direction.North);
                grid[column, row].SetNeighour(GetCell(column, row - 1), Cell.Direction.South);
                grid[column, row].SetNeighour(GetCell(column + 1, row), Cell.Direction.East);
                grid[column, row].SetNeighour(GetCell(column - 1, row), Cell.Direction.West);
            }
        }
    }


    /// <summary>
    /// returns a random cell of the given mask
    /// </summary>
    /// <param name="mask"></param>
    /// <returns></returns>
    public virtual Cell GetRandomCell(int mask = 0)
    {
        //To do: make a list of each cell in each mask so you can make this more efficient.
        // You could store them as just the coordinates in a list of vector2.

        //To do: this needs to take layer -1 into account. Sometimes it tries to access an invalid cell
        int column, row;
        Cell cell;

        do
        {
            column = Random.Range(0, Columns);
            row = Random.Range(0, Rows);
            cell = GetCell(column, row, mask);

            if (cell != null)
            {
                if (cell.Mask == -1 || (mask != 0 && cell.Mask != mask))
                {
                    cell = null;
                }
            }

        } while (cell == null);

        return GetCell(column, row);
    }
}
