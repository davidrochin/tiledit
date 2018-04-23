using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

    [Header("Debug")]
    public List<GameObject> floor;

    public WallManager wallManager;

    private void Awake() {
        floor = new List<GameObject>();
        wallManager = gameObject.AddComponent<WallManager>();
    }

    void Start () {
        for (int x = 0; x <= 4; x++) {
            for (int z = 0; z <= 6; z++) {
                SetFloor(new Vector3(x, 0f, z));
                if (x == 0f) { wallManager.AddPoint(new Vector3(-0.5f, 0f, z)); wallManager.Rebuild(); }
                if (z == 6f) { wallManager.AddPoint(new Vector3(x, 0f, 6.5f)); wallManager.Rebuild(); }
            }
        }
	}

    public bool SetFloor(Vector3 position) {

        GameObject floorPrefab = Structure.GetPrefab("structure_floor");

        if(floor.Find(a => a.transform.position == position) == null) {
            GameObject inst = Instantiate(floorPrefab, transform);
            floor.Add(inst);
            inst.transform.localPosition = position;
        } else {
            Debug.Log("Ya habia un " + position + ". " + floor.Find(a => a.transform.position == position).name);
        }
        return false;
    }

}
