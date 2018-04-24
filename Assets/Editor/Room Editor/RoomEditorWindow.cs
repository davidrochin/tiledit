using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoomEditorWindow : EditorWindow {

    public RoomEditMode mode = RoomEditMode.None;

    //General
    public int currentFloor = 0;

    //Furniture
    public Furniture selectedFurniture;

    [MenuItem ("Room/Editor")]
    public static void ShowWindow() {
        EditorWindow.GetWindow<RoomEditorWindow>("Room Editor");
    }

    private void OnGUI() {

        //Edit mode selector
        GUILayout.Label("Edit mode", EditorStyles.boldLabel);
        mode = (RoomEditMode)EditorGUILayout.EnumPopup(mode);

        //General settings
        GUILayout.Label("General settings", EditorStyles.boldLabel);
        currentFloor = EditorGUILayout.IntSlider(currentFloor, 1, 4);

        if (mode == RoomEditMode.FurniturePlacement) {
            GUILayout.Label("Furniture placement", EditorStyles.boldLabel);

            //Furniture picker
            if (GUILayout.Button("Select furniture")) { EditorGUIUtility.ShowObjectPicker<GameObject>(selectedFurniture, false, "furniture_", 0); }
            if (Event.current.commandName == "ObjectSelectorClosed" && EditorGUIUtility.GetObjectPickerControlID() == 0) {
                if (EditorGUIUtility.GetObjectPickerObject() != null) {
                    selectedFurniture = ((GameObject)EditorGUIUtility.GetObjectPickerObject()).GetComponent<Furniture>();
                } else {
                    selectedFurniture = null;
                }
                Focus();
            }

            //Show furniture name
            GUILayout.Label(selectedFurniture == null ? "" : selectedFurniture.name);

            //Show furniture image
            if (selectedFurniture != null) { GUILayout.Label(AssetPreview.GetAssetPreview(selectedFurniture.gameObject)); }

            //selectedFurniture = (GameObject)EditorGUILayout.ObjectField(selectedFurniture, typeof(GameObject), false);
            //GUILayout.SelectionGrid(0, new string[] { "Hola", "Adops"}, 1);

            //Debug stuff
            GUILayout.Label("Debug", EditorStyles.boldLabel);
            GUILayout.Label("" + GUIUtility.GUIToScreenPoint(Event.current.mousePosition));
            //if(Camera.current != null) { GUILayout.Label(Camera.current.pixelRect + ""); }
            //if (SceneView.currentDrawingSceneView != null) { GUILayout.Label(SceneView.currentDrawingSceneView.pivot + ", pivot"); }

            //Debug.Log(SceneView.lastActiveSceneView.camera.ScreenToViewportPoint(GUIUtility.GUIToScreenPoint(Event.current.mousePosition)));
            //Vector2 remain = Screen
            //Debug.Log(HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).origin);
        }

    }

    private void OnInspectorUpdate() {
        Repaint();
    }

    public enum RoomEditMode { None, WallPlacement, FloorPlacement, FurniturePlacement }

}
