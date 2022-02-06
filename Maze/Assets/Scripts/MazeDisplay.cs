using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDisplay : MonoBehaviour
{
    [SerializeField] Transform[] cubeSurfaces;
    [SerializeField] GameObject tilePrefab;

    float tileWidth = 1.4f;
    float floorDepth = 0.18f;

    // Start is called before the first frame update
    void Start()
    {
        // TestGrid();
    }

    public void DisplayGrid(GridStruct grid, int direction)
    {
        for (int row = 0; row < grid.columns; row++)
        {
            for (int column = 0; column < grid.rows; column++)
            {
                Cell cell = grid.grid[row, column];

                //  if (cell != null && grid.CellValid(cell))
                if (cell != null)
                {
                    Tile tile = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f) + cubeSurfaces[direction].position, Quaternion.identity, cubeSurfaces[direction]).GetComponent<Tile>();
                    //Tile tile = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f), Quaternion.identity).GetComponent<Tile>();
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

    public void PositionSurfaces(int columns, int rows)
    {
        cubeSurfaces[0].position = new Vector3(0, -tileWidth / 2 + floorDepth / 2, tileWidth * rows - tileWidth / 2 - floorDepth / 2);
        cubeSurfaces[1].position = new Vector3(0, 0, 0);
        cubeSurfaces[2].position = new Vector3(0, (-rows * tileWidth) + tileWidth / 2 + floorDepth / 2, -tileWidth / 2 + floorDepth / 2);
        cubeSurfaces[3].position = new Vector3(0, (-rows * tileWidth) + floorDepth, (tileWidth * rows) - tileWidth);
        cubeSurfaces[4].position = new Vector3(-tileWidth / 2 + floorDepth / 2, (-rows * tileWidth) + tileWidth / 2 + floorDepth / 2, 0);
        cubeSurfaces[5].position = new Vector3(tileWidth * rows - tileWidth / 2 - floorDepth / 2, -tileWidth / 2 + floorDepth / 2, 0);
    }

    public void OrientateSurfaces()
    {
        cubeSurfaces[0].eulerAngles = new Vector3(90, 0, 0);
        cubeSurfaces[1].eulerAngles = new Vector3(0, 0, 0);
        cubeSurfaces[2].eulerAngles = new Vector3(-90, 0, 0);
        cubeSurfaces[3].eulerAngles = new Vector3(180, 0, 0);
        cubeSurfaces[4].eulerAngles = new Vector3(0, 0, 90);
        cubeSurfaces[5].eulerAngles = new Vector3(0, 0, -90);

    }

    private void TestGrid()
    {
        Tile[,] cells = new Tile[2, 4];
        float i = 0;

        for (int row = 0; row < cells.GetLength(0); row++)
        {
            for (int column = 0; column < cells.GetLength(1); column++)
            {
                cells[row, column] = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f) + new Vector3(5, 0, 5), Quaternion.identity).GetComponent<Tile>();

            }
        }



        for (int row = 0; row < cells.GetLength(1); row++)
        {
            for (int column = 0; column < cells.GetLength(0); column++)
            {
                cells[column, row].floor.GetComponent<MeshRenderer>().material.color = new Color(i, 0, 0);
                i += 0.1f;
            }
        }


        //cells[0, 0].floor.GetComponent<MeshRenderer>().material.color = new Color(i, 0, 0);
        //cells[1, 0].floor.GetComponent<MeshRenderer>().material.color = new Color(i, 0, 0);
        //cells[2, 0].floor.GetComponent<MeshRenderer>().material.color = new Color(i, 0, 0);


    }


}
