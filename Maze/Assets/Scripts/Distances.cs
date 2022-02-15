using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distances
{
    Dictionary<Cell, int> cells;
    Cell root;

    public Distances(Cell root)
    {
        cells = new Dictionary<Cell, int>();
        this.root = root;
        cells.Add(root, 0);




    }


    private void SetDistances()
    {
        Queue<Cell> unVisitedCells = new Queue<Cell>();
        List<Cell> visitedCells = new List<Cell>();
        unVisitedCells.Enqueue(root);

        while (unVisitedCells.Count > 0)
        {
            Cell currentCell = unVisitedCells.Dequeue();
            //for (int i = 0; i < length; i++)
            //{

            //}
        }
    }

    /// <summary>
    /// returns the distance of the cell from the root. If the cell isn't recorded, it returns -1.
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    public int GetDistance(Cell cell)
    {
        if (cells.ContainsKey(cell))
        {
            return cells[cell];
        }
        else { return -1; }
    }


    public void SetDistance(Cell cell, int distance)
    {
        if (cells.ContainsKey(cell))
        {
            cells[cell] = distance;
        }
        else
        {
            cells.Add(cell, distance);
        }
    }



}
