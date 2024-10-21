using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    public int xSize = 20;
    public int zSize = 20;

    [SerializeField] int amplitude = 8;
    [SerializeField] float scale = 0.02f;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        //We need one more vertice on each axis than the amount of mesh's we want to create
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        

        for (int i = 0, z = 0; z <= zSize; z++) 
        { 
            for (int x = 0; x <= xSize; x++)
            {
                //.3 multiplication zooms out of noise
                //*5 increased the amplitude, increasing height variations
                float y = Mathf.PerlinNoise(x * scale, z * scale) * amplitude;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        for  (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;
                vert++;
                tris += 6;

            }
            vert++;
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }


}
