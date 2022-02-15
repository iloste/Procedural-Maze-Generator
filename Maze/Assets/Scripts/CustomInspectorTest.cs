using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
[CustomEditor(typeof(MazeGenerator))]
public class CustomInspectorTest : Editor
{
    const string resourceFilename = "custom-editor-uie";
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement customInspector = new VisualElement();
        //var visualTree = Resources.Load(resourceFilename) as VisualTreeAsset;
        //visualTree.CloneTree(customInspector);
       // customInspector.styleSheets.Add(Resources.Load($"{resourceFilename}-style") as StyleSheet);
        return customInspector;
    }
}