using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskedGrid : MyGrid
{

   public bool[,] Mask { get; private set; }

    public MaskedGrid(bool[,] mask)
    {
        Mask = mask;
        rows = mask.GetLength(0);
        columns = mask.GetLength(1);
        Count = rows * columns;

        grid = new Cell[rows, columns];
        PrepareGrid();
        ConfigureGrid();
    }

    protected override void ConfigureGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (Mask[row, column])
                {
                    grid[row, column].SetNeighour(GetCell(row - 1, column), Cell.Direction.North);
                    grid[row, column].SetNeighour(GetCell(row + 1, column), Cell.Direction.South);
                    grid[row, column].SetNeighour(GetCell(row, column + 1), Cell.Direction.East);
                    grid[row, column].SetNeighour(GetCell(row, column - 1), Cell.Direction.West);
                }
            }
        }
    }

}
