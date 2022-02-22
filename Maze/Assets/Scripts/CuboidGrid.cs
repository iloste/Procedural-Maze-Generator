using System.Collections.Generic;

namespace MGT
{
    //public class CuboidGrid : MazeGenGrid
    //{

    //    GridStruct[] grids;
    //    public CuboidGrid() : base()
    //    {
    //        grids = new GridStruct[6];
    //    }

    //    public CuboidGrid(int columns, int rows) : this()
    //    {
    //        for (int i = 0; i < grids.Length; i++)
    //        {
    //            grids[i].columns = columns;
    //            grids[i].rows = rows;
    //            grids[i].grid = new Cell[columns, rows];
    //            grids[i].ID = i;
    //        }

    //        PrepareGrid();
    //        SetupMask();
    //        ConfigureGrid();
    //    }



    //    #region Grid Setup
    //    /// <summary>
    //    /// Fills the grids.
    //    /// </summary>
    //    protected override void PrepareGrid()
    //    {
    //        for (int i = 0; i < grids.Length; i++)
    //        {
    //            for (int row = 0; row < grids[i].columns; row++)
    //            {
    //                for (int column = 0; column < grids[i].rows; column++)
    //                {
    //                    grids[i].grid[column, row] = new Cell(column, row, i);
    //                }
    //            }
    //        }
    //    }


    //    /// <summary>
    //    /// Sets the mask for each cell.
    //    /// </summary>
    //    protected override void SetupMask()
    //    {
    //        for (int i = 0; i < grids.Length; i++)
    //        {
    //            for (int row = 0; row < grids[i].rows; row++)
    //            {
    //                for (int column = 0; column < grids[i].columns; column++)
    //                {
    //                    int mask = 1;
    //                    grids[i].grid[column, row].Mask = mask;
    //                    cellCounts[mask]++;
    //                }
    //            }
    //        }
    //    }


    //    /// <summary>
    //    /// Set the neighbours of each cell
    //    /// </summary>
    //    protected override void ConfigureGrid()
    //    {
    //        for (int gridID = 0; gridID < grids.Length; gridID++)
    //        {
    //            for (int row = 0; row < grids[gridID].rows; row++)
    //            {
    //                for (int column = 0; column < grids[gridID].columns; column++)
    //                {
    //                    grids[gridID].grid[column, row].SetNeighour(GetCell(column, row + 1, gridID), Cell.CardinalDirection.North);
    //                    grids[gridID].grid[column, row].SetNeighour(GetCell(column, row - 1, gridID), Cell.CardinalDirection.South);
    //                    grids[gridID].grid[column, row].SetNeighour(GetCell(column + 1, row, gridID), Cell.CardinalDirection.East);
    //                    grids[gridID].grid[column, row].SetNeighour(GetCell(column - 1, row, gridID), Cell.CardinalDirection.West);
    //                }
    //            }
    //        }

    //        //// linking grid 0
    //        SetFrontierNeighbours(GetGridFrontier(grids[0], Cell.CardinalDirection.North), Cell.CardinalDirection.North, GetGridFrontier(grids[3], Cell.CardinalDirection.South), Cell.CardinalDirection.South, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[0], Cell.CardinalDirection.South), Cell.CardinalDirection.South, GetGridFrontier(grids[1], Cell.CardinalDirection.North), Cell.CardinalDirection.North, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[0], Cell.CardinalDirection.East), Cell.CardinalDirection.East, GetGridFrontier(grids[5], Cell.CardinalDirection.North), Cell.CardinalDirection.North, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[0], Cell.CardinalDirection.West), Cell.CardinalDirection.West, GetGridFrontier(grids[4], Cell.CardinalDirection.North), Cell.CardinalDirection.North, true);

    //        // linking grid 1
    //        SetFrontierNeighbours(GetGridFrontier(grids[1], Cell.CardinalDirection.South), Cell.CardinalDirection.South, GetGridFrontier(grids[2], Cell.CardinalDirection.North), Cell.CardinalDirection.North, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[1], Cell.CardinalDirection.East), Cell.CardinalDirection.East, GetGridFrontier(grids[5], Cell.CardinalDirection.West), Cell.CardinalDirection.West, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[1], Cell.CardinalDirection.West), Cell.CardinalDirection.West, GetGridFrontier(grids[4], Cell.CardinalDirection.East), Cell.CardinalDirection.East, false);

    //        // linking grid 2
    //        SetFrontierNeighbours(GetGridFrontier(grids[2], Cell.CardinalDirection.South), Cell.CardinalDirection.South, GetGridFrontier(grids[3], Cell.CardinalDirection.North), Cell.CardinalDirection.North, false);
    //        SetFrontierNeighbours(GetGridFrontier(grids[2], Cell.CardinalDirection.East), Cell.CardinalDirection.East, GetGridFrontier(grids[5], Cell.CardinalDirection.South), Cell.CardinalDirection.South, true);
    //        SetFrontierNeighbours(GetGridFrontier(grids[2], Cell.CardinalDirection.West), Cell.CardinalDirection.West, GetGridFrontier(grids[4], Cell.CardinalDirection.South), Cell.CardinalDirection.South, false);

