using System.Collections.Generic;
using UnityEngine;

namespace MGT
{
    public class MazeGenerator : MonoBehaviour
    {

        #region Variables
        public Algorithm algorithm;
        public List<Algorithm> algorithms = new List<Algorithm>();
        public Vector2Int gridSize;
        public bool braidMaze;
        public int braidPercentage;
        public bool displayDeadEnds;
        public int currentMask;
        public MazeDisplay mazeDisplay;
        public bool useCuboidMaze;
        public Texture2D layerImage;
        public bool useRandomSeed = true;
        public int seed;
        public int layerColourCount;
        public List<Color> layerColours = new List<Color>();
        public int tab;
        public bool removeDeadends;

        MazeGenGrid grid;
        Pathfinding pf;

        public float xzScale = 1;
        public float yScale = 1;

        #endregion

        public enum Algorithm
        {
            BinaryTree,
            SideWinder,
            AldousBroder,
            Wilson,
            AldousBroderLive,
            RecursiveBacktracker,
            HuntAndKill,
            CreateRoom,
            IgnoreLayer,
        }

        public enum Colouring
        {
            None,
            PathBetween,
            LongestPath,
            ColourMaze,
        }


        /// <summary>
        /// Generates and Displays the a maze with all of the data held in the instance
        /// </summary>
        public void GenerateMaze()
        {
            InitialiseSeed();   

            if (tab == 1)
            {
                GenerateImageBasedMaze();
            }
            else
            {
                GenerateAutoGridMaze();
            }

            ConnectRegions();
            BraidMaze();

            if (removeDeadends)
            {
                RemoveDeadEnds();
            }

            mazeDisplay = GetComponent<MazeDisplay>();
            mazeDisplay.DisplayGrid(grid, xzScale, yScale);
        }


        /// <summary>
        /// Generates a maze that fills the grid the size of columns and rows
        /// </summary>
        private void GenerateAutoGridMaze()
        {
            grid = new MazeGenGrid(gridSize.x, gridSize.y);
            RunMazeGenAlgorithm(algorithm, 0);
        }


        /// <summary>
        /// Generates a maze based on the masks gathered from the given image
        /// </summary>
        private void GenerateImageBasedMaze()
        {
            if (layerImage != null)
            {
                Color[] bitmap = layerImage.GetPixels();
                grid = new MazeGenGrid(bitmap, layerColours, layerImage.width, layerImage.height);

                for (int i = 1; i < algorithms.Count; i++)
                {
                    RunMazeGenAlgorithm(algorithms[i], i);
                }
            }
            else
            {
                Debug.LogError("No Layer Image Selected");
            }
        }


        private void InitialiseSeed()
        {
            if (useRandomSeed)
            {
                seed = Random.Range(0, 10000000);
            }

            Random.InitState(seed);
        }


        /// <summary>
        /// Deletes the child objects that form the maze in the editor
        /// </summary>
        public void DestroyMaze()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }
      

