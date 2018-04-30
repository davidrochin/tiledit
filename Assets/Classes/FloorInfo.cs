using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct FloorInfo {

    public bool present;
    public int height;
    public int materialId;

    public FloorInfo(bool present, int height, int materialId) {
        this.present = present;
        this.height = height;
        this.materialId = materialId;
    }

    public FloorInfo(bool present) : this(present, 0, 0) {}

    public FloorInfo(bool present, int height) : this(present, height, 0) {}

}
