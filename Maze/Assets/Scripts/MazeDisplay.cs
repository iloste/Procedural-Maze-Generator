using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeDisplay : MonoBehaviour
{
    [SerializeField] GameObject[] mazeTiles;
    [SerializeField] GameObject[] roomTiles;


    enum TileType
    {
        Default,
        MDeadEnd,
        MCorner,
        MThroughPassage,
        MCrossJunction,
        MTJunction,
        RCorner,
        RWall,
        RDeadEnd,
        REmpty,
    }
    

    private TileType GetTileType(Cell cell)
    {
        int linksCount = cell.Links.Count;

        #region If cell is in room
        if (!cell.InRoom)
        {
            if (linksCount == 1)
            {
                return TileType.MDeadEnd;
            }
            else if (linksCount == 2)
            {
                if ((cell.IsLinked(Cell.Direction.North) && cell.IsLinked(Cell.Direction.South)) ||
                    (cell.IsLinked(Cell.Direction.East) && cell.IsLinked(Cell.Direction.West)))
                {
                    return TileType.MThroughPassage;
                }
                else
                {
                    return TileType.MCorner;
                }
            }
            else if (linksCount == 3)
            {
                return TileType.MTJunction;
            }
            else if (linksCount == 4)
            {
                return TileType.MCrossJunction;
            }
        }
        #endregion
        #region If cell is NOT in room
        else
        {
            if (linksCount == 1)
            {
                return TileType.RDeadEnd;
            }
            else if (linksCount == 2)
            {
                return TileType.RCorner;
            }
            else if (linksCount == 3)
            {
                return TileType.RWall;
            }
            else
            {
                return TileType.REmpty;
            }
        }
        #endregion

        return TileType.Default;
    }

    public void DisplayGrid(MyGrid grid, float xzScale, float yScale)
    {
        for (int row = 0; row < grid.Rows; row++)
        {
            for (int column = 0; column < grid.Columns; column++)
            {
                Cell cell = grid.GetCell(column, row);

                if (cell != null)
                {
                    TileType tileType = GetTileType(cell);
                    Tile tile = null;

                    switch (tileType)
                    {
                        case TileType.Default:
                            Debug.LogError("Default Tile Selected");
                            break;
                        #region Deadend
                        case TileType.MDeadEnd:
                            tile = Instantiate(mazeTiles[1], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();
                            
                            if (cell.IsLinked(Cell.Direction.North))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.East))
                            {
                                tile.transform.eulerAngles = new Vector3(0, -90, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.South))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 0, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.West))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            break;
                        #endregion
                        #region Corner
                        case TileType.MCorner:
                            tile = Instantiate(mazeTiles[0], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (cell.IsLinked(Cell.Direction.North))
                            {
                                if (cell.IsLinked(Cell.Direction.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.Direction.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -180, 0);
                                }
                            }
                            else
                            {
                                if (cell.IsLinked(Cell.Direction.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                else if (cell.IsLinked(Cell.Direction.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                            }
                            break;
                        #endregion
                        #region Through Pass
                        case TileType.MThroughPassage:
                            tile = Instantiate(mazeTiles[2], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (cell.IsLinked(Cell.Direction.North))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 0, 0);
                            }
                            else
                            {
                                tile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            break;
                        #endregion
                        #region Cross Junction
                        case TileType.MCrossJunction:
                            tile = Instantiate(mazeTiles[4], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();
                            break;
                        #endregion
                        #region TJunction
                        case TileType.MTJunction:
                            tile = Instantiate(mazeTiles[3], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (!cell.IsLinked(Cell.Direction.North))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.East))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.South))
                            {
                                tile.transform.eulerAngles = new Vector3(0, -90, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.West))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 0, 0);
                            }
                            break;
                        #endregion
                        #region RWall
                        case TileType.RWall:
                            tile = Instantiate(roomTiles[2], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (!cell.IsLinked(Cell.Direction.North))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.East))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.South))
                            {
                                tile.transform.eulerAngles = new Vector3(0,-90, 0);
                            }
                            else if (!cell.IsLinked(Cell.Direction.West))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 0, 0);
                            }
                            break;
                        #endregion
                        #region RCorner
                        case TileType.RCorner:
                            tile = Instantiate(roomTiles[1], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (cell.IsLinked(Cell.Direction.North))
                            {
                                if (cell.IsLinked(Cell.Direction.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.Direction.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -180, 0);
                                }
                            }
                            else
                            {
                                if (cell.IsLinked(Cell.Direction.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                else if (cell.IsLinked(Cell.Direction.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                            }
                            break;
                        #endregion
                        #region RDeadEnd
                        case TileType.RDeadEnd:
                            tile = Instantiate(roomTiles[0], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();

                            if (cell.IsLinked(Cell.Direction.North))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 180, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.East))
                            {
                                tile.transform.eulerAngles = new Vector3(0, -90, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.South))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 0, 0);
                            }
                            else if (cell.IsLinked(Cell.Direction.West))
                            {
                                tile.transform.eulerAngles = new Vector3(0, 90, 0);
                            }
                            break;
                        #endregion
                        #region REmpty
                        case TileType.REmpty:
                            tile = Instantiate(roomTiles[3], new Vector3(column * xzScale, 0, row * xzScale), Quaternion.identity, transform).GetComponent<Tile>();
                            break;
                        #endregion
                        default:
                            break;
                    }

                    tile.SetScale(xzScale, yScale);
                    cell.Tile = tile;
                }
            }
        }
    }

    #region 3D maze

    //float tileWidth = 1.4f;
    //float floorDepth = 0.18f;

    //public void DisplayGrid(GridStruct grid, int direction)
    //{
    //    for (int row = 0; row < grid.columns; row++)
    //    {
    //        for (int column = 0; column < grid.rows; column++)
    //        {
    //            Cell cell = grid.grid[row, column];

    //            //  if (cell != null && grid.CellValid(cell))
    //            if (cell != null)
    //            {
    //                Tile tile = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f) + cubeSurfaces[direction].position, Quaternion.identity, cubeSurfaces[direction]).GetComponent<Tile>();
    //                //Tile tile = Instantiate(tilePrefab, new Vector3(row * 1.4f, 0, column * 1.4f), Quaternion.identity).GetComponent<Tile>();
    //                cell.Tile = tile;

    //                if (cell.IsLinked(Cell.Direction.North))
    //                {
    //                    tile.DeactivateWall(Cell.Direction.North);
    //                }
    //                if (cell.IsLinked(Cell.Direction.South))
    //                {
    //                    tile.DeactivateWall(Cell.Direction.South);
    //                }
    //                if (cell.IsLinked(Cell.Direction.East))
    //                {
    //                    tile.DeactivateWall(Cell.Direction.East);
    //                }
    //                if (cell.IsLinked(Cell.Direction.West))
    //                {
    //                    tile.DeactivateWall(Cell.Direction.West);
    //                }
    //            }
    //        }
    //    }
    //}

    //public void PositionSurfaces(int columns, int rows)
    //{
    //    cubeSurfaces[0].position = new Vector3(0, -tileWidth / 2 + floorDepth / 2, tileWidth * rows - tileWidth / 2 - floorDepth / 2);
    //    cubeSurfaces[1].position = new Vector3(0, 0, 0);
    //    cubeSurfaces[2].position = new Vector3(0, (-rows * tileWidth) + tileWidth / 2 + floorDepth / 2, -tileWidth / 2 + floorDepth / 2);
    //    cubeSurfaces[3].position = new Vector3(0, (-rows * tileWidth) + floorDepth, (tileWidth * rows) - tileWidth);
    //    cubeSurfaces[4].position = new Vector3(-tileWidth / 2 + floorDepth / 2, (-rows * tileWidth) + tileWidth / 2 + floorDepth / 2, 0);
    //    cubeSurfaces[5].position = new Vector3(tileWidth * rows - tileWidth / 2 - floorDepth / 2, -tileWidth / 2 + floorDepth / 2, 0);
    //}

    //public void OrientateSurfaces()
    //{
    //    cubeSurfaces[0].eulerAngles = new Vector3(90, 0, 0);
    //    cubeSurfaces[1].eulerAngles = new Vector3(0, 0, 0);
    //    cubeSurfaces[2].eulerAngles = new Vector3(-90, 0, 0);
    //    cubeSurfaces[3].eulerAngles = new Vector3(180, 0, 0);
    //    cubeSurfaces[4].eulerAngles = new Vector3(0, 0, 90);
    //    cubeSurfaces[5].eulerAngles = new Vector3(0, 0, -90);

    //}

    #endregion

}
