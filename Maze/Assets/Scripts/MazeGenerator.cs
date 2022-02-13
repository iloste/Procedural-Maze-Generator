using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Algorithm algorithm;
    [SerializeField] Colouring colouring;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] bool braidMaze;
    [Tooltip("100% = no dead ends.")]
    [SerializeField] [Range(0, 100)] int braidPercentage;
    [SerializeField] bool displayDeadEnds;
    [SerializeField] int currentMask;
    [SerializeField] MazeDisplay mazeDisplay;
    [SerializeField] bool useCuboidMaze;
    [SerializeField] Texture2D layerImage;
    [SerializeField] Color[] layerColours;

    MyGrid grid;
    Pathfinding pf;


    public enum Algorithm
    {
        BinaryTree,
        SideWinder,
        AldousBroder,
        Wilson,
        AldousBroderLive,
        RecursiveBacktracker,
        HuntAndKill,
        LinkAllCells,
    }

    public enum Colouring
    {
        None,
        PathBetween,
        LongestPath,
        ColourMaze,
    }

    // Start is called before the first frame update
    void Start()
    {


        if (!useCuboidMaze)
        {

            grid = new MyGrid(gridSize.x, gridSize.y);
            GenerateMaze(algorithm, currentMask);

            //Color[] bitmap = layerImage.GetPixels();
            //grid = new MyGrid(bitmap, layerColours, layerImage.width, layerImage.height);

            //GenerateMaze(algorithm, 1);
            //GenerateMaze(Algorithm.LinkAllCells, 2);

            BraidMaze();
            mazeDisplay.DisplayGrid(grid);
            //DisplayGrid(grid);
            //Pathfinding();
            //DisplayDeadEnds();
        }
        else
        {
            CuboidGrid cuboidGrid = new CuboidGrid(20, 20);
            RecursiveBacktracker(cuboidGrid.GetGrid(0));

            // don't hard code the 6
            for (int i = 0; i < 6; i++)
            {
                mazeDisplay.DisplayGrid(cuboidGrid.GetGrid(i), i);
            }

            mazeDisplay.OrientateSurfaces();
            mazeDisplay.PositionSurfaces(cuboidGrid.GetGrid(0).columns, cuboidGrid.GetGrid(0).rows);
            //pf = new Pathfinding(cuboidGrid.GetGrid(0).columns, cuboidGrid.GetGrid(0).rows);
            //DisplayPath(pf.ShortestPath(cuboidGrid.GetGrid(0).grid[0, 0], cuboidGrid.GetGrid(4).grid[0, 0], 0));
        }
    }


    #region Start Functions
    private void GenerateMaze(Algorithm algorithm, int currentMask = 0)
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
            case Algorithm.LinkAllCells:
                LinkAllCells(grid, currentMask);
                break;
            default:
                break;
        }
    }


    private void BraidMaze()
    {
        if (braidMaze)
        {
            grid.BraidMaze(braidPercentage);
        }
    }


    private void Pathfinding()
    {
        pf = new Pathfinding(grid.Columns, grid.Rows);

        switch (colouring)
        {
            case Colouring.None:
                break;
            case Colouring.PathBetween:
                DisplayPath(pf.ShortestPath(grid.GetCell(0, grid.Rows - 1), grid.GetCell(grid.Columns - 1, 0)));
                break;
            case Colouring.LongestPath:
                DisplayPath(pf.FindLongestPath(grid, grid.GetCell(0, 0)));
                break;
            case Colouring.ColourMaze:
                pf.FloodGrid(grid.GetCell(grid.Columns / 2, grid.Rows / 2));
                DisplayGridColour(grid, pf.maxDistance);
                break;
            default:
                break;
        }
    }


    ///To do: create a seperate class for displaying grids?
    void DisplayGrid(MyGrid grid)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                Cell cell = grid.GetCell(column, row);

                //  if (cell != null && grid.CellValid(cell))
                if (cell != null)
                {
                    Tile tile = Instantiate(tilePrefab, new Vector3(column * 1.4f, 0, row * 1.4f), Quaternion.identity).GetComponent<Tile>();
                    cell.Tile = tile;

                    if (cell.IsLinked(Cell.Direction.North))
                    {
                        tile.DeactivateWall(Cell.Direction.North);
                    }
                    if (cell.IsLinked(Cell.Direction.South))
                    {
                        tile.DeactivateWall(Cell.Direction.South);
                    }
                    if (cell.IsLinked(Cell.Direction.East))
                    {
                        tile.DeactivateWall(Cell.Direction.East);
                    }
                    if (cell.IsLinked(Cell.Direction.West))
                    {
                        tile.DeactivateWall(Cell.Direction.West);
                    }
                }
            }
        }
    }


    void DisplayGridColour(MyGrid grid, int maxDistance)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                if (grid.CellValid(column, row))
                {
                    Cell cell = grid.GetCell(column, row);
                    float normVal = (float)pf.GetDistanceFromOrigin(cell) / maxDistance;
                    cell.Tile.floor.GetComponent<MeshRenderer>().material.color = new Color(0, 1 - normVal, 0);
                }
            }
        }
    }


    void DisplayPath(Stack<Cell> path)
    {
        int maxDistance = path.Count;

        while (path.Count > 0)
        {
            Cell cell = path.Pop();
            float normVal = (float)pf.GetDistanceFromOrigin(cell) / maxDistance;
            GameObject floor = cell.Tile.floor;
            floor.GetComponent<MeshRenderer>().material.color = new Color(0, 1 - normVal, 0);
        }
    }


    void DisplayDeadEnds()
    {
        if (displayDeadEnds)
        {
            List<Cell> deadEnds = grid.GetDeadEnds();

            for (int i = 0; i < deadEnds.Count; i++)
            {
                deadEnds[i].Tile.floor.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
            }
        }
    }

    #endregion


    #region Binary Tree
    /// <summary>
    /// Creates a grid using the Binary Tree method.
    /// </summary>
    /// <param name="grid"></param>
    private void BinaryTree(MyGrid grid)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                List<Cell> neighbours = new List<Cell>();
                Cell cell = grid.GetCell(column, row);
                Cell neighbour;

                // try to get north and east neighbours
                if (cell.neighbours.TryGetValue(Cell.Direction.North, out neighbour))
                {
                    neighbours.Add(neighbour);
                }
                if (cell.neighbours.TryGetValue(Cell.Direction.East, out neighbour))
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
    private void SideWinder(MyGrid grid)
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

                if (!cell.neighbours.ContainsKey(Cell.Direction.North))
                {
                    atNorthBoundary = true;
                }
                else
                {
                    atNorthBoundary = false;
                }

                if (!cell.neighbours.ContainsKey(Cell.Direction.East))
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

                    if (member.neighbours.ContainsKey(Cell.Direction.North))
                    {
                        member.LinkCell(member.neighbours[Cell.Direction.North], true);
                    }

                    cellsInRun.Clear();
                }
                else
                {
                    cell.LinkCell(cell.neighbours[Cell.Direction.East], true);
                }
            }
        }
    }
    #endregion


    #region Aldous-Broder
    private void AldousBroder(MyGrid grid, int mask = 0)
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
    private void AldousBroderLive(MyGrid grid)
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
    private void Wilson(MyGrid grid, int mask = 0)
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
    private void RecursiveBacktracker(MyGrid grid, int mask = 0)
    {
        Stack<Cell> cells = new Stack<Cell>();
        cells.Push(grid.GetRandomCell(mask));
        cells.Peek().Visited = true;

        while (cells.Count > 0)
        {
            Cell currentCell = cells.Peek();
            Cell neighbour = currentCell.RandomUnvisitedNeighbour(mask);

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
            Cell neighbour = currentCell.RandomUnvisitedNeighbour(mask);

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
    private void HuntAndKill(MyGrid grid, int mask = 0)
    {
        int cellCount = grid.GetCellCount(mask);
        Cell currentCell = grid.GetRandomCell(mask);
        currentCell.Visited = true;
        cellCount--;

        while (cellCount > 0)
        {
            Cell neighbour = currentCell.RandomUnvisitedNeighbour(mask);

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
    private void LinkAllCells(MyGrid grid, int mask = 0)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                // will be null if the cell isn't on the mask
                Cell cell = grid.GetCell(column, row, mask);

                if (cell != null)
                {
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

}
