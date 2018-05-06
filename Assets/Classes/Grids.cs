using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISerializableGrid<CellInfo> {
    int GetLength(int dimension);
    CellInfo[] GetAll();
    int[] GetUsedMaterialsIds();
    bool InRange(int col, int row);
    CellInfo this[int col, int row] { get; set; }
}

[System.Serializable]
public class FloorGrid : ISerializableGrid<FloorInfo> {

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

    public int[] GetUsedMaterialsIds() {
        List<int> materialsID = new List<int>();
        foreach (FloorInfo current in GetAll()) {
            if (!materialsID.Exists(a => a == current.materialId)) {
                materialsID.Add(current.materialId);
            }
        }
        materialsID.Sort();
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

    public bool InRange(int x, int y) {
        return true;
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

[System.Serializable]
public class WallGrid : ISerializableGrid<WallNode> {

    public WallCol[] cols;

    public WallGrid(int sizeX, int sizeY) {
        cols = new WallCol[sizeX];
        for (int x = 0; x < sizeX; x++) {
            cols[x] = new WallCol(sizeY);
        }
    }

    public WallNode this[int col, int row] {
        get { return cols[col].rows[row]; }
        set { cols[col].rows[row] = value; }
    }

    public WallNode[] GetAll() {
        List<WallNode> all = new List<WallNode>();
        foreach (WallCol col in cols) {
            foreach (WallNode current in col.rows) {
                all.Add(current);
            }
        }
        return all.ToArray();
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

    public int[] GetUsedMaterialsIds() {
        List<int> materialsID = new List<int>();
        foreach (WallNode current in GetAll()) {
            if (!materialsID.Exists(a => a == current.eastInnerMaterialId)) {
                materialsID.Add(current.eastInnerMaterialId);
            }
            if (!materialsID.Exists(a => a == current.eastOuterMaterialId)) {
                materialsID.Add(current.eastOuterMaterialId);
            }
            if (!materialsID.Exists(a => a == current.northInnerMaterialId)) {
                materialsID.Add(current.northInnerMaterialId);
            }
            if (!materialsID.Exists(a => a == current.northOuterMaterialId)) {
                materialsID.Add(current.northOuterMaterialId);
            }
        }
        materialsID.Sort();
        return materialsID.ToArray();
    }

    public bool InRange(int x, int y) {
        if (x < 0 || x >= GetLength(0)) return false;
        if (y < 0 || y >= GetLength(1)) return false;
        return true;
    }

    public WallNode GetSafe(int x, int y) {
        if(!InRange(x, y)) {
            return new WallNode();
        } else {
            return this[x, y];
        }
    }

    public int GetIntersectionsCount(int x, int z) {
        int count = 0;
        if (this[x, z].connectedNorth) count++;
        if (this[x, z].connectedEast) count++;
        if (GetSafe(x - 1, z).connectedEast) count++;
        if (GetSafe(x, z - 1).connectedNorth) count++;
        return count;
    }

    public bool IsIntersection(int x, int z) {
        int intersectionsCount = GetIntersectionsCount(x, z);
        if (intersectionsCount >= 3) {
            return true;
        } else if(intersectionsCount <= 1){
            return false;
        } else {
            if(this[x, z].connectedNorth && GetSafe(x, z - 1).connectedNorth) {
                return false;
            }
            if (this[x, z].connectedEast && GetSafe(x - 1, z).connectedEast) {
                return false;
            } else {
                return true;
            }
        }
    }

    [System.Serializable]
    public class WallCol {

        public WallCol(int rows) {
            this.rows = new WallNode[rows];
            for (int i = 0; i < this.rows.Length; i++) {
                this.rows[i] = new WallNode();
            }
        }

        public WallNode[] rows;
    }
}
