using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TilemapSaveHandler))]
public class TilemapManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var script = (TilemapSaveHandler)target;

        if (GUILayout.Button("Save Map"))
        {
            script.SaveMapFromEditor();
        }

        if (GUILayout.Button("Load Map"))
        {
            script.LoadMapFromEditor();
        }
    }
}
