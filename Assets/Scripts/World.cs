﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class World : MonoBehaviour {

    [Header("Size")]
    public int sizeX = 64;
    public int sizeZ = 64;

    [Header("Grids")]
    public FloorGrid floorGrid;
    public WallGrid wallGrid;

    [Header("Objects")]
    GameObject floor;
    GameObject walls;

    private void Awake() {
        if(floorGrid == null) {
            floorGrid = new FloorGrid(sizeX, sizeZ);
        }
        if (wallGrid == null) {
            wallGrid = new WallGrid(sizeX + 1, sizeZ + 1);
        }
    }

    void Start () {
        /*LoadTestRoom();
        RebuildFloor();*/
	}

    public void Resize() {
        FloorGrid oldFloorGrid = floorGrid;
        WallGrid oldWallGrid = wallGrid;

        //Resize floor grid
        floorGrid = new FloorGrid(sizeX, sizeZ);
        for(int x = 0; x < sizeX; x++) {
            for (int z = 0; z < sizeZ; z++) {
                if(floorGrid.InRange(x, z) && oldFloorGrid.InRange(x, z)) {
                    floorGrid[x, z] = oldFloorGrid[x, z];
                }
            }
        }

        //Resize wall grid
        wallGrid = new WallGrid(sizeX + 1, sizeZ + 1);
    }

    public void RebuildFloor() {

        Mesh floorMesh = MeshBuilder.GenerateFloorMesh(floorGrid);
        MeshFilter meshFilter = transform.Find("Floor").GetComponent<MeshFilter>();
        meshFilter.mesh = floorMesh;

        MeshRenderer renderer = meshFilter.GetComponent<MeshRenderer>();
        renderer.materials = MaterialBank.GetFloorMaterials(floorGrid.GetUsedMaterialsIds());
        
    }

    public void RebuildWalls() {

        Mesh wallMesh = MeshBuilder.GenerateWallMesh(wallGrid);
        MeshFilter meshFilter = transform.Find("Walls").GetComponent<MeshFilter>();
        meshFilter.mesh = wallMesh;
        meshFilter.GetComponent<MeshRenderer>().material = MaterialBank.GetWallMaterial(3);

    }

    public void Rebuild() {
        RebuildFloor();
        RebuildWalls();
    }

    public void ClearMeshes() {

    }

    public void LoadTestRoom() {

        sizeX = 5; sizeZ = 8;

        floorGrid = new FloorGrid(sizeX, sizeZ);
        wallGrid = new WallGrid(sizeX + 1, sizeZ + 1);

        //Place Floor
        for (int x = 0; x < floorGrid.GetLength(0); x++) {
            for (int z = 0; z < floorGrid.GetLength(1); z++) {
                floorGrid[x, z] = new FloorInfo(true, 0, 0);
            }
        }

        //Place Walls
        for (int x = 0; x < wallGrid.GetLength(0); x++) {
            for (int z = 0; z < wallGrid.GetLength(1); z++) {

                //West walls
                if (x == 0 && z != wallGrid.GetLength(1) - 1) {
                    wallGrid[x, z] = new WallNode(true, false);
                    wallGrid[x, z].SetAllMaterials(1);
                };

                //North walls
                if (z == wallGrid.GetLength(1) - 1 && x < wallGrid.GetLength(0) - 1) {
                    wallGrid[x, z] = new WallNode(false, true);
                    wallGrid[x, z].SetAllMaterials(1);
                };
            }
        }

        RebuildFloor();
        RebuildWalls();
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + sizeX * 0.5f, transform.position.y, transform.position.z + sizeZ * 0.5f), new Vector3(sizeX, 0f, sizeZ));
    }

}

