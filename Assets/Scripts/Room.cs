using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Walls))]
[RequireComponent(typeof(Floor))]

public class Room : MonoBehaviour {

    [Header("Debug")]
    public List<GameObject> floorList;

    public Walls walls;
    public Floor floor;

    private void Awake() {
        floorList = new List<GameObject>();

        if((walls = GetComponent<Walls>()) == null) {
            walls = gameObject.AddComponent<Walls>();
        }
        if ((floor = GetComponent<Floor>()) == null) {
            floor = gameObject.AddComponent<Floor>();
        }

    }

    void Start () {
        LoadTestRoom();
	}

    void LoadTestRoom() {
        walls.ClearMarkers();
        walls.wallMaterial = MaterialBank.GetWallMaterial(1);
        for (float x = 0.5f; x <= 4.5; x++) {
            for (float z = 0.5f; z <= 6.5; z++) {
                floor.AddMarker(new Vector3(x, 0f, z)); floor.Rebuild();
                if (x == 0.5f) { walls.AddMarker(new Vector3(0f, 0f, z)); walls.Rebuild(); }
                if (z == 6.5f) { walls.AddMarker(new Vector3(x, 0f, 7f)); walls.Rebuild(); }
            }
        }
    }

}
