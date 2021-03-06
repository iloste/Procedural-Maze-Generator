using UnityEngine;
using UnityEditor;

namespace PMG
{
    public class MazeDisplay : MonoBehaviour
    {
        [Header("Maze Tiles")]
        [SerializeField] GameObject deadendMaze;
        [SerializeField] GameObject cornerMaze;
        [SerializeField] GameObject throughPassMaze;
        [SerializeField] GameObject crossJunctionMaze;
        [SerializeField] GameObject tJunctionMaze;
        
        [Space(5)]
        [Header("Room Tiles")]
        [SerializeField] GameObject deadendRoom;
        [SerializeField] GameObject cornerRoom;
        [SerializeField] GameObject throughPassRoom;
        [SerializeField] GameObject tJunctionRoom;
        [SerializeField] GameObject crossJunctionRoom;
        [SerializeField] GameObject tJunctionDoorRoom;
        [SerializeField] GameObject cornerDoorRoom;


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
            RTDoor,
            RCornerDoor,
            RThroughPass
        }


        /// <summary>
        /// Returns TileType based on the number of links and if the cell is considered in a room/hallway, or is a door.
        /// </summary>
        private TileType GetTileType(Cell cell)
        {
            int linksCount = cell.Links.Count;

            #region If cell is NOT in room
            if (!cell.InRoom)
            {
                if (linksCount == 1)
                {
                    return TileType.MDeadEnd;
                }
                else if (linksCount == 2)
                {
                    if ((cell.IsLinked(Cell.CardinalDirection.North) && cell.IsLinked(Cell.CardinalDirection.South)) ||
                        (cell.IsLinked(Cell.CardinalDirection.East) && cell.IsLinked(Cell.CardinalDirection.West)))
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
            #region If cell is in room
            else
            {
                if (!cell.isDoor)
                {
                    if (linksCount == 1)
                    {
                        return TileType.RDeadEnd;
                    }
                    else if (linksCount == 2)
                    {
                        if ((cell.IsLinked(Cell.CardinalDirection.North) && cell.IsLinked(Cell.CardinalDirection.South)) ||
                            (cell.IsLinked(Cell.CardinalDirection.East) && cell.IsLinked(Cell.CardinalDirection.West)))
                        {
                            return TileType.RThroughPass;
                        }
                        else
                        {
                            return TileType.RCorner;
                        }
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
                else
                {
                    if (linksCount == 4)
                    {
                        return TileType.RTDoor;
                    }
                    else if (linksCount == 3)
                    {
                        return TileType.RCornerDoor;
                    }
                }
            }
            #endregion

            Debug.LogError("Cell does not have a TileType!");
            return TileType.Default;
        }


        /// <summary>
        /// Generates the GameObjects needed to display the grid in the editor. 
        /// The tiles will be children of this gameobject
        /// </summary>
        /// <param name="grid">Instance of the grid being displayed</param>
        /// <param name="xzScale">Must be greater than 0</param>
        /// <param name="yScale">Must be greater than 0</param>
        public void DisplayGrid(MazeGenGrid grid, float xzScale, float yScale)
        {
            if (grid == null)
            {
                throw new System.NullReferenceException("grid is null");
            }

            if (xzScale <= 0)
            {
                xzScale = 1;
            }
            if (yScale <= 0)
            {
                yScale = 1;
            }

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
                                tile = Instantiate(deadendMaze, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.South))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                break;
                            #endregion
                            #region Corner
                            case TileType.MCorner:
                                tile = Instantiate(cornerMaze, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    if (cell.IsLinked(Cell.CardinalDirection.East))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                    }
                                    else if (cell.IsLinked(Cell.CardinalDirection.West))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, -180, 0);
                                    }
                                }
                                else
                                {
                                    if (cell.IsLinked(Cell.CardinalDirection.East))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                    }
                                    else if (cell.IsLinked(Cell.CardinalDirection.West))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                    }
                                }
                                break;
                            #endregion
                            #region Through Pass
                            case TileType.MThroughPassage:
                                tile = Instantiate(throughPassMaze, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
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
                                tile = Instantiate(crossJunctionMaze, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();
                                break;
                            #endregion
                            #region TJunction
                            case TileType.MTJunction:
                                tile = Instantiate(tJunctionMaze, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (!cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.South))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            #endregion
                            #region RWall
                            case TileType.RWall:
                                tile = Instantiate(tJunctionRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (!cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.South))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (!cell.IsLinked(Cell.CardinalDirection.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            #endregion
                            #region RCorner
                            case TileType.RCorner:
                                tile = Instantiate(cornerRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    if (cell.IsLinked(Cell.CardinalDirection.East))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                    }
                                    else if (cell.IsLinked(Cell.CardinalDirection.West))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, -180, 0);
                                    }
                                }
                                else
                                {
                                    if (cell.IsLinked(Cell.CardinalDirection.East))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                    }
                                    else if (cell.IsLinked(Cell.CardinalDirection.West))
                                    {
                                        tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                    }
                                }
                                break;
                            #endregion
                            #region RDeadEnd
                            case TileType.RDeadEnd:
                                tile = Instantiate(deadendRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.South))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.West))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                break;
                            #endregion
                            #region REmpty
                            case TileType.REmpty:
                                tile = Instantiate(crossJunctionRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();
                                break;
                            #endregion
                            #region RTDoor
                            case TileType.RTDoor:
                                tile = Instantiate(tJunctionDoorRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North) && !cell.neighbours[Cell.CardinalDirection.North].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.East) && !cell.neighbours[Cell.CardinalDirection.East].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.South) && !cell.neighbours[Cell.CardinalDirection.South].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.West) && !cell.neighbours[Cell.CardinalDirection.West].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            #endregion
                            #region RCornerDoor
                            case TileType.RCornerDoor:
                                tile = Instantiate(cornerDoorRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North) && !cell.neighbours[Cell.CardinalDirection.North].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.East) && !cell.neighbours[Cell.CardinalDirection.East].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 180, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.South) && !cell.neighbours[Cell.CardinalDirection.South].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, -90, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.West) && !cell.neighbours[Cell.CardinalDirection.West].InRoom)
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                break;
                            #endregion
                            case TileType.RThroughPass:
                                tile = Instantiate(throughPassRoom, new Vector3(column * xzScale, 0, row * xzScale) + transform.position, Quaternion.identity, transform).GetComponent<Tile>();

                                if (cell.IsLinked(Cell.CardinalDirection.North))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 0, 0);
                                }
                                else if (cell.IsLinked(Cell.CardinalDirection.East))
                                {
                                    tile.transform.eulerAngles = new Vector3(0, 90, 0);
                                }

                                break;
                            default:
                                break;
                        }

                        if (tile != null)
                        {
                            //integrates changes into Unity's undo system.
                            Undo.RegisterCreatedObjectUndo(tile.gameObject, "Object Created");
                            Undo.RecordObject(tile.gameObject, "Set Scale");
                            tile.SetScale(xzScale, yScale);
                            Undo.RecordObject(tile.gameObject, "Set Tile Variable");
                            cell.Tile = tile; 
                        }
                    }
                }
            }
        }
    }
}