    //        // linking grid 3
    //        SetFrontierNeighbours(GetGridFrontier(grids[3], Cell.CardinalDirection.East), Cell.CardinalDirection.East, GetGridFrontier(grids[5], Cell.CardinalDirection.East), Cell.CardinalDirection.East, true);
    //        SetFrontierNeighbours(GetGridFrontier(grids[3], Cell.CardinalDirection.West), Cell.CardinalDirection.West, GetGridFrontier(grids[4], Cell.CardinalDirection.West), Cell.CardinalDirection.West, true);

    //        // grid 4 and 5 are link from the above.
    //    }


    //    /// <summary>
    //    /// returns a list of the norther/eastern/southern/western border cells.
    //    /// </summary>
    //    /// <param name="grid"></param>
    //    /// <param name="direction"></param>
    //    /// <returns></returns>
    //    public List<Cell> GetGridFrontier(GridStruct grid, Cell.CardinalDirection direction)
    //    {
    //        List<Cell> cells = new List<Cell>();

    //        switch (direction)
    //        {
    //            case Cell.CardinalDirection.North:
    //                for (int column = 0; column < grid.columns; column++)
    //                {
    //                    cells.Add(grid.grid[column, grid.rows - 1]);
    //                }
    //                break;
    //            case Cell.CardinalDirection.South:
    //                for (int column = 0; column < grid.columns; column++)
    //                {
    //                    cells.Add(grid.grid[column, 0]);
    //                }
    //                break;
    //            case Cell.CardinalDirection.East:
    //                for (int row = 0; row < grid.rows; row++)
    //                {
    //                    cells.Add(grid.grid[grid.columns - 1, row]);
    //                }
    //                break;
    //            case Cell.CardinalDirection.West:
    //                for (int row = 0; row < grid.rows; row++)
    //                {
    //                    cells.Add(grid.grid[0, row]);
    //                }
    //                break;
    //        }

    //        return cells;
    //    }


    //    /// <summary>
    //    /// Links the cells in a given list
    //    /// </summary>
    //    /// <param name="listA"></param>
    //    /// <param name="listB"></param>
    //    /// <param name="flipList">Reverses one of the lists.</param>
    //    private void SetFrontierNeighbours(List<Cell> listA, Cell.CardinalDirection direction1, List<Cell> listB, Cell.CardinalDirection direction2, bool flipList)
    //    {

    //        // make new set neighbour function that takes 2 directions so you can set the northern and eastern neighbours together.
    //        if (!flipList)
    //        {
    //            for (int i = 0; i < listA.Count; i++)
    //            {
    //                listA[i].SetNeighour(listB[i], direction1, direction2);
    //            }
    //        }
    //        else
    //        {
    //            for (int i = 0; i < listA.Count; i++)
    //            {
    //                listA[i].SetNeighour(listB[listB.Count - 1 - i], direction1, direction2);
    //            }
    //        }
    //    }

    //    #endregion


    //    public GridStruct GetGrid(int ID)
    //    {
    //        if (ID > grids.Length)
    //        {
    //            return new GridStruct();
    //        }

    //        return grids[ID];
    //    }


    //    public override void BraidMaze(int percentage)
    //    {
    //        base.BraidMaze(percentage);
    //    }

    //    public override bool CellValid(Cell cell, int mask = 0)
    //    {
    //        return base.CellValid(cell, mask);
    //    }

    //    public bool CellValid(int row, int column, int gridID, int mask = 0)
    //    {
    //        Cell cell = grids[gridID].grid[row, column];
    //        return CellValid(cell, mask);
    //    }


    //    public Cell GetCell(int column, int row, int gridID, int mask = 0)
    //    {
    //        if (row >= 0 && row < grids[gridID].rows)
    //        {
    //            if (column >= 0 && column < grids[gridID].columns)
    //            {
    //                if (CellValid(column, row, gridID, mask))
    //                {
    //                    return grids[gridID].grid[column, row];
    //                }
    //                else
    //                {
    //                    return null;
    //                }
    //            }
    //        }

    //        return null;
    //    }

    //    public override int GetCellCount(int mask = 0)
    //    {
    //        return base.GetCellCount(mask);
    //    }

    //    public override List<Cell> GetDeadEnds()
    //    {
    //        return base.GetDeadEnds();
    //    }

    //    public override Cell GetRandomCell(int mask = 0)
    //    {
    //        return base.GetRandomCell(mask);
    //    }

    //    public override Cell GetUnvisitedCell(bool withVisitedNeighbour, int mask = 0)
    //    {
    //        return base.GetUnvisitedCell(withVisitedNeighbour, mask);
    //    }
    //}

}