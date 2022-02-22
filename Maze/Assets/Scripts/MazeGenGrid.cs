using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGT
{
    public class MazeGenGrid
    {
        private int rows, columns;
        private Cell[,] grid;
        private int[] cellCounts;

        public int Rows { get { return rows; } }
        public int Columns { get { return columns; } }


        #region Constructors
        public MazeGenGrid()
        {
            cellCounts = new int[9];
        }


        public MazeGenGrid(int columns, int rows) : this()
        {
            this.rows = rows;
            this.columns = columns;
            cellCounts[0] = columns * rows;

            grid = new Cell[columns, rows];

            PopulateGrid();
            SetupMask();
            ConfigureGrid();
        }


        public MazeGenGrid(Color[] bitmap, List<Color> layerColours, int columns, int rows) : this()
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

            PopulateGrid();
            SetupMask(mask, layerColours);
            ConfigureGrid();
        }

        #endregion


        #region Setup Functions

        /// <summary>
        /// sets the layermask of the cells.
        /// </summary>
        /// <param name="map">The 2D Color array representing each cell</param>
        /// <param name="layerColours">The layer that each colour belongs to</param>
        private void SetupMask(Color[,] map, List<Color> layerColours)
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
                        for (int i = 1; i < layerColours.Count; i++)
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


        /// <summary>
        /// Sets the mask of all cells to the given mask. 
        /// </summary>
        /// <param name="mask">Lowest number is 1 as 0 is reserved for accessing all layers</param>
        private void SetupMask(int mask = 1)
        {
            if (mask < 1)
            {
                mask = 1;
            }

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    grid[column, row].Mask = mask;
                    cellCounts[mask]++;
                }
            }
        }


        private void PopulateGrid()
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
        private void ConfigureGrid()
        {
            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    grid[column, row].SetNeighour(GetCell(column, row + 1), Cell.CardinalDirection.North);
                    grid[column, row].SetNeighour(GetCell(column, row - 1), Cell.CardinalDirection.South);
                    grid[column, row].SetNeighour(GetCell(column + 1, row), Cell.CardinalDirection.East);
                    grid[column, row].SetNeighour(GetCell(column - 1, row), Cell.CardinalDirection.West);
                }
            }
        }

        #endregion


        #region Get Functions
        
        /// <summary>
        /// Returns the number of cells on the given mask. If mask is 0, returns the number of valid cells from all masks
        /// </summary>
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
        /// Returns the first valid, unvisited cell found. Returns null if there are none.
        /// </summary>
        public Cell GetUnvisitedCell(bool withVisitedNeighbour, int mask = 0)
        {
            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    if (mask == 0 || mask == grid[column, row].Mask)
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
        public List<Cell> GetDeadEnds()
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
        /// Returns the given cell on the given mask. If not on the mask or the cell is otherwise invalid, returns null
        /// </summary>
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


        /// <summary>
        /// returns a random cell on the given mask
        /// </summary>
        public Cell GetRandomCell(int mask = 0)
        {
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


        /// <summary>
        /// Returns a list of connectors that contain cells with neighbours that belong to a different mask/layer
        /// </summary>
        /// <returns></returns>
        public List<Connector> GetConnectingCells()
        {
            List<Connector> connectingCells = new List<Connector>();

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Cell currentCell = grid[column, row];

                    if (currentCell != null && currentCell.Mask != -1)
                    {
                        Cell neighbour = currentCell.GetBoarderingNeighbour();

                        if (neighbour != null)
                        {
                            currentCell.Region = currentCell.Mask;
                            Connector conCell = new Connector();
                            conCell.currentCell = currentCell;
                            conCell.connectedCell = neighbour;
                            connectingCells.Add(conCell);
                        }
                    }
                }
            }

            return connectingCells;
        }

        #endregion


        /// <summary>
        /// Finds the dead ends of the grid and links them to unlinked, neighbouring cells so they're no longer deadends.
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
                    Cell neighbour = currentCell.GetDeadEndNeighbour();

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


        /// <summary>
        /// If the given mask is 0, returns true so long as the cells mask isn't negative. 
        /// If the given mask is higher than 0, returns true only if the cell's mask matches the given mask
        /// </summary>
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


        /// <summary>
        /// If the given mask is 0, returns true so long as the cells mask isn't negative. 
        /// If the given mask is higher than 0, returns true only if the cell's mask matches the given mask
        /// </summary>
        public bool CellValid(int column, int row, int mask = 0)
        {
            Cell cell = grid[column, row];
            return CellValid(cell, mask);
        }
    }
}