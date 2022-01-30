using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] GameObject tilePrefab;
    [SerializeField] Algorithm algorithm;
    [SerializeField] Colouring colouring;
    [SerializeField] Vector2Int gridSize;
    [SerializeField] bool braidMaze;
    [Tooltip("100% = no dead ends.")]
    [SerializeField] int braidPercentage;
    [SerializeField] bool displayDeadEnds;

    MyGrid grid;
    Pathfinding pf;


    public enum Algorithm
    {
        BinaryTree,
        SideWinder,
        AldousBroder,
        Wilson,
        AldousBroderLive,
    }

    public enum Colouring
    {
        PathBetween,
        LongestPath,
        ColourMaze,
    }

    // Start is called before the first frame update
    void Start()
    {
        // grid = new MyGrid("Assets/mazes/maze.txt");
        grid = new MyGrid(gridSize.x, gridSize.y);

        switch (algorithm)
        {
            case Algorithm.BinaryTree:
                BinaryTree(grid);
                break;
            case Algorithm.SideWinder:
                SideWinder(grid);
                break;
            case Algorithm.AldousBroder:
                AldousBroder(grid);
                break;
            case Algorithm.Wilson:
                Wilson(grid);
                break;
            default:
                break;
        }


        pf = new Pathfinding(grid.Rows, grid.Columns);

        if (braidMaze)
        {
            grid.BraidMaze(braidPercentage);
        }

        DisplayGrid(grid);


        switch (colouring)
        {
            case Colouring.PathBetween:
                DisplayPath(pf.ShortestPath(grid.GetCell(0, grid.Rows - 1), grid.GetCell(grid.Columns - 1, 0)));
                break;
            case Colouring.LongestPath:
                DisplayPath(pf.FindLongestPath(grid, grid.GetCell(0, 0)));
                break;
            case Colouring.ColourMaze:
                pf.FloodGrid(grid.GetCell(grid.Rows / 2, grid.Columns / 2));
                DisplayGridColour(grid, pf.maxDistance);
                break;
            default:
                break;
        }

        if (displayDeadEnds)
        {
            DisplayDeadEnds();
        }
    }




    ///To do: create a seperate class for displaying grids?
    void DisplayGrid(MyGrid grid)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                Cell cell = grid.GetCell(row, column);

                //  if (cell != null && grid.CellValid(cell))
                if (cell != null)
                {
                    Tile tile = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f), Quaternion.identity).GetComponent<Tile>();
                    cell.Tile = tile;

                    if (cell.IsLinked(Cell.Direction.North))
                    {
                        tile.northWall.SetActive(false);
                    }
                    if (cell.IsLinked(Cell.Direction.South))
                    {
                        tile.southWall.SetActive(false);
                    }
                    if (cell.IsLinked(Cell.Direction.East))
                    {
                        tile.eastWall.SetActive(false);
                    }
                    if (cell.IsLinked(Cell.Direction.West))
                    {
                        tile.westWall.SetActive(false);
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
                if (grid.CellValid(row, column))
                {
                    Cell cell = grid.GetCell(row, column);
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
            cell.Tile.floor.GetComponent<MeshRenderer>().material.color = new Color(0, 1 - normVal, 0);
        }

    }

    void DisplayDeadEnds()
    {
        List<Cell> deadEnds = grid.GetDeadEnds();

        for (int i = 0; i < deadEnds.Count; i++)
        {
            deadEnds[i].Tile.floor.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);

        }
    }



    #region Binary Tree
    /// <summary>
    /// Creates a grid using the Binary Tree method.
    /// </summary>
    /// <param name="grid"></param>
    static void BinaryTree(MyGrid grid)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                List<Cell> neighbours = new List<Cell>();
                Cell cell = grid.GetCell(row, column);
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
    static void SideWinder(MyGrid grid)
    {
        List<Cell> cellsInRun = new List<Cell>();
        bool endRun = false;
        bool atEastBoundary = false;
        bool atNorthBoundary = false;

        for (int row = 0; row < grid.Rows; row++)
        {

            for (int column = 0; column < grid.Columns; column++)
            {
                Cell cell = grid.GetCell(row, column);
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
                    // if (cell.neighbours.ContainsKey(Cell.Direction.East))
                    {
                        cell.LinkCell(cell.neighbours[Cell.Direction.East], true);
                    }
                }

            }
        }
    }
    #endregion


    #region Aldous-Broder
    static void AldousBroder(MyGrid grid)
    {
        int cellCount = grid.Count;
        Cell currentCell = grid.RandomCell();
        currentCell.Visted = true;
        cellCount--;

        while (cellCount > 0)
        {
            Cell neighbour = currentCell.RandomNeighbour();

            if (!neighbour.Visted)
            {
                currentCell.LinkCell(neighbour, true);
                neighbour.Visted = true;
                cellCount--;
            }

            currentCell = neighbour;
        }
    }
    #endregion
    #region Aldous-Broder Live
    static void AldousBroderLive(MyGrid grid)
    {
        int cellCount = grid.Count;
        Cell currentCell = grid.RandomCell();
        currentCell.Visted = true;
        cellCount--;

        while (cellCount > 0)
        {
            Cell neighbour = currentCell.RandomNeighbour();

            if (!neighbour.Visted)
            {
                currentCell.LinkCell(neighbour, true);
                neighbour.Visted = true;
                cellCount--;
            }

            currentCell = neighbour;
        }
    }
    #endregion

    #region Wilson
    static void Wilson(MyGrid grid)
    {
        int cellCount = grid.Count;
        Cell currentCell = grid.RandomCell();
        grid.RandomCell().Visted = true;
        cellCount--;
        Stack<Cell> stack = new Stack<Cell>();

        while (cellCount > 0)
        {
            while (currentCell.Visted)
            {
                currentCell = grid.RandomCell();
            }
            stack.Push(currentCell);

            Cell neighbour = currentCell.RandomNeighbour();

            if (neighbour.Visted)
            {
                currentCell = neighbour;

                for (int i = 0; i < stack.Count; i++)
                {
                    currentCell.LinkCell(stack.Peek(), true);
                    currentCell = stack.Pop();
                    currentCell.Visted = true;
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

}
