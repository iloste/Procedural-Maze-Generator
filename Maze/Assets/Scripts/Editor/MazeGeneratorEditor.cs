using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// Declare type of Custom Editor
[CustomEditor(typeof(MazeGenerator))] //1

public class MazeGeneratorEditor : Editor
{


    // To Do:
    // Link room should use prefabs without pillars. Maybe use a bool to check if the cell is in a room or not.
    // Auto detect all the colours from the image. Mayeb have a button that will run the code. 




    int layerColourCountPrevious = 0;
    float labelWidth = 150f;
    MazeGenerator mazeGenerator;
    SerializedObject maze;
    private void OnEnable()
    {
        mazeGenerator = (MazeGenerator)target;
        maze = new SerializedObject(mazeGenerator);
    }

    public override void OnInspectorGUI() //2
    {
        maze.Update();
        // Call base class method
        // base.DrawDefaultInspector();

        // Custom form for Player Preferences


        GUILayout.Space(20f);
        GUILayout.Label("Custom Editor Elements", EditorStyles.boldLabel);

        GUILayout.Space(10f);
        GUILayout.Label("Test");


        #region Grid Data
        GUILayout.BeginHorizontal();
        maze.FindProperty("gridSize").vector2IntValue = EditorGUILayout.Vector2IntField("Grid Size", maze.FindProperty("gridSize").vector2IntValue);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        maze.FindProperty("layerImage").objectReferenceValue = EditorGUILayout.ObjectField("Layer Image", maze.FindProperty("layerImage").objectReferenceValue, typeof(Texture2D), true);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        maze.FindProperty("layerColourCount").intValue = EditorGUILayout.DelayedIntField("Layer Colour Count", maze.FindProperty("layerColourCount").intValue);
        GUILayout.EndHorizontal();

        if (mazeGenerator.layerColourCount != layerColourCountPrevious)
        {
            layerColourCountPrevious = mazeGenerator.layerColourCount;
            UpdateList(mazeGenerator.layerColours, mazeGenerator.layerColourCount);
            UpdateList(mazeGenerator.algorithms, mazeGenerator.layerColourCount);
            maze.ApplyModifiedProperties();
            maze.UpdateIfRequiredOrScript();
        }

        for (int i = 0; i < mazeGenerator.layerColours.Count; i++)
        {
            GUILayout.Space(10f);
            GUILayout.Label("Layer " + i, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            maze.FindProperty("layerColours").GetArrayElementAtIndex(i).colorValue = EditorGUILayout.ColorField("Layer Colour", maze.FindProperty("layerColours").GetArrayElementAtIndex(i).colorValue);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            maze.FindProperty("algorithms").GetArrayElementAtIndex(i).enumValueIndex = (int)(MazeGenerator.Algorithm)EditorGUILayout.EnumPopup("Layer Algorithm", (MazeGenerator.Algorithm)System.Enum.GetValues(typeof(MazeGenerator.Algorithm)).GetValue(maze.FindProperty("algorithms").GetArrayElementAtIndex(i).enumValueIndex));
            GUILayout.EndHorizontal();
        }
        GUILayout.Space(10f);

        #endregion

        #region Maze Gen Data
        GUILayout.BeginHorizontal();
        maze.FindProperty("algorithm").enumValueIndex = (int)(MazeGenerator.Algorithm)EditorGUILayout.EnumPopup("Algorithm", (MazeGenerator.Algorithm)System.Enum.GetValues(typeof(MazeGenerator.Algorithm)).GetValue(maze.FindProperty("algorithm").enumValueIndex));
        GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //mazeGenerator.colouring = (MazeGenerator.Colouring)EditorGUILayout.EnumPopup("Colouring", mazeGenerator.colouring);
        //GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        maze.FindProperty("braidMaze").boolValue = EditorGUILayout.Toggle("Braid Maze", maze.FindProperty("braidMaze").boolValue);
        GUILayout.EndHorizontal();

        if (maze.FindProperty("braidMaze").boolValue)
        {
            GUILayout.BeginHorizontal();
            maze.FindProperty("braidPercentage").intValue = EditorGUILayout.IntSlider("Braid Maze", maze.FindProperty("braidPercentage").intValue, 0, 100);
            GUILayout.EndHorizontal();
        }
        #endregion

        #region Display Data
        GUILayout.BeginHorizontal();
        maze.FindProperty("displayDeadEnds").boolValue = EditorGUILayout.Toggle("Display Deadends", maze.FindProperty("displayDeadEnds").boolValue);
        GUILayout.EndHorizontal();

        #endregion



        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Maze"))
        {
            mazeGenerator.DeleteMaze();
            mazeGenerator.GenerateMaze();
        }
        if (GUILayout.Button("Delete Maze")) //8
        {
            mazeGenerator.DeleteMaze();
        }
        GUILayout.EndHorizontal();
        // Custom Button with Image as Thumbnail
        maze.ApplyModifiedProperties();
    }

    private void UpdateList<T>(List<T> list, int newCount) where T : new()
    {
        if (list.Count < newCount)
        {
            int toAdd = newCount - list.Count;

            for (int i = 0; i < toAdd; i++)
            {
                list.Add(new T());
            }
        }
        else
        {
            int toRemove = list.Count - newCount;

            for (int i = 0; i < toRemove; i++)
            {
                list.RemoveAt(list.Count - 1);
            }
        }
    }
}
