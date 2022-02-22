
namespace MGT
{
    public class Connector
    {
        public Cell currentCell;
        public Cell connectedCell;


        /// <summary>
        /// Returns true if the currentCell's and connectedCell's regions match either: 
        /// regionA and regionB or regionB and regionA, respectively
        /// </summary>
        public bool SameConnection(int regionA, int regionB)
        {
            if (currentCell.Region == regionA && connectedCell.Region == regionB ||
                currentCell.Region == regionB && connectedCell.Region == regionA)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// Returns true if either cell is part of the given region
        /// </summary>
        public bool ConnectsToRegion(int region)
        {
            if (currentCell.Region == region || connectedCell.Region == region)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Changes currentCell's and/or connectedCell's regions to the newRegion if they were in the oldRegion
        /// </summary>
        /// <param name="oldRegion"></param>
        /// <param name="newRegion"></param>
        public void MergeRegions(int oldRegion, int newRegion)
        {
            if (currentCell.Region == oldRegion)
            {
                currentCell.Region = newRegion;
            }
            else if (connectedCell.Region == oldRegion)
            {
                connectedCell.Region = newRegion;
            }
        }


        /// <summary>
        /// Returns true if currentCell and connectedCell are in the same region
        /// </summary>
        /// <returns></returns>
        public bool IsOneRegion()
        {
            if (currentCell.Region == connectedCell.Region)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}