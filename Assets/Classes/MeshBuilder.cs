using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder {

    static float wallThickness = 0.2f;
    static float wallHeight = 3f;

    public static Mesh GenerateFloorMesh(FloorGrid floorGrid) {

        float floorThickness = 0.13f;

        //Get how many Materials this grid has
        int[] materialIDS = floorGrid.GetUsedMaterialsIds();
        Mesh floorMesh = new Mesh(); floorMesh.name = "floor_mesh";
        floorMesh.subMeshCount = materialIDS.Length;

        //Initialize the Lists
        List<Vector3> vertex = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int lastVertex = -1;

        //Every Material in this Mesh has to be one Submesh. Do the whole thing for every Material
        for (int m = 0; m < materialIDS.Length; m++) {

            //Reset triangle List because it is another SubMesh
            triangles = new List<int>();

            //Iterate throught the Grid
            for (int x = 0; x < floorGrid.GetLength(0); x++) {
                for (int z = 0; z < floorGrid.GetLength(1); z++) {

                    //Obtain the FloorInfo of this cell
                    FloorInfo fi = floorGrid[x, z];

                    //IF the cell has the material that we are currently building...
                    if(fi.materialId == materialIDS[m]) {
                        if (fi.present) {

                            //Añadir los vertices
                            vertex.Add(new Vector3(x, fi.height, z));          //Bottom Left
                            vertex.Add(new Vector3(x, fi.height, z + 1));      //Top Left
                            vertex.Add(new Vector3(x + 1f, fi.height, z));     //Bottom Right
                            vertex.Add(new Vector3(x + 1, fi.height, z + 1));  //Top Right

                            //Calcular los indices de los vertices para construir los tris
                            int bottomLeft = lastVertex + 1;
                            int topLeft = lastVertex + 2;
                            int bottomRight = lastVertex + 3;
                            int topRight = lastVertex + 4;

                            //Construir los dos triangulos usando los vertices en sentido de las agujas del reloj
                            triangles.Add(bottomLeft); triangles.Add(topLeft); triangles.Add(bottomRight);
                            triangles.Add(bottomRight); triangles.Add(topLeft); triangles.Add(topRight);
                            lastVertex += 4;

                            //Añadir las normales hacia arriba
                            normals.Add(Vector3.up); normals.Add(Vector3.up);
                            normals.Add(Vector3.up); normals.Add(Vector3.up);

                            //Añadir los mapas UV para las texturas
                            uvs.Add(new Vector2(0, 0)); uvs.Add(new Vector2(0, 1));
                            uvs.Add(new Vector2(1, 0)); uvs.Add(new Vector2(1, 1));

                            bool needsThickness = false; Vector3 thickNormal = new Vector3();
                            Vector3 thickStart = new Vector3(); Vector3 thickEnd = new Vector3();

                            //Determinar si es necesario añadir grosor, y de donde a donde
                            for (int i = 0; i < 4; i++) {

                                needsThickness = false;

                                if (i == 0 && (z == 0 || floorGrid[x, z - 1].present == false)) {
                                    needsThickness = true;
                                    thickStart = new Vector3(x, fi.height, z);
                                    thickEnd = new Vector3(x + 1, fi.height, z);
                                    thickNormal = -Vector3.forward;
                                }
                                if (i == 1 && (x == floorGrid.GetLength(0) - 1 || floorGrid[x + 1, z].present == false)) {
                                    needsThickness = true;
                                    thickStart = new Vector3(x + 1, fi.height, z);
                                    thickEnd = new Vector3(x + 1, fi.height, z + 1);
                                    thickNormal = Vector3.right;
                                }

                                //Añadir efecto de grosor de ser necesario
                                if (needsThickness) {
                                    vertex.Add(thickStart);                                //Top Left
                                    vertex.Add(thickStart + Vector3.down * floorThickness);//Bottom Left
                                    vertex.Add(thickEnd);                                  //Top Right
                                    vertex.Add(thickEnd + Vector3.down * floorThickness);  //Bottom Right
                                    topLeft = lastVertex + 1;
                                    bottomLeft = lastVertex + 2;
                                    topRight = lastVertex + 3;
                                    bottomRight = lastVertex + 4;
                                    triangles.Add(bottomLeft); triangles.Add(topLeft); triangles.Add(bottomRight);
                                    triangles.Add(bottomRight); triangles.Add(topLeft); triangles.Add(topRight);
                                    lastVertex += 4;
                                    normals.Add(thickNormal); normals.Add(thickNormal);
                                    normals.Add(thickNormal); normals.Add(thickNormal);
                                    uvs.Add(new Vector2(0, 1)); uvs.Add(new Vector2(0f, 1f - floorThickness));
                                    uvs.Add(new Vector2(1, 1)); uvs.Add(new Vector2(1, 1f - floorThickness));
                                }
                            }

                            
                        }
                    }
                }
            }

            //Debug.Log("V:" + vertex.Count + ", T:" + triangles.Count + ", N:" + normals.Count + ", UV:" + uvs.Count);

            floorMesh.vertices = vertex.ToArray();
            floorMesh.SetTriangles(triangles.ToArray(), m);
            floorMesh.normals = normals.ToArray();
            floorMesh.uv = uvs.ToArray();
        }

        return floorMesh;
    }

    public static Mesh GenerateWallMesh(WallGrid wallGrid) {

        float halfThickness = wallThickness * 0.5f;

        //Get how many Materials this grid has
        //int[] materialIDS = wallGrid.GetUsedMaterialsIds();
        Mesh wallMesh = new Mesh(); wallMesh.name = "wall_mesh";
        //wallMesh.subMeshCount = materialIDS.Length;

        List<CombineInstance> meshesToCombine = new List<CombineInstance>();
        CombineInstance ci;

        //Iterate throught the Grid
        for (int x = 0; x < wallGrid.GetLength(0); x++) {
            for (int z = 0; z < wallGrid.GetLength(1); z++) {

                //Obtain the WallNode of this cell
                WallNode node = wallGrid[x, z];

                //Generate connection to North if necessary
                if (node.connectedNorth) {

                    bool intersectsOnNorth = false; if ((wallGrid.InRange(x, z + 1) && wallGrid[x, z + 1].connectedEast) || (wallGrid.InRange(x - 1, z + 1) && wallGrid[x - 1, z + 1].connectedEast)) { intersectsOnNorth = true; }
                    bool intersectsOnSouth = false; if (wallGrid[x, z].connectedEast || (x - 1 >= 0 && wallGrid[x - 1, z].connectedEast)) { intersectsOnSouth = true; }

                    //North Inner Wall
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x, 0f, z) + Vector3.right * wallThickness * 0.5f + Vector3.forward * (intersectsOnSouth ? halfThickness : 0f),
                        new Vector3(x, 0f, z) + Vector3.right * wallThickness * 0.5f + Vector3.forward * 1f + Vector3.up * wallHeight - Vector3.forward * (intersectsOnNorth ? halfThickness : 0f),
                        Vector3.right, 0,
                        intersectsOnSouth ? halfThickness : 0f,
                        intersectsOnNorth ? halfThickness : 0f
                        );
                    meshesToCombine.Add(ci);

                    //North Outer Wall
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x, 0f, z - (intersectsOnNorth ? halfThickness : 0f)) + -Vector3.right * wallThickness * 0.5f + Vector3.forward * 1f,
                        new Vector3(x, 0f, z + (intersectsOnSouth ? halfThickness : 0f)) + -Vector3.right * wallThickness * 0.5f + Vector3.up * wallHeight,
                        -Vector3.right, 0,
                        intersectsOnNorth ? halfThickness : 0f,
                        intersectsOnSouth ? halfThickness : 0f
                        );
                    meshesToCombine.Add(ci);

                    //Add Top
                    ci = new CombineInstance();
                    ci.mesh = GetPlane(
                        new Vector3(x, 0f, z + (intersectsOnSouth ? halfThickness : 0f)) + Vector3.up * wallHeight - Vector3.right * wallThickness * 0.5f,
                        new Vector3(x, 0f, z) + Vector3.up * wallHeight - Vector3.right * wallThickness * 0.5f + Vector3.forward * (1f - (intersectsOnNorth ? halfThickness : 0f)),
                        new Vector3(x, 0f, z) + Vector3.up * wallHeight - Vector3.right * wallThickness * 0.5f + Vector3.forward * (1f - (intersectsOnNorth ? halfThickness : 0f)) + Vector3.right * wallThickness,
                        new Vector3(x, 0f, z + (intersectsOnSouth ? halfThickness : 0f)) + Vector3.up * wallHeight - Vector3.right * wallThickness * 0.5f + Vector3.right * wallThickness,
                        Vector3.up, 0
                        );
                    meshesToCombine.Add(ci);

                }

                //Generate connection to East if necessary
                if (node.connectedEast) {

                    bool intersectsOnEast = false; if (wallGrid.GetSafe(x + 1, z).connectedNorth || wallGrid.GetSafe(x + 1, z - 1).connectedNorth) { intersectsOnEast = true; }
                    bool intersectsOnWest = false; if (node.connectedNorth || wallGrid.GetSafe(x, z - 1).connectedNorth) { intersectsOnWest = true; }

                    //East Inner Wall
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x + 1f - (intersectsOnEast ? halfThickness : 0f), 0f, z + halfThickness),
                        new Vector3(x + (intersectsOnWest ? halfThickness : 0f), wallHeight, z + halfThickness),
                        Vector3.forward, 0,
                        intersectsOnEast ? halfThickness : 0f,
                        intersectsOnWest ? halfThickness : 0f
                        );
                    meshesToCombine.Add(ci);

                    //East Outer Wall
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x + (intersectsOnWest ? halfThickness : 0f), 0f, z - halfThickness),
                        new Vector3(x + 1f - (intersectsOnEast ? halfThickness : 0f), wallHeight, z - halfThickness),
                        -Vector3.forward, 0,
                        intersectsOnWest ? halfThickness : 0f,
                        intersectsOnEast ? halfThickness : 0f
                        );
                    meshesToCombine.Add(ci);

                    //Add Top
                    ci = new CombineInstance();
                    ci.mesh = GetPlane(
                        new Vector3(x + (intersectsOnWest ? halfThickness : 0f), 0f + wallHeight, z - wallThickness * 0.5f),
                        new Vector3(x + (intersectsOnWest ? halfThickness : 0f), 0f + wallHeight, z + wallThickness * 0.5f),
                        new Vector3(x + 1f - (intersectsOnEast ? halfThickness : 0f), 0f + wallHeight, z + wallThickness * 0.5f),
                        new Vector3(x + 1f - (intersectsOnEast ? halfThickness : 0f), 0f + wallHeight, z - wallThickness * 0.5f),
                        Vector3.up, 0
                        );
                    meshesToCombine.Add(ci);
                }

                //Generate intersection if necessary
                if (wallGrid.IsIntersection(x, z)) {

                    //Top
                    ci = new CombineInstance();
                    Vector3 planeBottomLeft = new Vector3(x - halfThickness, wallHeight, z - halfThickness);
                    ci.mesh = GetPlane(
                        planeBottomLeft,
                        planeBottomLeft + Vector3.forward * wallThickness,
                        planeBottomLeft + Vector3.forward * wallThickness + Vector3.right * wallThickness,
                        planeBottomLeft + Vector3.right * wallThickness,
                        Vector3.up, 0
                        );
                    meshesToCombine.Add(ci);

                    //Intersection EAST Wall
                    if (!node.connectedEast) {
                        ci = new CombineInstance();
                        ci.mesh = GetIntersectionWall(
                            new Vector3(x + halfThickness, 0f, z - halfThickness),
                            new Vector3(x + halfThickness, wallHeight, z + halfThickness),
                            Vector3.right, 0
                            );
                        meshesToCombine.Add(ci);
                    }

                    //Intersection WEST Wall
                    if (!wallGrid.GetSafe(x - 1, z).connectedEast) {
                        ci = new CombineInstance();
                        ci.mesh = GetIntersectionWall(
                            new Vector3(x - halfThickness, 0f, z + halfThickness),
                            new Vector3(x - halfThickness, wallHeight, z - halfThickness),
                            -Vector3.right, 0
                            );
                        meshesToCombine.Add(ci);
                    }

                    //Intersection NORTH Wall
                    if (!node.connectedNorth) {
                        ci = new CombineInstance();
                        ci.mesh = GetIntersectionWall(
                            new Vector3(x + halfThickness, 0f, z + halfThickness),
                            new Vector3(x - halfThickness, wallHeight, z + halfThickness),
                            Vector3.forward, 0
                            );
                        meshesToCombine.Add(ci);
                    }

                    //Intersection SOUTH Wall
                    if (!wallGrid.GetSafe(x, z -1).connectedNorth) {
                        ci = new CombineInstance();
                        ci.mesh = GetIntersectionWall(
                            new Vector3(x - halfThickness, 0f, z - halfThickness),
                            new Vector3(x + halfThickness, wallHeight, z - halfThickness),
                            -Vector3.forward, 0
                            );
                        meshesToCombine.Add(ci);
                    }

                }

                //Generate Wall ends and starts if neccesary

                //Start NORTH Wall
                if (node.connectedNorth && !wallGrid.IsIntersection(x, z) && !wallGrid.GetSafe(x, z - 1).connectedNorth) {
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x - halfThickness, 0f, z),
                        new Vector3(x + halfThickness, wallHeight, z),
                        -Vector3.forward, 0,
                        (1f - wallThickness) * 0.5f,
                        (1f - wallThickness) * 0.5f
                        );
                    meshesToCombine.Add(ci);
                }

                //End NORTH Wall
                if (node.connectedNorth && !wallGrid.GetSafe(x, z + 1).IsConnected()) {
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x + halfThickness, 0f, z + 1f),
                        new Vector3(x - halfThickness, wallHeight, z + 1f),
                        Vector3.forward, 0,
                        (1f - wallThickness) * 0.5f,
                        (1f - wallThickness) * 0.5f
                        );
                    meshesToCombine.Add(ci);
                }

                //Start EAST Wall
                if (node.connectedEast && !wallGrid.IsIntersection(x, z) && !wallGrid.GetSafe(x - 1, z).connectedEast) {
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x, 0f, z + halfThickness),
                        new Vector3(x, wallHeight, z - halfThickness),
                        -Vector3.right, 0,
                        (1f - wallThickness) * 0.5f,
                        (1f - wallThickness) * 0.5f
                        );
                    meshesToCombine.Add(ci);
                }

                //End EAST Wall
                if (node.connectedEast && !wallGrid.GetSafe(x + 1, z).IsConnected()) {
                    ci = new CombineInstance();
                    ci.mesh = GetWallPlane(
                        new Vector3(x + 1f, 0f, z - halfThickness),
                        new Vector3(x + 1f, wallHeight, z + halfThickness),
                        Vector3.right, 0,
                        (1f - wallThickness) * 0.5f,
                        (1f - wallThickness) * 0.5f
                        );
                    meshesToCombine.Add(ci);
                }
            }
        }

        //Debug.Log(meshesToCombine.Count);
        wallMesh.CombineMeshes(meshesToCombine.ToArray(), true, false, false);

        return wallMesh;
    }

    static Mesh GetPart(Part partType, Vector3 start, int subMeshIndex) {

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int lastVertex = -1;

        Mesh part = new Mesh();

        switch (partType) {
            case Part.WallNorth:

                //Armar el primer cuadro
                vertices.Add(start);
                vertices.Add(start + -Vector3.right * wallThickness * 0.5f); //Bottom Left
                vertices.Add(vertices[vertices.Count - 1] + Vector3.up * wallHeight); //Top Left
                vertices.Add(vertices[vertices.Count - 1] + Vector3.right * wallThickness); //Top Right
                vertices.Add(vertices[vertices.Count - 1] + Vector3.down * wallHeight); //Bottom Left
                break;
        }

        return part;

    }

    static Mesh GetWallPlane(Vector3 bottomLeftPos, Vector3 topRightPos, Vector3 normal, int subMesh, float uvLeftOff, float uvRightOff) {
        Mesh planeMesh = new Mesh();

        //Initialize the Lists
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int lastVertex = -1;

        //Armar el primer cuadro
        vertices.Add(bottomLeftPos); //Bottom Left
        vertices.Add(bottomLeftPos + Vector3.up * (topRightPos - bottomLeftPos).y); //Top Left
        vertices.Add(topRightPos); //Top Right
        vertices.Add(topRightPos + Vector3.down * (topRightPos - bottomLeftPos).y); //Bottom Right

        //Calcular los indices de los vertices para construir los tris
        int bottomLeft = lastVertex + 1;
        int topLeft = lastVertex + 2;
        int topRight = lastVertex + 3;
        int bottomRight = lastVertex + 4;

        //Construir los dos triangulos usando los vertices en sentido de las agujas del reloj
        triangles.Add(bottomLeft); triangles.Add(topLeft); triangles.Add(bottomRight);
        triangles.Add(bottomRight); triangles.Add(topLeft); triangles.Add(topRight);
        lastVertex += 4;

        //Añadir las normales
        normals.Add(normal); normals.Add(normal);
        normals.Add(normal); normals.Add(normal);

        //Añadir los mapas UV para las texturas
        uvs.Add(new Vector2(0 + uvLeftOff, 0)); uvs.Add(new Vector2(0 + uvLeftOff, 1));
        uvs.Add(new Vector2(1 - uvRightOff, 1)); uvs.Add(new Vector2(1 - uvRightOff, 0));

        planeMesh.vertices = vertices.ToArray();
        planeMesh.SetTriangles(triangles.ToArray(), 0);
        planeMesh.normals = normals.ToArray();
        planeMesh.uv = uvs.ToArray();

        return planeMesh;
    }

    static Mesh GetWallPlane(Vector3 bottomLeftPos, Vector3 topRightPos, Vector3 normal, int subMesh) {
        return GetWallPlane(bottomLeftPos, topRightPos, normal, subMesh, 0f, 0f);
    }

    static Mesh GetIntersectionWall(Vector3 bottomLeftPos, Vector3 topRightPos, Vector3 normal, int subMesh) {
        List<CombineInstance> combineInstances = new List<CombineInstance>();

        Vector3 topLeftPos = new Vector3(bottomLeftPos.x, topRightPos.y, bottomLeftPos.z);
        Vector3 topRightPosLeftPart = topLeftPos + (topRightPos - topLeftPos).normalized * (topRightPos - topLeftPos).magnitude * 0.5f;

        Vector3 bottomRightPos = new Vector3(topRightPos.x, bottomLeftPos.y, topRightPos.z);
        Vector3 bottomLeftPosRightPart = bottomLeftPos + (bottomRightPos - bottomLeftPos).normalized * (bottomRightPos - bottomLeftPos).magnitude * 0.5f;

        //Left part
        CombineInstance ci = new CombineInstance();
        ci.mesh = GetWallPlane(
            bottomLeftPos,
            topRightPosLeftPart,
            normal, subMesh,
            1f - wallThickness * 0.5f, 
            0f);
        combineInstances.Add(ci);

        //Right part
        ci = new CombineInstance();
        ci.mesh = GetWallPlane(
            bottomLeftPosRightPart,
            topRightPos,
            normal, subMesh,
            0f,
            1f - wallThickness * 0.5f);
        combineInstances.Add(ci);

        Mesh mesh = new Mesh(); mesh.CombineMeshes(combineInstances.ToArray(), true, false, false);
        return mesh;
    }

    static Mesh GetPlane(Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, Vector3 normal, int subMesh) {
        Mesh planeMesh = new Mesh();

        //Initialize the Lists
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int lastVertex = -1;

        //Armar el primer cuadro
        vertices.Add(bl); //Bottom Left
        vertices.Add(tl); //Top Left
        vertices.Add(tr); //Top Right
        vertices.Add(br); //Bottom Right

        //Calcular los indices de los vertices para construir los tris
        int bottomLeft = lastVertex + 1;
        int topLeft = lastVertex + 2;
        int topRight = lastVertex + 3;
        int bottomRight = lastVertex + 4;

        //Construir los dos triangulos usando los vertices en sentido de las agujas del reloj
        triangles.Add(bottomLeft); triangles.Add(topLeft); triangles.Add(bottomRight);
        triangles.Add(bottomRight); triangles.Add(topLeft); triangles.Add(topRight);
        lastVertex += 4;

        //Añadir las normales
        normals.Add(normal); normals.Add(normal);
        normals.Add(normal); normals.Add(normal);

        //Añadir los mapas UV para las texturas
        uvs.Add(new Vector2(0, 0)); uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1)); uvs.Add(new Vector2(1, 0));

        planeMesh.vertices = vertices.ToArray();
        planeMesh.SetTriangles(triangles.ToArray(), 0);
        planeMesh.normals = normals.ToArray();
        planeMesh.uv = uvs.ToArray();

        return planeMesh;
    }

    public enum Part { FloorTop, FloorTicknes, WallNorth, WallEast, WallIntersection}
}