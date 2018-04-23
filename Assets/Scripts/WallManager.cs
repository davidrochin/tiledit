using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallManager : MonoBehaviour {

    public List<Vector3> points;
    public List<GameObject> walls;

	void Awake () {
        points = new List<Vector3>();
        walls = new List<GameObject>();
    }

    public bool AddPoint(Vector3 point) {
        if (points.Exists(a => a == point) == false) {
            points.Add(point);
            return true;
        }
        return false;
    }

    public void Rebuild() {

        //Borrar todas las paredes
        foreach(GameObject go in walls) {
            Destroy(go);
        }
        walls.Clear();

        //Empezar a construir
        foreach (Vector3 point in points) {

            //Obtener los vecinos (los puntos con los cuales se deberia conectar este punto)
            Vector3[] neighbors = GetNeighbors(point);
            //Debug.Log("El punto " + point + " tiene " + neighbors.Length + " vecinos.");

            foreach (Vector3 neighbor in neighbors) {
                Vector3 wallPos = point;

                //Revisar que no haya una pared ahi
                if (walls.Exists(a => a.transform.position == wallPos) == false) {
                    GameObject wall = Instantiate(Structure.GetPrefab("structure_wall"), wallPos, Quaternion.identity);
                    if(point.x == neighbor.x) { wall.transform.rotation = Quaternion.Euler(0f, 90f, 0f); }
                    wall.transform.parent = transform;
                    walls.Add(wall);
                }
            }
        }
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
            if(points.Exists(a => a == possible)) {
                neighbors.Add(possible);
            }
        }

        return neighbors.ToArray();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        foreach (Vector3 point in points) {
            Gizmos.DrawSphere(point, 0.1f);
        }
    }
}
