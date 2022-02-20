using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{


    [SerializeField] public GameObject floor;

    [SerializeField] Transform tilePiece;



    public void SetScale(float xzScale, float yScale)
    {
        tilePiece.localScale = new Vector3(xzScale, yScale, xzScale);
    }
}
