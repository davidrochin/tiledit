using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTile {

    public Room room;

    public int height = 0;
    public RoomVector position;

    public string tileName;

    public RoomTile() {

    }
}

public struct RoomVector {

    public int x;
    public int z;

    public RoomVector(int x, int z) {
        this.x = x;
        this.z = z;
    }
}
