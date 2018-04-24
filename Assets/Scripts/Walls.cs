using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walls : MonoBehaviour {

    [Header("Settings")]
    public Material wallMaterial;

    [Header("Marcadores")]
    public List<Vector3> wallMarkers;
    public List<Vector3> doorMarkers;

    [Header("Objetos")]
    public List<GameObject> wallObjects;
    public List<GameObject> doorObjects;

    void Awake() {
        if (wallMarkers == null) wallMarkers = new List<Vector3>();
        if (wallObjects == null) wallObjects = new List<GameObject>();
    }

    public bool AddMarker(Vector3 point) {
        if (wallMarkers.Exists(a => a == point) == false) {
            wallMarkers.Add(point);
            return true;
        }
        return false;
    }

    public void ClearMarkers() {
        ClearObjects();
        wallMarkers.Clear();
        doorMarkers.Clear();
    }

    public void Rebuild() {

        //Borrar todas las paredes
        ClearObjects();

        //Empezar a construir
        foreach (Vector3 point in wallMarkers) {

            //Revisar que no haya una pared ahi
            if (wallObjects.Exists(a => a.transform.position == point) == false) {
                GameObject wall = Instantiate(Structure.GetPrefab("structure_wall"), point, Quaternion.identity);
                if (point.x == 0f) { wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f); }
                wall.transform.parent = transform;

                //Asignar el material del muro
                if (wallMaterial) { wall.GetComponent<MeshRenderer>().material = wallMaterial; } else { wall.GetComponent<MeshRenderer>().material = MaterialBank.GetWallMaterial(0); }

                wallObjects.Add(wall);
            }

        }
    }

    public void ClearObjects() {
        foreach (GameObject go in wallObjects) {
            if (UnityEditor.EditorApplication.isPlaying) {
                Destroy(go);
            } else {
                DestroyImmediate(go);
            }
        }
        wallObjects.Clear();
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
            if (wallMarkers.Exists(a => a == possible)) {
                neighbors.Add(possible);
            }
        }

        return neighbors.ToArray();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach (Vector3 point in wallMarkers) {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
