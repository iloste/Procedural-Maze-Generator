using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] int number;
    [SerializeField] int number12;
    [SerializeField] int number22;
    public string playerName;
    public Vector2 v;
    public void Print()
    {
        Debug.Log(playerName);
    }

}
