using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Room))]
public class RoomEditor : Editor {

    public static RoomEditMode editMode;
    public static bool editModeEnabled;
    public static int floorLevel;

    //Edit Plane
    public static Plane editPlane;

    //Mouse position in plane and grid-locked mouse positions
    Vector3 mouseEditPos;
    Vector3 mouseFloorGridPos;
    Vector3 mouseWallGridPos;

    //Walls and Floor managers (do not use directly)
    Walls walls;
    Floor floor;

    //Wall points for drawing walls
    List<Vector3> wallEditPoints = new List<Vector3>();

    public override void OnInspectorGUI() {

        //Draw default inspector
        base.OnInspectorGUI();

        GUILayout.Label("Editor", EditorStyles.boldLabel);

        //Edit mode selector
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Toggle(editMode == RoomEditMode.None, "None", EditorStyles.miniButtonLeft)) editMode = RoomEditMode.None;
        if (GUILayout.Toggle(editMode == RoomEditMode.Floor, "Floor", EditorStyles.miniButtonMid)) editMode = RoomEditMode.Floor;
        if (GUILayout.Toggle(editMode == RoomEditMode.Walls, "Walls", EditorStyles.miniButtonMid)) editMode = RoomEditMode.Walls;
        if (GUILayout.Toggle(editMode == RoomEditMode.Furniture, "Furniture", EditorStyles.miniButtonRight)) editMode = RoomEditMode.Furniture;
        EditorGUILayout.EndHorizontal();

        //Floor selector
        if(editMode != RoomEditMode.None) {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Floor Level");
            floorLevel = EditorGUILayout.IntSlider(floorLevel, 1, 3);
            editPlane = new Plane(Vector3.up, Vector3.zero + Vector3.up * (floorLevel - 1));
            EditorGUILayout.EndHorizontal();
        }

        //Debug buttons
        GUILayout.Label("Debug", EditorStyles.boldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear Objects")) {
            ((Room)target).walls.ClearObjects();
            ((Room)target).floor.ClearObjects();
            SetSceneDirty();
        }
        if (GUILayout.Button("Rebuild all")) {
            ((Room)target).walls.Rebuild();
            ((Room)target).floor.Rebuild();
            SetSceneDirty();
        }
        if (GUILayout.Button("Load Test Room")) {
            ((Room)target).LoadTestRoom();
            SetSceneDirty();
        }
        GUILayout.EndHorizontal();
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
                    if (GetFloor().AddMarker(mouseFloorGridPos)) { GetFloor().Rebuild(); SetSceneDirty(); }
                } 
                
                //Right click to erase
                /*else if(currentEvent.button == 0) {
                    if (GetFloor().RemoveMarker(mouseFloorGridPos)) { GetFloor().Rebuild(); }
                }*/
                
            }
        }

        //If in Walls edit mode
        if (editMode == RoomEditMode.Walls) {

            //Draw the marker
            Handles.DrawLine(mouseWallGridPos, mouseWallGridPos + Vector3.up * 3f);

            //If mouse clicked or dragged
            Event currentEvent = Event.current;
            if (currentEvent.type == EventType.MouseDown || currentEvent.type == EventType.MouseDrag) {
                if (currentEvent.button == 0 && wallEditPoints.Exists(a => a == mouseWallGridPos) == false) {
                    wallEditPoints.Add(mouseWallGridPos);
                }
            }

            //If mouse released
            if(currentEvent.button == 0 && currentEvent.type == EventType.MouseUp) {

                //Check that there is at least two wallEditPoints
                if(wallEditPoints.Count >= 2) {

                    //Add walls from wallEditPoints
                    for (int i = 0; i < wallEditPoints.Count - 1; i++) {

                        //Add a wall between this point and the next
                        GetWalls().AddMarker(MiddleBetween(wallEditPoints[i], wallEditPoints[i + 1]));
                        SetSceneDirty();

                    }
                }

                //Clear point list and rebuild
                wallEditPoints.Clear();
                GetWalls().Rebuild();
            }

            //Draw wallEditPoints if any
            foreach (Vector3 editPoint in wallEditPoints) {
                Handles.DrawSolidDisc(editPoint, Vector3.up, 0.05f);
            }
        }

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

    Vector3 MiddleBetween(Vector3 a, Vector3 b) {
        Vector3 dir = b - a;
        return a + dir * (Vector3.Distance(a, b) * 0.5f);
    }

    Floor GetFloor() {
        if(floor != null) {
            return floor;
        } else {
            return ((Room)target).floor;
        }
    }

    Walls GetWalls() {
        if (walls != null) {
            return walls;
        } else {
            return ((Room)target).walls;
        }
    }

    bool SetSceneDirty() {
        if (!EditorApplication.isPlaying) {
            return UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        } else {
            return false;
        }  
    }

    public enum RoomEditMode { None, Floor, Walls, Furniture }
}
