using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor {

    public static RoomEditMode editMode;
    public static bool editModeEnabled;
    public static int editFloor;

    public static Plane editPlane;

    Vector3 mouseEditPos;
    Vector3 mouseFloorGridPos;
    Vector3 mouseWallGridPos;

    Walls walls;
    Floor floor;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        GUILayout.Label("Editor", EditorStyles.boldLabel);

        //Floor selector
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Floor");
        editFloor = EditorGUILayout.IntSlider(editFloor, 1, 3);
        editPlane = new Plane(Vector3.up, Vector3.zero + Vector3.up * (editFloor - 1));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(editMode == RoomEditMode.None, "None", EditorStyles.miniButtonLeft)) editMode = RoomEditMode.None;
        if (GUILayout.Toggle(editMode == RoomEditMode.Floor, "Floor", EditorStyles.miniButtonMid)) editMode = RoomEditMode.Floor;
        if (GUILayout.Toggle(editMode == RoomEditMode.Walls, "Walls", EditorStyles.miniButtonMid)) editMode = RoomEditMode.Walls;
        if (GUILayout.Toggle(editMode == RoomEditMode.Furniture, "Furniture", EditorStyles.miniButtonRight)) editMode = RoomEditMode.Furniture;
        EditorGUILayout.EndHorizontal();
        
    }

    private void OnSceneGUI() {

        //Calculate mouse position on imaginary plane
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        float rayHitDistance = 0f; bool onPlane = editPlane.Raycast(ray, out rayHitDistance);
        mouseEditPos = ray.origin + ray.direction * rayHitDistance;

        //Calculate mouse position locked to mouse and wall grids
        mouseFloorGridPos = new Vector3(RoundDown(mouseEditPos.x) + 0.5f, mouseEditPos.y, RoundDown(mouseEditPos.z) + 0.5f);
        mouseWallGridPos = new Vector3(RoundNearest(mouseEditPos.x), mouseEditPos.y, RoundNearest(mouseEditPos.z));

        //Dibujar el punto del mouse
        //Handles.BeginGUI();
        //Handles.color = Color.red; Handles.DrawSolidDisc(mouseEditPos, Vector3.up, 0.1f);
        //Handles.color = Color.cyan; Handles.DrawSolidDisc(mouseFloorGridPos, Vector3.up, 0.1f);
        //Handles.color = Color.green; Handles.DrawSolidDisc(mouseWallGridPos, Vector3.up, 0.1f);
        //Handles.EndGUI();

        //Prevent lose of focus on GameObject if we are in any edit mode
        if(editMode != RoomEditMode.None) {
            Selection.activeObject = target;
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }

        //If in Floor edit mode
        if (editMode == RoomEditMode.Floor) {
            Handles.DrawWireCube(mouseFloorGridPos, new Vector3(1f, 0f, 1f));

            //If mouse clicked
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag) {

                //Left click to draw
                if (currentEvent.button == 0) {
                    if (GetFloor().AddMarker(mouseFloorGridPos)) { GetFloor().Rebuild(); }
                } 
                
                //Right click to erase
                /*else if(currentEvent.button == 0) {
                    if (GetFloor().RemoveMarker(mouseFloorGridPos)) { GetFloor().Rebuild(); }
                }*/
                
            }
        }

        //If in Walls edit mode
        if (editMode == RoomEditMode.Walls) {
            Handles.DrawLine(mouseWallGridPos, mouseWallGridPos + Vector3.up * 3f);
        }

    }

    float DistanceToWholeX(Vector3 point) {
        Vector3 goal = new Vector3(RoundNearest(point.x), point.y, point.z);
        return Vector3.Distance(point, goal);
    }

    float DistanceToWholeZ(Vector3 point) {
        Vector3 goal = new Vector3(point.x, point.y, RoundNearest(point.z));
        return Vector3.Distance(point, goal);
    }

    int RoundDown(float f) {
        if(f > 0f) {
            return (int)f;
        } else {
            return (int)(f - 1f);
        }
    }

    int RoundNearest(float f) {
        float decimals = f % 1f;
        if(decimals <= 0.5f) {
            return (int)f;
        } else {
            return (int)(f + 1);
        }
    }

    float GetDecimal(float f) {
        return f % 1f;
    }

    Floor GetFloor() {
        if(floor != null) {
            return floor;
        } else {
            return ((Room)target).GetComponent<Floor>();
        }
    }

    public enum RoomEditMode { None, Floor, Walls, Furniture }
}
