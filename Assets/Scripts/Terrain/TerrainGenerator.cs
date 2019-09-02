using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class TerrainGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] verctices;
    int[] triangles;

    public int xSize = 100;
    public int zSize = 30;

    public float frecuency = 2f;
    public float scale = 0.3f;

    // Start is called before the first frame update
    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void CreateShape()
    {
        verctices = new Vector3[(xSize + 1) * (zSize + 1)];

        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * scale, z * scale) * frecuency; 
                verctices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;
        int modifiedTriangles = 0;
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;
                modifiedTriangles += 6;
            }
            vert++;
        }
    }
    private void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = verctices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();

         var col = gameObject.AddComponent<MeshCollider>();
        col.sharedMesh = mesh;
    }
}
