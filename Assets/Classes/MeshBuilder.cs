using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshBuilder {

    public static Mesh GenerateFloorMesh(FloorGrid floorInfo) {

        float floorThickness = 0.13f;

        List<Vector3> vertex = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        int lastVertex = -1;

        for (int x = 0; x < floorInfo.GetLength(0); x++) {
            for (int z = 0; z < floorInfo.GetLength(1); z++) {
                FloorInfo fi = floorInfo[x, z];
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
                    tris.Add(bottomLeft); tris.Add(topLeft); tris.Add(bottomRight);
                    tris.Add(bottomRight); tris.Add(topLeft); tris.Add(topRight);
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

                        if (i == 0 && (z == 0 || floorInfo[x, z - 1].present == false)) {
                            needsThickness = true;
                            thickStart = new Vector3(x, fi.height, z);
                            thickEnd = new Vector3(x + 1, fi.height, z);
                            thickNormal = -Vector3.forward;
                        }
                        if (i == 1 && (x == floorInfo.GetLength(0) - 1 || floorInfo[x + 1, z].present == false)) {
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
                            tris.Add(bottomLeft); tris.Add(topLeft); tris.Add(bottomRight);
                            tris.Add(bottomRight); tris.Add(topLeft); tris.Add(topRight);
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

        Mesh floorMesh = new Mesh(); floorMesh.name = "floor_mesh";

        floorMesh.SetVertices(vertex);
        floorMesh.SetTriangles(tris.ToArray(), 0);
        floorMesh.normals = normals.ToArray();
        floorMesh.uv = uvs.ToArray();
        //floorMesh.RecalculateTangents();

        return floorMesh;
    }

    public static Mesh GenerateWallMesh(WallNode[,] wallNodes) {
        return new Mesh();
    }

}