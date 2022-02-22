using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGT
{
    public class Pathfinding
    {
        public enum PathfindingOptions
        {
            PathBetween,
            LongestPath,
            FloodGrid,
        }


        public int maxDistance;
        Cell mostDistantCell;

        private int[,] distances;
        public Cell Origin { get; private set; }
        public Cell Destination { get; private set; }


        public Pathfinding(int columns, int rows)
        {
            distances = new int[columns, rows];
            ResetDistances();
        }


        private void ResetDistances()
        {
            for (int row = 0; row < distances.GetLength(0); row++)
            {
                for (int column = 0; column < distances.GetLength(1); column++)
                {
                    distances[column, row] = -1;
                }
            }
        }

        public void FloodGrid(Cell origin)
        {
            Origin = origin;
            Destination = null;

            List<Cell> cells = new List<Cell>();
            List<Cell> frontier = new List<Cell>();
            frontier.Add(origin);
            int distance = 0;

            while (frontier.Count > 0)
            {
                for (int i = 0; i < frontier.Count; i++)
                {
                    SetDistanceFromOrigin(frontier[i], distance);
                }

                distance++;

                cells = frontier;
                frontier = new List<Cell>();

                while (cells.Count > 0)
                {
                    for (int i = 0; i < cells[0].Links.Count; i++)
                    {
                        Cell linkedCell = cells[0].Links[i];

                        if (GetDistanceFromOrigin(linkedCell) == -1)
                        {
                            frontier.Add(linkedCell);
                        }
                    }

                    mostDistantCell = cells[0];
                    cells.RemoveAt(0);
                }
            }

            maxDistance = distance;
        }


        public int GetDistanceFromOrigin(Cell cell)
        {
            if (cell == null)
            {
                throw new System.Exception("Null cell");
            }
            if (cell.Row < 0 && cell.Column < 0)
            {
                throw new System.Exception("Null cell");
            }

            return distances[cell.Column, cell.Row];
        }

        private void SetDistanceFromOrigin(Cell cell, int distance)
        {
            if (cell == null)
            {
                throw new System.Exception("Null cell");
            }

            distances[cell.Column, cell.Row] = distance;
        }


        public Stack<Cell> ShortestPath(Cell origin, Cell destination, int mask = 0)
        {
            FloodGrid(origin);
            Origin = origin;
            Destination = destination;

            Stack<Cell> path = new Stack<Cell>();

            Cell cell = destination;

            while (GetDistanceFromOrigin(cell) > 0)
            {
                path.Push(cell);

                for (int i = 0; i < cell.Links.Count; i++)
                {
                    if (GetDistanceFromOrigin(cell.Links[i]) < GetDistanceFromOrigin(cell))
                    {
                        cell = cell.Links[i];
                        break;
                    }
                }
            }

            path.Push(cell);
            return path;
        }


        public Stack<Cell> FindLongestPath(MazeGenGrid grid, Cell origin)
        {
            FloodGrid(origin);
            Cell newOrigin = mostDistantCell;
            ResetDistances();
            //grid.ResetGridDistances();
            FloodGrid(newOrigin);
            Origin = newOrigin;
            Destination = mostDistantCell;
            return ShortestPath(Origin, Destination);
        }

    }

}