using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Walls))]
[CanEditMultipleObjects]
public class WallsInspector : Editor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Rebuild")) {
            ((Walls)target).Rebuild();
        }
        if (GUILayout.Button("Clear Objects")) {
            ((Walls)target).ClearObjects();
        }
        GUILayout.EndHorizontal();
    }

}
