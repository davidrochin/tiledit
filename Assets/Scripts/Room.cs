using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [Header("Debug")]
    public Walls walls;
    public Floor floor;

    private void Awake() {
        floor.gameObject = gameObject;
        walls.gameObject = gameObject;

    }

    void Start () {
        LoadTestRoom();
	}

    public void ClearAll() {
        walls.ClearMarkers();
        walls.ClearObjects();
        floor.ClearMarkers();
        floor.ClearObjects();
    }

    public void LoadTestRoom() {
        Awake();
        ClearAll();

        walls.material = MaterialBank.GetWallMaterial(1);
        for (float x = 0.5f; x <= 4.5; x++) {
            for (float z = 0.5f; z <= 6.5; z++) {
                floor.AddMarker(new Vector3(x, 0f, z)); floor.Rebuild();
                if (x == 0.5f) { walls.AddMarker(new Vector3(0f, 0f, z)); walls.Rebuild(); }
                if (z == 6.5f) { walls.AddMarker(new Vector3(x, 0f, 7f)); walls.Rebuild(); }
            }
        }
    }

    private void OnDrawGizmos() {

        //Dibujar los marcadores si no hay objetos
        if(floor.objects.Count <= 0) {
            Gizmos.color = Color.cyan;
            foreach (Vector3 marker in floor.markers) {
                Gizmos.DrawWireCube(marker, new Vector3(1f, 0f, 1f));
            }
        }

        if(walls.wallObjects.Count <= 0) {
            Gizmos.color = Color.green;
            foreach (Vector3 marker in walls.wallMarkers) {
                if(marker.x != 0f) {
                    Gizmos.DrawWireCube(marker + Vector3.up * 1.5f, new Vector3(1f, 3f, 0f));
                } else {
                    Gizmos.DrawWireCube(marker + Vector3.up * 1.5f, new Vector3(0f, 3f, 1f));
                }
            }
        }
    }

}
