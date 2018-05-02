using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WallNode {

    public bool connectedNorth = false;
    public bool connectedEast = false;

    public int northInnerMaterialId = 0;
    public int northOuterMaterialId = 0;
    public int eastInnerMaterialId = 0;
    public int eastOuterMaterialId = 0;

    public WallNode(bool connectedNorth, bool connectedEast) {
        this.connectedNorth = connectedNorth;
        this.connectedEast = connectedEast;
    }

    public WallNode() : this(false, false) { }
}
