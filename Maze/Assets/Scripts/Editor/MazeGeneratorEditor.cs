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
    MazeGenerator mazeGenerator;
    SerializedObject maze;
    bool showLayers = true;
    GUIStyle myFoldoutStyle;

    private void OnEnable()
    {
        mazeGenerator = (MazeGenerator)target;
        maze = new SerializedObject(mazeGenerator);
    }

    public override void OnInspectorGUI()
    {
        maze.Update();
        myFoldoutStyle = new GUIStyle(EditorStyles.foldout);
        myFoldoutStyle.fontStyle = FontStyle.Bold;
        myFoldoutStyle.margin = new RectOffset(8, 4, 0, 0);


        #region Grid Data

        EditorGUILayout.BeginVertical("GroupBox");
        maze.FindProperty("tab").intValue = GUILayout.Toolbar(maze.FindProperty("tab").intValue, new string[] { "Auto Grid", "Image Controlled Grid" });
        GUILayout.Space(10);

        switch (maze.FindProperty("tab").intValue)
        {
            case 0:
                // using manual grid size
                GUILayout.BeginHorizontal();
                maze.FindProperty("gridSize").vector2IntValue = EditorGUILayout.Vector2IntField("Grid Size", maze.FindProperty("gridSize").vector2IntValue);
                GUILayout.EndHorizontal();

                GUILayout.Space(4);

                GUILayout.BeginHorizontal();
                maze.FindProperty("algorithm").enumValueIndex = (int)(MazeGenerator.Algorithm)EditorGUILayout.EnumPopup("Maze Algorithm", (MazeGenerator.Algorithm)System.Enum.GetValues(typeof(MazeGenerator.Algorithm)).GetValue(maze.FindProperty("algorithm").enumValueIndex));
                GUILayout.EndHorizontal();
                break;
            case 1:
                // using bitmap for grid
                EditorGUILayout.HelpBox("If using an image for the grid, the maze will take it's size from the width and height of the image.", MessageType.Info);
                GUILayout.Space(5);

                GUILayout.BeginHorizontal();
                maze.FindProperty("layerImage").objectReferenceValue = EditorGUILayout.ObjectField(new GUIContent("Layer Image", "Use a texture2D as the base of the maze. Below, you can selet each colour used in the maze and tell it which algorithm to use on that layer."), maze.FindProperty("layerImage").objectReferenceValue, typeof(Texture2D), true);
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                showLayers = EditorGUILayout.Foldout(showLayers, "Layers", myFoldoutStyle);
                GUILayout.EndHorizontal();

                if (showLayers)
                {
                    GUILayout.Space(5);

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
                }
                break;
            default:
                break;
        }
        GUILayout.Space(5);
        EditorGUILayout.EndVertical();

        #endregion

        #region Maze Gen Data
        //GUILayout.BeginHorizontal();
        //maze.FindProperty("algorithm").enumValueIndex = (int)(MazeGenerator.Algorithm)EditorGUILayout.EnumPopup("Algorithm", (MazeGenerator.Algorithm)System.Enum.GetValues(typeof(MazeGenerator.Algorithm)).GetValue(maze.FindProperty("algorithm").enumValueIndex));
        //GUILayout.EndHorizontal();

        //GUILayout.BeginHorizontal();
        //mazeGenerator.colouring = (MazeGenerator.Colouring)EditorGUILayout.EnumPopup("Colouring", mazeGenerator.colouring);
        //GUILayout.EndHorizontal();

        EditorGUILayout.BeginVertical("GroupBox");

        GUILayout.BeginHorizontal();
        maze.FindProperty("braidMaze").boolValue = EditorGUILayout.Toggle(new GUIContent("Braid Maze", "Remove a certain amount of the dead ends in the maze."), maze.FindProperty("braidMaze").boolValue);
        GUILayout.EndHorizontal();

        if (maze.FindProperty("braidMaze").boolValue)
        {
            GUILayout.BeginHorizontal();
            maze.FindProperty("braidPercentage").intValue = EditorGUILayout.IntSlider(new GUIContent("Braid Percentage", "Percent of dead ends that will be removed."), maze.FindProperty("braidPercentage").intValue, 0, 100);
            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        maze.FindProperty("useRandomSeed").boolValue = EditorGUILayout.Toggle(new GUIContent("Use Random Seed", "Using the same seed will generate the same maze."), maze.FindProperty("useRandomSeed").boolValue);
        GUILayout.EndHorizontal();

        EditorGUI.BeginDisabledGroup(maze.FindProperty("useRandomSeed").boolValue);
        GUILayout.BeginHorizontal();
        maze.FindProperty("seed").intValue = EditorGUILayout.IntField("Seed", maze.FindProperty("seed").intValue);
        GUILayout.EndHorizontal();
        EditorGUI.EndDisabledGroup();

        GUILayout.EndVertical();
        GUILayout.Space(5);
        #endregion

        #region Display Data
        //GUILayout.BeginHorizontal();
        //maze.FindProperty("displayDeadEnds").boolValue = EditorGUILayout.Toggle("Display Deadends", maze.FindProperty("displayDeadEnds").boolValue);
        //GUILayout.EndHorizontal();

        #endregion

        #region Generate Buttons

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Maze"))
        {
            mazeGenerator.DeleteMaze();
            mazeGenerator.GenerateMaze();
        }

        if (GUILayout.Button("Delete Maze"))
        {
            mazeGenerator.DeleteMaze();
        }

        GUILayout.EndHorizontal();

        #endregion

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
