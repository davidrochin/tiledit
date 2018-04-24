using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour {
    [Header("Settings")]
    public Material floorMaterial;

    [Header("Markers")]
    public List<Vector3> floorMarkers;

    [Header("Objects")]
    public List<GameObject> floorObjects;

    void Awake() {
        if (floorMarkers == null) floorMarkers = new List<Vector3>();
        if (floorObjects == null) floorObjects = new List<GameObject>();
    }

    public bool AddMarker(Vector3 point) {
        if (floorMarkers.Exists(a => a == point) == false) {
            floorMarkers.Add(point);
            return true;
        }
        return false;
    }

    public bool RemoveMarker(Vector3 point) {
        if (floorMarkers.Exists(a => a == point) == true) {
            floorMarkers.Remove(point);
            return true;
        }
        return false;
    }

    public void ClearMarkers() {
        ClearObjects();
        floorMarkers.Clear();
    }

    public void Rebuild() {

        //Clear all objects
        ClearObjects();

        //Build
        foreach (Vector3 point in floorMarkers) {

            //Check that is not going to overlap another floor
            if (floorObjects.Exists(a => a.transform.position == point) == false) {
                GameObject floor = Instantiate(Structure.GetPrefab("structure_floor"), point, Quaternion.identity);
                floor.transform.parent = transform;

                //Asign material
                if (floorMaterial) { floor.GetComponent<MeshRenderer>().material = floorMaterial; } else { floor.GetComponent<MeshRenderer>().material = MaterialBank.GetFloorMaterial(0); }

                floorObjects.Add(floor);
            }

        }
    }

    public void ClearObjects() {
        foreach (GameObject go in floorObjects) {
            if (UnityEditor.EditorApplication.isPlaying) {
                Destroy(go);
            } else {
                DestroyImmediate(go);
            }
        }
        floorObjects.Clear();
    }

    Vector3[] GetNeighbors(Vector3 point) {
        List<Vector3> neighbors = new List<Vector3>();

        Vector3[] possibleNeighbors = new Vector3[] {
            new Vector3(point.x + 1f, point.y, point.z),
            new Vector3(point.x - 1f, point.y, point.z),
            new Vector3(point.x, point.y, point.z + 1f),
            new Vector3(point.x, point.y, point.z -1f)
        };

        foreach (Vector3 possible in possibleNeighbors) {
            if (floorMarkers.Exists(a => a == possible)) {
                neighbors.Add(possible);
            }
        }

        return neighbors.ToArray();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        foreach (Vector3 point in floorMarkers) {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
