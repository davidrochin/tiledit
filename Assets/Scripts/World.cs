using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour {

    [Header("Size")]
    public int sizeX = 64;
    public int sizeZ = 64;

    [Header("Arrays")]
    public FloorGrid floorGrid;
    public WallNode[,] wallNodes = new WallNode[0, 0];

    [Header("Objects")]
    GameObject floor;
    GameObject walls;

    private void Awake() {
        if(floorGrid == null) {
            floorGrid = new FloorGrid(sizeX, sizeZ);
        }
    }

    void Start () {
        /*LoadTestRoom();
        RebuildFloor();*/
	}

    public void RebuildFloor() {

        Mesh floorMesh = MeshBuilder.GenerateFloorMesh(floorGrid);
        MeshFilter meshFilter = transform.Find("Floor").GetComponent<MeshFilter>();
        meshFilter.mesh = floorMesh;
        meshFilter.GetComponent<MeshRenderer>().material = MaterialBank.GetFloorMaterial(0);
        
    }

    public void RebuildWalls() {

        Mesh wallMesh = MeshBuilder.GenerateWallMesh(wallNodes);
        MeshFilter meshFilter = transform.Find("Walls").GetComponent<MeshFilter>();
        meshFilter.mesh = wallMesh;
        meshFilter.GetComponent<MeshRenderer>().material = MaterialBank.GetWallMaterial(0);

    }

    public void ClearMeshes() {

    }

    public void LoadTestRoom() {
        floorGrid = new FloorGrid(5, 8);
        wallNodes = new WallNode[6, 9];
        for (int x = 0; x < floorGrid.GetLength(0); x++) {
            for (int z = 0; z < floorGrid.GetLength(1); z++) {
                floorGrid[x, z] = new FloorInfo(true, 0, 0);
                //if(x == floorInfo.GetLength(0) - 1 && z == 0) { floorInfo[x, z] = new FloorInfo(false, 0, 0); }
            }
        }
        RebuildFloor();
    }

    private void OnDrawGizmos() {

    }

}

[System.Serializable]
public class FloorGrid {

    public FloorCol[] cols;

    public FloorGrid(int sizeX, int sizeY) {
        cols = new FloorCol[sizeX];

        for (int x = 0; x < sizeX; x++) {
            cols[x] = new FloorCol(sizeY);
        }

    }

    public int GetLength(int dimension) {
        switch (dimension) {
            case 0:
                return cols.Length;
            case 1:
                return cols[0].rows.Length;
        }
        return -1;
    }

    public int[] GetUsedMaterialsIDS() {
        List<int> materialsID = new List<int>();
        foreach (FloorInfo current in GetAll()) {
            if(!materialsID.Exists(a => a == current.materialId)) {
                materialsID.Add(current.materialId);
            }
        }
        return materialsID.ToArray();
    }

    public FloorInfo[] GetAll() {
        List<FloorInfo> all = new List<FloorInfo>();
        foreach (FloorCol col in cols) {
            foreach (FloorInfo current in col.rows) {
                all.Add(current);
            }
        }
        return all.ToArray();
    }

    public FloorInfo this[int colIndex, int rowIndex] {
        get { return cols[colIndex].rows[rowIndex]; }
        set { cols[colIndex].rows[rowIndex] = (FloorInfo)value; }
    }

    [System.Serializable]
    public class FloorCol {

        public FloorCol(int rows) {
            this.rows = new FloorInfo[rows];
        }

        public FloorInfo[] rows;
    }

}
