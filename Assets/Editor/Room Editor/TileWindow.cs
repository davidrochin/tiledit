using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TileWindow : EditorWindow {

    [MenuItem ("Room Editor/Tiles")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(TileWindow));
    }

    private void OnGUI() {

        GUILayout.Label("Base settings");
        GUILayout.SelectionGrid(0, new string[] { "Hola", "Adops"}, 1);

    }

}
