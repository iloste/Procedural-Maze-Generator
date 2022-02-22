using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MGT
{
    public class Tile : MonoBehaviour
    {
        [SerializeField] public GameObject floor;
        [SerializeField] Transform tilePiece;


        /// <summary>
        /// Sets the scale of the floor and walls
        /// </summary>
        /// <param name="xzScale"></param>
        /// <param name="yScale"></param>
        public void SetScale(float xzScale, float yScale)
        {
            if (tilePiece != null)
            {
                tilePiece.localScale = new Vector3(xzScale, yScale, xzScale); 
            }

            if (floor != null)
            {
                floor.transform.localScale = new Vector3(xzScale, yScale, xzScale);
            }
        }
    }
}