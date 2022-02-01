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
        // cellValidity = new bool[rows, columns];

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
        // cellValidity = new bool[rows, columns];

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

    //public MyGrid(Texture2D bitmap)
    //{
    //    Color[] colours = bitmap.GetPixels();
    //    columns = bitmap.width;
    //    rows = bitmap.height;
    //    Count = rows * columns;
    //    grid = new Cell[rows, columns];
    //    cellValidity = new bool[rows, columns];
    //    int y = 0;
    //    int x = 0;

    //    for (int i = 0; i < colours.Length; i++)
    //    {
    //        y = (i + 1) / bitmap.width;
    //        x = i - y * bitmap.width;
    //        if (y == 64 || x == 64)
    //        {

    //        }
    //        Debug.Log(1);
    //        if (colours[i] == Color.white)
    //        {
    //            cellValidity[x, y] = false;
    //        }
    //        else
    //        {
    //            cellValidity[x, y] = true;
    //        }
    //    }




    //}



    public void setupValidity()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                //cellValidity[i, j] = true;
            }
        }
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

        //return cellValidity[cell.Row, cell.Column];
    }

    public bool CellValid(int row, int column, int mask = 0)
    {

        Cell cell = grid[row, column];
        return CellValid(cell, mask);


        //if (row >= 0 && row < Rows)
        //{
        //    if (column >= 0 && column < Columns)
        //    {
        //        return cellValidity[row, column];
        //    }
        //}

        //throw new System.Exception("cell does not exist");
    }

    public Cell GetCell(int row, int column, int mask = 0)
    {
        if (row >= 0 && row < rows)
        {
            if (column >= 0 && column < columns)
            {
                //if (grid[row, column].Valid)
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

    #region Ascii Display
    //public void DisplayGrid()
    //{
    //    string top = "";
    //    string body = "";
    //    string bottom = "";
    //    bool previousCellWasNull = false;

    //    for (int row = 0; row < rows; row++)
    //    {
    //        top = "+";
    //        body = "|";

    //        for (int column = 0; column < columns; column++)
    //        {
    //            Cell cell = GetCell(row, column);

    //            if (cell != null)
    //            {
    //                //get the top of each row. Note: The top of each row is the bottom of the previous. This is why we don't get the bottom except for the last row.
    //                if (cell.IsLinked(Cell.Direction.North))
    //                {
    //                    top += "   ";
    //                }
    //                else
    //                {
    //                    top += "---";
    //                }

    //                top += "+";

    //                //get the body of each row. 
    //                if (cell.Valid)
    //                {
    //                    if (!previousCellWasNull)
    //                    {
    //                        body += "   ";
    //                    }
    //                    else
    //                    {
    //                        body = body.Remove(body.Length - 1);
    //                        body += "|   ";
    //                    }
    //                }

    //                if (cell.IsLinked(Cell.Direction.East))
    //                {
    //                    body += " ";
    //                }
    //                else
    //                {
    //                    body += "|";
    //                }
    //                previousCellWasNull = false;

    //            }
    //            else
    //            {
    //                if (previousCellWasNull)
    //                {
    //                    top += "+---";
    //                    body += "|   ";
    //                }
    //                else
    //                {
    //                    top += "    ";
    //                    body += "    ";
    //                }
    //                //  top += "---+";
    //                //body += "xxx|";

    //                previousCellWasNull = true;
    //            }
    //        }

    //        Console.WriteLine(top);
    //        Console.WriteLine(body);

    //    }

    //    // get the very bottom of the grid
    //    bottom += "+";

    //    for (int i = 0; i < columns; i++)
    //    {
    //        bottom += "---+";
    //    }

    //    Console.WriteLine(bottom);

    //}
    #endregion

    #region Bitmap
    //public void CreatePNG()
    //{
    //    int cellSize = 12;
    //    Bitmap bitmap = new Bitmap(rows * cellSize, columns * cellSize);
    //    bool[,] boolmap = new bool[rows * cellSize, columns * cellSize];

    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            Cell cell = grid[row, column];
    //            if (cell.Valid)
    //            {
    //                DrawToBitmap(ref boolmap, row * cellSize, column * cellSize, row * cellSize + 10, column * cellSize + 10);
    //                if (cell.neighbours.ContainsKey(Cell.Direction.North))
    //                {

    //                }

    //            }
    //        }
    //    }

    //    for (int row = 0; row < boolmap.GetLength(0); row++)
    //    {
    //        for (int column = 0; column < boolmap.GetLength(1); column++)
    //        {
    //            if (boolmap[row, column])
    //            {
    //                bitmap.SetPixel(row, column, Color.White);
    //            }
    //            else
    //            {
    //                bitmap.SetPixel(row, column, Color.Black);
    //            }
    //        }
    //    }

    //    bitmap.Save(System.IO.Directory.GetCurrentDirectory() + "/Res/Maze.png", System.Drawing.Imaging.ImageFormat.Png);
    //}

    //public bool DrawToBitmap(ref bool[,] bitmap, int row, int column, int rowEnd, int columnEnd)
    //{
    //    if (row < 0 || row > bitmap.GetLength(0))
    //    {
    //        return false;
    //    }
    //    else if (column < 0 || column > bitmap.GetLength(1))
    //    {
    //        return false;
    //    }
    //    else if (rowEnd <= row || rowEnd > bitmap.GetLength(0))
    //    {
    //        return false;
    //    }
    //    else if (columnEnd <= column || columnEnd > bitmap.GetLength(1))
    //    {
    //        return false;
    //    }

    //    for (int rowIndex = row; rowIndex < rowEnd; rowIndex++)
    //    {
    //        for (int columnIndex = column; columnIndex < columnEnd; columnIndex++)
    //        {
    //            bitmap[rowIndex, columnIndex] = true;
    //        }
    //    }

    //    return true;
    //}
    #endregion


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
        // } while (cell == null || !CellValid(cell));

        return GetCell(row, column);
    }

    //public void ResetGridDistances()
    //{
    //    for (int row = 0; row < rows; row++)
    //    {
    //        for (int column = 0; column < columns; column++)
    //        {
    //            //grid[row, column].DistanceFromOrigin = 0;
    //            //grid[row, column].DijkstraVisited = false;
    //        }
    //    }
    //}
}
