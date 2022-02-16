using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CustomEditorTest : EditorWindow
{


    GameObject tilePrefab;
    MazeGenerator.Algorithm algorithm;
    MazeGenerator.Colouring colouring;
    Vector2Int gridSize;
    bool braidMaze;
    int braidPercentage;
    bool displayDeadEnds;
    int currentMask;
    MazeDisplay mazeDisplay;
    bool useCuboidMaze;

    bool toggleButtons = false;


    [MenuItem("Window/CustomEditorTest")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CustomEditorTest));

    }

    private void OnGUI()
    {
        



        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        gridSize = EditorGUILayout.Vector2IntField("Grid Size", gridSize);

        GUILayout.Label("Prefabs", EditorStyles.boldLabel);
        tilePrefab = (GameObject)EditorGUILayout.ObjectField("Prefab", tilePrefab, typeof(GameObject), true);

        algorithm = (MazeGenerator.Algorithm)EditorGUILayout.EnumPopup("Algorithm", algorithm);
        colouring = (MazeGenerator.Colouring)EditorGUILayout.EnumPopup("Colouring", colouring);

        braidMaze = EditorGUILayout.Toggle("Braid Maze", braidMaze);
        displayDeadEnds = EditorGUILayout.Toggle("Display Deadends", displayDeadEnds);

        braidPercentage = EditorGUILayout.IntSlider("Braid Maze", braidPercentage, 0, 100);



        toggleButtons = EditorGUILayout.Toggle("Toggle Buttons", toggleButtons);

        if (toggleButtons)
        {
            if (GUILayout.Button("Print", GUILayout.MaxWidth(300)))
            {
                Print();
                GenerateMaze();
            }
            if (GUILayout.Button("MazeTest", GUILayout.MaxWidth(300)))
            {
               // Debug.Log(MazeGenerator.instance.GetGrid().GetCellCount(0));
            }
            if (GUILayout.Button("NewGridTest", GUILayout.MaxWidth(300)))
            {
               // MazeGenerator.instance.NewGrid();

            }
            if (GUILayout.Button("NewGridTest2", GUILayout.MaxWidth(300)))
            {
               // MazeGenerator.instance.NewGrid2();


            }
        }

    }


    public void GenerateMaze()
    {
       // MazeGenerator.instance.GenerateMaze();
       // Debug.Log(MazeGenerator.instance.GetGrid().GetCellCount(0));
    }


    public void Print()
    {
        Debug.Log("Success");
    }

    private void ToggleButtons()
    {
        toggleButtons = !toggleButtons;
    }
}
