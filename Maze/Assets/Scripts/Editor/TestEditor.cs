using UnityEngine;
using UnityEditor;

// Declare type of Custom Editor
[CustomEditor(typeof(Test))] //1
public class TestEditor : Editor
{

    // float thumbnailWidth = 70;
    //float thumbnailHeight = 70;
   // float labelWidth = 150f;

    //  string playerName = "Player 1";
    //string playerLevel = "1";
    //string playerElo = "5";
    //string playerScore = "100";


    // OnInspector GUI
    public override void OnInspectorGUI() //2
    {

        // Call base class method
        // base.DrawDefaultInspector();

        // Custom form for Player Preferences
        Test test = (Test)target;

        GUILayout.Space(20f); //2
        GUILayout.Label("Custom Editor Elements", EditorStyles.boldLabel); //3

        GUILayout.Space(10f);
        GUILayout.Label("Test");

        GUILayout.BeginHorizontal(); //4
        //GUILayout.Label("Player Name", GUILayout.Width(labelWidth)); //5
        test.playerName = GUILayout.TextField(test.playerName); //6
        GUILayout.EndHorizontal(); //7

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Maze")) //8
        {
            test.Print();
        }
        if (GUILayout.Button("Delete Maze")) //8
        {

        }
        GUILayout.EndHorizontal();
        // Custom Button with Image as Thumbnail
    }
}