        /// <summary>
        /// Connects all of the different layers together. Each region gets at least 1 connection.
        /// </summary>
        private void ConnectRegions()
        {
            List<Connector> connectors = grid.GetConnectingCells();

            while (connectors.Count > 0)
            {
                int index = Random.Range(0, connectors.Count);
                Connector randomEgde = connectors[index];
                connectors.RemoveAt(index);

                if (randomEgde.IsOneRegion())
                {
                    continue;
                }

                randomEgde.currentCell.LinkCell(randomEgde.connectedCell, true);

                if (randomEgde.currentCell.InRoom)
                {
                    randomEgde.currentCell.isDoor = true;
                }
                else if (randomEgde.connectedCell.InRoom)
                {
                    randomEgde.connectedCell.isDoor = true;
                }

                int higherRegion;
                int lowerRegion;

                // prepare region variables
                if (randomEgde.currentCell.Region < randomEgde.connectedCell.Region)
                {
                    lowerRegion = randomEgde.currentCell.Region;
                    higherRegion = randomEgde.connectedCell.Region;
                }
                else
                {
                    lowerRegion = randomEgde.connectedCell.Region;
                    higherRegion = randomEgde.currentCell.Region;
                }

                for (int i = 0; i < connectors.Count; i++)
                {
                    // remove connectors connecting the same two regions
                    if (connectors[i].SameConnection(randomEgde.currentCell.Region, randomEgde.connectedCell.Region))
                    {
                        connectors.RemoveAt(i);
                        i--;
                    }
                    // anything connecting to the higher region
                    else if (connectors[i].ConnectsToRegion(higherRegion))
                    {
                        // make higher region the same as lower region
                        connectors[i].MergeRegions(higherRegion, lowerRegion);

                        // if both regions are the same, remove it
                        if (connectors[i].IsOneRegion())
                        {
                            connectors.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Removes dead ends by finding a cell with only one link and removing it, then doing the same to it's link until a
        /// cell with at least 2 links is found.
        /// </summary>
        private void RemoveDeadEnds()
        {
            Cell cell;

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    cell = grid.GetCell(column, row, 0);

                    if (cell != null)
                    {
                        if (cell.Links.Count == 1)
                        {
                            DeleteCorridor(cell);
                        }
                        else if (cell.Links.Count == 0)
                        {
                            cell.Mask = -1;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Removes each cell in a corridor that has only one link, starting with the end (the given cell).
        /// </summary>
        /// <param name="cell">cell should have only 1 link</param>
        private void DeleteCorridor(Cell cell)
        {
            while (cell.Links.Count == 1)
            {
                Cell neighbour = cell.Links[0];
                cell.RemoveNeighbours();
                cell.RemoveAllLinks();
                cell.Mask = -1;
                cell = neighbour;
            }
        }


        private void RunMazeGenAlgorithm(Algorithm algorithm, int currentMask = 0)
        {
            switch (algorithm)
            {
                case Algorithm.BinaryTree:
                    BinaryTree(grid);
                    break;
                case Algorithm.SideWinder:
                    SideWinder(grid);
                    break;
                case Algorithm.AldousBroder:
                    AldousBroder(grid, currentMask);
                    break;
                case Algorithm.Wilson:
                    Wilson(grid, currentMask);
                    break;
                case Algorithm.RecursiveBacktracker:
                    RecursiveBacktracker(grid, currentMask);
                    break;
                case Algorithm.HuntAndKill:
                    HuntAndKill(grid, currentMask);
                    break;
                case Algorithm.CreateRoom:
                    LinkAllCells(grid, currentMask);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Removes deadends by joining them with one of their neighbours they haven't yet linked to
        /// </summary>
        private void BraidMaze()
        {
            if (braidMaze)
            {
                grid.BraidMaze(braidPercentage);
            }
        }


        #region Maze Gen Algorithms


        #region Binary Tree
        /// <summary>
        /// Creates a grid using the Binary Tree method.
        /// </summary>
        /// <param name="grid"></param>
        private void BinaryTree(MazeGenGrid grid)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    List<Cell> neighbours = new List<Cell>();
                    Cell cell = grid.GetCell(column, row);
                    Cell neighbour;

                    // try to get north and east neighbours
                    if (cell.neighbours.TryGetValue(Cell.CardinalDirection.North, out neighbour))
                    {
                        neighbours.Add(neighbour);
                    }
                    if (cell.neighbours.TryGetValue(Cell.CardinalDirection.East, out neighbour))
                    {
                        neighbours.Add(neighbour);
                    }

                    if (neighbours.Count > 0)
                    {
                        int rInt = Random.Range(0, neighbours.Count);
                        cell.LinkCell(neighbours[rInt], true);
                    }

                }
            }
        }
        #endregion


        #region SideWinder
        private void SideWinder(MazeGenGrid grid)
        {
            List<Cell> cellsInRun = new List<Cell>();
            bool endRun = false;
            bool atEastBoundary = false;
            bool atNorthBoundary = false;

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    Cell cell = grid.GetCell(column, row);
                    cellsInRun.Add(cell);

                    if (!cell.neighbours.ContainsKey(Cell.CardinalDirection.North))
                    {
                        atNorthBoundary = true;
                    }
                    else
                    {
                        atNorthBoundary = false;
                    }

                    if (!cell.neighbours.ContainsKey(Cell.CardinalDirection.East))
                    {
                        atEastBoundary = true;
                    }
                    else
                    {
                        atEastBoundary = false;
                    }

                    int randomIndex = Random.Range(0, 2);

                    if (atEastBoundary || (!atNorthBoundary && randomIndex == 0))
                    {
                        endRun = true;
                    }
                    else
                    {
                        endRun = false;
                    }

                    if (endRun)
                    {
                        Cell member = cellsInRun[Random.Range(0, cellsInRun.Count)];

                        if (member.neighbours.ContainsKey(Cell.CardinalDirection.North))
                        {
                            member.LinkCell(member.neighbours[Cell.CardinalDirection.North], true);
                        }

                        cellsInRun.Clear();
                    }
                    else
                    {
                        cell.LinkCell(cell.neighbours[Cell.CardinalDirection.East], true);
                    }
                }
            }
        }
        #endregion


        #region Aldous-Broder
        private void AldousBroder(MazeGenGrid grid, int mask = 0)
        {
            int cellCount = grid.GetCellCount(mask);
            Cell currentCell = grid.GetRandomCell(mask);
            currentCell.Visited = true;
            cellCount--;

            while (cellCount > 0)
            {
                Cell neighbour = currentCell.GetRandomNeighbour(mask);

                if (!neighbour.Visited)
                {
                    currentCell.LinkCell(neighbour, true);
                    neighbour.Visited = true;
                    cellCount--;
                }

                currentCell = neighbour;
            }
        }
        #endregion


        #region Aldous-Broder Live
        private void AldousBroderLive(MazeGenGrid grid)
        {
            int cellCount = grid.GetCellCount();
            Cell currentCell = grid.GetRandomCell();
            currentCell.Visited = true;
            cellCount--;

            while (cellCount > 0)
            {
                Cell neighbour = currentCell.GetRandomNeighbour();

                if (!neighbour.Visited)
                {
                    currentCell.LinkCell(neighbour, true);
                    neighbour.Visited = true;
                    cellCount--;
                }

                currentCell = neighbour;
            }
        }
        #endregion


        #region Wilson
        private void Wilson(MazeGenGrid grid, int mask = 0)
        {
            int cellCount = grid.GetCellCount(mask);
            Cell currentCell = grid.GetRandomCell(mask);
            grid.GetRandomCell(mask).Visited = true;
            cellCount--;
            Stack<Cell> stack = new Stack<Cell>();

            while (cellCount > 0)
            {
                while (currentCell.Visited)
                {
                    currentCell = grid.GetRandomCell(mask);
                }

                stack.Push(currentCell);
                Cell neighbour = currentCell.GetRandomNeighbour(mask);

                if (neighbour.Visited)
                {
                    currentCell = neighbour;

                    for (int i = 0; i < stack.Count; i++)
                    {
                        currentCell.LinkCell(stack.Peek(), true);
                        currentCell = stack.Pop();
                        currentCell.Visited = true;
                        cellCount--;
                        i--;
                    }
                }
                else if (stack.Contains(neighbour))
                {
                    for (int i = 0; i < stack.Count; i++)
                    {
                        if (stack.Peek() == neighbour)
                        {
                            break;
                        }
                        else
                        {
                            stack.Pop();
                            i--;
                        }
                    }
                }
                else
                {
                    currentCell = neighbour;
                }
            }
        }
        #endregion


        #region Recursive Backtracker
        private void RecursiveBacktracker(MazeGenGrid grid, int mask = 0)
        {
            Stack<Cell> cells = new Stack<Cell>();
            cells.Push(grid.GetRandomCell(mask));
            cells.Peek().Visited = true;

            while (cells.Count > 0)
            {
                Cell currentCell = cells.Peek();
                Cell neighbour = currentCell.GetRandomUnvisitedNeighbour(mask);

                if (neighbour != null)
                {
                    currentCell.LinkCell(neighbour, true);
                    neighbour.Visited = true;
                    cells.Push(neighbour);
                }
                else
                {
                    cells.Pop();
                }
            }
        }
        private void RecursiveBacktracker(GridStruct grid, int mask = 0)
        {
            Stack<Cell> cells = new Stack<Cell>();
            // cells.Push(grid.GetRandomCell(mask));
            cells.Push(grid.grid[0, 0]);
            cells.Peek().Visited = true;

            while (cells.Count > 0)
            {
                Cell currentCell = cells.Peek();
                Cell neighbour = currentCell.GetRandomUnvisitedNeighbour(mask);

                if (neighbour != null)
                {
                    currentCell.LinkCell(neighbour, true);
                    neighbour.Visited = true;
                    cells.Push(neighbour);
                }
                else
                {
                    cells.Pop();
                }
            }
        }

        #endregion


        #region HuntAndKill
        private void HuntAndKill(MazeGenGrid grid, int mask = 0)
        {
            int cellCount = grid.GetCellCount(mask);
            Cell currentCell = grid.GetRandomCell(mask);
            currentCell.Visited = true;
            cellCount--;

            while (cellCount > 0)
            {
                Cell neighbour = currentCell.GetRandomUnvisitedNeighbour(mask);

                if (neighbour != null)
                {
                    currentCell.LinkCell(neighbour, true);
                    neighbour.Visited = true;
                    cellCount--;
                    currentCell = neighbour;
                }
                else
                {
                    if (cellCount > 0)
                    {
                        currentCell = grid.GetUnvisitedCell(true, mask);
                        currentCell.Visited = true;
                        neighbour = currentCell.GetRandomVisitedNeighbour(mask);
                        currentCell.LinkCell(neighbour, true);
                        cellCount--;
                    }
                }
            }
        }
        #endregion


        #region Link All Cells
        /// <summary>
        /// Links all the cells on a given mask.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="mask"></param>
        private void LinkAllCells(MazeGenGrid grid, int mask = 0)
        {
            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    // will be null if the cell isn't on the mask
                    Cell cell = grid.GetCell(column, row, mask);

                    if (cell != null)
                    {
                        cell.InRoom = true;
                        List<Cell> neighbours = cell.GetNeighbours(mask);

                        for (int i = 0; i < neighbours.Count; i++)
                        {
                            if (!cell.IsLinked(neighbours[i]))
                            {
                                cell.LinkCell(neighbours[i], true);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #endregion

    }

}