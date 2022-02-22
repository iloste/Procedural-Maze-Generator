using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace MGT
{
    public class Cell
    {
        public enum CardinalDirection
        {
            Default,
            North,
            South,
            East,
            West,
        }


        #region Variables
        public int Row { get; private set; }
        public int Column { get; private set; }

        public Dictionary<CardinalDirection, Cell> neighbours = new Dictionary<CardinalDirection, Cell>();
        /// <summary>
        /// Cells that form a passageway with the current cell
        /// </summary>
        public List<Cell> Links { get; private set; }
        public bool Visited { get; set; } = false;
        public int Mask { get; set; }
        public Tile Tile { get; set; }
        public int GridNum { get; set; }
        public bool InRoom { get; set; }
        public int Region { get; set; }
        public bool isDoor { get; set; }

        #endregion


        #region Constructors
        public Cell(int column, int row)
        {
            this.Row = row;
            this.Column = column;
            Links = new List<Cell>();
        }


        public Cell(int row, int column, int gridNum) : this(row, column)
        {
            GridNum = gridNum;
        }

        #endregion


        #region Linked cell functions

        /// <summary>
        /// Links this cell to the given cell so that they will form a passageway.
        /// </summary>
        /// <param name="newLink">The cell to be linked to</param>
        /// <param name="bidi">If true, will call the function again from the given cell</param>
        public void LinkCell(Cell newLink, bool bidi)
        {
            if (newLink != null)
            {
                Links.Add(newLink);

                if (bidi)
                {
                    newLink.LinkCell(this, false);
                }
            }
        }


        /// <summary>
        /// The cells will no longer form a passageway. This does not remove them as neighbours.
        /// </summary>
        /// <param name="cell">The cell to unlink from</param>
        /// <param name="bidi">If true, will call the function again from the given cell</param>
        public void UnlinkCell(Cell cell, bool bidi)
        {
            if (cell != null)
            {
                Links.Remove(cell);

                if (bidi)
                {
                    cell.UnlinkCell(this, false);
                }
            }
        }

        
        public bool IsLinked(Cell cell)
        {
            if (cell != null)
            {
                foreach (Cell c in Links)
                {
                    if (c != null)
                    {
                        if (c == cell)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }


        /// <summary>
        /// Checks if the cell is linked to the neighbour of the given direction
        /// </summary>
        public bool IsLinked(CardinalDirection direction)
        {
            if (direction != CardinalDirection.Default)
            {
                if (neighbours.ContainsKey(direction))
                {
                    if (Links.Contains(neighbours[direction]))
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        public void RemoveAllLinks()
        {
            for (int i = 0; i < Links.Count; i++)
            {
                Links[i].UnlinkCell(this, true);
            }
        }

        #endregion


        #region Neighbour Functions

        public void SetNeighour(Cell newNeighbour, CardinalDirection direction)
        {
            if (newNeighbour == null)
            {
                return;
            }
            else if (newNeighbour.Mask == -1 || Mask == -1)
            {
                return;
            }

            if (!neighbours.ContainsKey(direction))
            {
                switch (direction)
                {
                    case CardinalDirection.Default:
                        throw new System.Exception("no direction");
                    case CardinalDirection.North:
                        neighbours.Add(CardinalDirection.North, newNeighbour);
                        newNeighbour.neighbours.Add(CardinalDirection.South, this);
                        break;
                    case CardinalDirection.South:
                        neighbours.Add(CardinalDirection.South, newNeighbour);
                        newNeighbour.neighbours.Add(CardinalDirection.North, this);
                        break;
                    case CardinalDirection.East:
                        neighbours.Add(CardinalDirection.East, newNeighbour);
                        newNeighbour.neighbours.Add(CardinalDirection.West, this);
                        break;
                    case CardinalDirection.West:
                        neighbours.Add(CardinalDirection.West, newNeighbour);
                        newNeighbour.neighbours.Add(CardinalDirection.East, this);
                        break;
                    default:
                        break;
                }
            }
        }


        /// <summary>
        /// Used to set neighbour cells that aren't on a conventional 2D grid. For example, connecting the east side of a cell to the south 
        /// side of another cell.
        /// </summary>
        /// <param name="newNeigbour"></param>
        /// <param name="direction1">newNeighbour will be the 'direction1 neighbour' (eg; 'east neighbour')</param>
        /// <param name="direction2">this cell will be the 'direction2 neighbour' (eg: 'south neighbour') for the newNeighbour</param>
        public void SetNeighour(Cell newNeigbour, CardinalDirection direction1, CardinalDirection direction2)
        {
            if (newNeigbour == null)
            {
                return;
            }
            else if (newNeigbour.Mask == -1 || Mask == -1)
            {
                return;
            }

            if (!neighbours.ContainsKey(direction1) && !newNeigbour.neighbours.ContainsKey(direction2))
            {
                neighbours.Add(direction1, newNeigbour);
                newNeigbour.neighbours.Add(direction2, this);
            }
            else
            {
                Debug.LogError("Neighbour Exists");
            }
        }


        /// <summary>
        /// Removes the cell in the given direction as a neighbour. This does not unlink them if they were linked.
        /// </summary>
        public void RemoveNeighbour(CardinalDirection direction)
        {
            switch (direction)
            {
                case CardinalDirection.Default:
                    throw new System.Exception("no direction");
                case CardinalDirection.North:
                    neighbours[CardinalDirection.North].neighbours.Remove(CardinalDirection.South);
                    neighbours.Remove(CardinalDirection.North);
                    break;
                case CardinalDirection.South:
                    neighbours[CardinalDirection.South].neighbours.Remove(CardinalDirection.North);
                    neighbours.Remove(CardinalDirection.South);
                    break;
                case CardinalDirection.East:
                    neighbours[CardinalDirection.East].neighbours.Remove(CardinalDirection.West);
                    neighbours.Remove(CardinalDirection.East);
                    break;
                case CardinalDirection.West:
                    neighbours[CardinalDirection.West].neighbours.Remove(CardinalDirection.East);
                    neighbours.Remove(CardinalDirection.West);
                    break;
                default:
                    break;
            }
        }


        /// <summary>
        /// Returns a neighbour in the given mask. Returns a random neighbour from any mask if the given mask is 0.
        /// Returns null if no suitable neighbours.
        /// </summary>
        /// <param name="mask">Works on cells on the given mask. If 0, works with all valid cells</param>
        /// <returns></returns>
        public Cell GetRandomNeighbour(int mask = 0)
        {
            if (mask == 0)
            {
                Cell neighbour = neighbours.ElementAt(Random.Range(0, neighbours.Count)).Value;
                return neighbour;
            }
            else
            {
                List<Cell> validNeighbours = new List<Cell>();

                for (int i = 0; i < neighbours.Count; i++)
                {
                    if (neighbours.ElementAt(i).Value.Mask == mask)
                    {
                        validNeighbours.Add(neighbours.ElementAt(i).Value);
                    }
                }

                if (validNeighbours.Count > 0)
                {
                    return validNeighbours.ElementAt(Random.Range(0, validNeighbours.Count));
                }
                else
                {
                    return null;
                }
            }
        }


        /// <summary>
        /// returns the first dead end neighbour. Returns null if there aren't any.
        /// </summary>
        public Cell GetDeadEndNeighbour()
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours.ElementAt(i).Value.Links.Count == 1)
                {
                    return neighbours.ElementAt(i).Value;
                }
            }

            return null;
        }


        /// <summary>
        /// Returns a random unvisited neighbour on the given mask. Returns null if there are none.
        /// </summary>
        /// <param name="mask">If the mask is 0, it works for all masks</param>
        /// <returns></returns>
        public Cell GetRandomUnvisitedNeighbour(int mask = 0)
        {
            List<Cell> unvisitedNeighbours = new List<Cell>();

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
                {
                    if (!neighbours.ElementAt(i).Value.Visited)
                    {
                        unvisitedNeighbours.Add(neighbours.ElementAt(i).Value);
                    }
                }
            }

            if (unvisitedNeighbours.Count == 0)
            {
                return null;
            }

            return unvisitedNeighbours[Random.Range(0, unvisitedNeighbours.Count)];
        }


        /// <summary>
        /// Returns true if at least one of the neighbours have been visited.
        /// </summary>
        public bool HasVisitedNeighbour()
        {
            for (int i = 0; i < neighbours.Count; i++)
            {
                if (neighbours.ElementAt(i).Value.Visited)
                {
                    return true;
                }
            }

            return false;
        }


        /// <summary>
        /// Returns a random visited neighbour. Returns null if there aren't any.
        /// </summary>
        /// <param name="mask">Works on cells on the given mask. If 0, works with all valid cells</param>
        public Cell GetRandomVisitedNeighbour(int mask = 0)
        {
            List<Cell> visitedNeighbours = new List<Cell>();

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
                {
                    if (neighbours.ElementAt(i).Value.Visited)
                    {
                        visitedNeighbours.Add(neighbours.ElementAt(i).Value);
                    }
                }
            }

            if (visitedNeighbours.Count == 0)
            {
                return null;
            }

            return visitedNeighbours[Random.Range(0, visitedNeighbours.Count)];
        }


        /// <summary>
        /// Returns a list of all the neighbours on the given mask.
        /// </summary>
        /// <param name="mask">If the mask is 0, it works for all masks</param>
        public List<Cell> GetNeighbours(int mask = 0)
        {
            List<Cell> validNeighbours = new List<Cell>();

            for (int i = 0; i < neighbours.Count; i++)
            {
                if (mask == 0 || neighbours.ElementAt(i).Value.Mask == mask)
                {
                    validNeighbours.Add(neighbours.ElementAt(i).Value);
                }
            }

            return validNeighbours;
        }


        /// <summary>
        /// Returns a neighbour that's on another layer, if there is one. Returns null if there isn't
        /// </summary>
        /// <returns></returns>
        public Cell GetBoarderingNeighbour()
        {
            foreach (KeyValuePair<CardinalDirection, Cell> neighbour in neighbours)
            {
                if (neighbour.Value.Mask != -1 && neighbour.Value.Mask != Mask)
                {
                    return neighbour.Value;
                }
            }

            return null;
        }


        /// <summary>
        /// Removes all the neighbours. If they were linked cells, they will still need to be unlinked.
        /// </summary>
        public void RemoveNeighbours()
        {
            Cell neighbour;

            if (neighbours.TryGetValue(CardinalDirection.North, out neighbour))
            {
                neighbour.RemoveNeighbour(CardinalDirection.South);
            }
            else if (neighbours.TryGetValue(CardinalDirection.South, out neighbour))
            {
                neighbour.RemoveNeighbour(CardinalDirection.North);
            }
            else if (neighbours.TryGetValue(CardinalDirection.East, out neighbour))
            {
                neighbour.RemoveNeighbour(CardinalDirection.West);
            }
            else if (neighbours.TryGetValue(CardinalDirection.West, out neighbour))
            {
                neighbour.RemoveNeighbour(CardinalDirection.East);
            }
        }
        #endregion
    }
}