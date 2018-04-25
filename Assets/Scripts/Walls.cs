using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Walls {

    public Material material;

    public List<Vector3> wallMarkers;
    public List<Vector3> doorMarkers;

    public List<GameObject> wallObjects;
    public List<GameObject> doorObjects;

    [HideInInspector]
    public GameObject gameObject;

    public Walls(GameObject gameObject) {
        this.gameObject = gameObject;
    }

    public bool AddMarker(Vector3 point) {

        //Check that it is on a valid grid point
        if(((point.x % 1f == 0 && point.z % 1f == 0.5f) || (point.x % 1f == 0.5 && point.z % 1f == 0)) == false) {
            return false;
        }

        //Add to marker list only if it doesn't exist already
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
                GameObject wall = GameObject.Instantiate(Structure.GetPrefab("structure_wall"), point, Quaternion.identity);
                if (point.x % 1f == 0f) { wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f); }
                wall.transform.parent = gameObject.transform;

                //Asignar el material del muro
                if (material) { wall.GetComponent<MeshRenderer>().material = material; } else { wall.GetComponent<MeshRenderer>().material = MaterialBank.GetWallMaterial(0); }

                wallObjects.Add(wall);
            }

        }
    }

    public void ClearObjects() {
        foreach (GameObject go in wallObjects) {
            if (UnityEditor.EditorApplication.isPlaying) {
                GameObject.Destroy(go);
            } else {
                GameObject.DestroyImmediate(go);
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

}
