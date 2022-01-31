using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] GameObject northWall;
    [SerializeField] GameObject southWall;

    [SerializeField] GameObject eastWall;
    [SerializeField] GameObject westWall;

    [SerializeField] public GameObject floor;

    [SerializeField] GameObject northEastPillar;
    [SerializeField] GameObject northWestPillar;
    [SerializeField] GameObject southEastPillar;
    [SerializeField] GameObject southWestPillar;

  

    public void DeactivateWall(Cell.Direction direction)
    {
        switch (direction)
        {
            case Cell.Direction.North:
                northWall.SetActive(false);
                break;
            case Cell.Direction.East:
                eastWall.SetActive(false);
                break;
            case Cell.Direction.South:
                southWall.SetActive(false);
                break;
            case Cell.Direction.West:
                westWall.SetActive(false);
                break;
            default:
                break;
        }

        if (!northWall.activeSelf && !eastWall.activeSelf)
        {
            northEastPillar.SetActive(false);
        }
        if (!northWall.activeSelf && !westWall.activeSelf)
        {
            northWestPillar.SetActive(false);
        }
        if (!southWall.activeSelf && !eastWall.activeSelf)
        {
            southEastPillar.SetActive(false);
        }
        if (!southWall.activeSelf && !westWall.activeSelf)
        {
            southWestPillar.SetActive(false);
        }

    }
}
