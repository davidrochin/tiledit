using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Floor {
    public Material material;
    public List<Vector3> markers;
    public List<GameObject> objects;

    [HideInInspector]
    public GameObject gameObject;

    public Floor(GameObject gameObject) {
        if (markers == null) markers = new List<Vector3>();
        if (objects == null) objects = new List<GameObject>();
    }

    public bool AddMarker(Vector3 point) {
        if (markers.Exists(a => a == point) == false) {
            markers.Add(point);
            return true;
        }
        return false;
    }

    public bool RemoveMarker(Vector3 point) {
        if (markers.Exists(a => a == point) == true) {
            markers.Remove(point);
            return true;
        }
        return false;
    }

    public void ClearMarkers() {
        ClearObjects();
        markers.Clear();
    }

    public void Rebuild() {

        //Clear all objects
        ClearObjects();

        //Build
        foreach (Vector3 point in markers) {

            //Check that is not going to overlap another floor
            if (objects.Exists(a => a.transform.position == point) == false) {
                GameObject floor = GameObject.Instantiate(Structure.GetPrefab("structure_floor"), point, Quaternion.identity);
                floor.transform.parent = gameObject.transform;

                //Asign material
                if (material) { floor.GetComponent<MeshRenderer>().material = material; } else { floor.GetComponent<MeshRenderer>().material = MaterialBank.GetFloorMaterial(0); }

                objects.Add(floor);
            }

        }
    }

    public void ClearObjects() {
        foreach (GameObject go in objects) {
            if (UnityEditor.EditorApplication.isPlaying) {
                GameObject.Destroy(go);
            } else {
                GameObject.DestroyImmediate(go);
            }
        }
        objects.Clear();
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
            if (markers.Exists(a => a == possible)) {
                neighbors.Add(possible);
            }
        }

        return neighbors.ToArray();
    }

}
