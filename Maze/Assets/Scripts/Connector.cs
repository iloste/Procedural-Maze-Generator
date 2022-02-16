using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connector
{
    public Cell currentCell;
    public Cell connectedCell;


    public bool SameConnection(int a, int b)
    {
        if (currentCell.Region == a && connectedCell.Region == b ||
            currentCell.Region == b && connectedCell.Region == a)
        {
            return true;
        }

        return false;
    }

    public bool ConnectsToRegion(int a)
    {
        if (currentCell.Region == a || connectedCell.Region == a)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


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
