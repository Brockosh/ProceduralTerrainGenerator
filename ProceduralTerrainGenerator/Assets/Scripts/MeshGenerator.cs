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

    public int mapWidth = 20;
    public int mapHeight = 20;
    public float noiseScale;

    public void GenerateMap()
    {
        //float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);
    }


    [SerializeField] int amplitude = 8;
    [SerializeField] float scale = 5;
    [SerializeField] int octaves;
    [SerializeField] float persistence = 5;
    [SerializeField] float lacunarity = 5;
    [SerializeField] Vector2 offset;


    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();
    }

    private void Update()
    {
        CreateShape();
        UpdateMesh();
    }


    void CreateShape()
    {
        //We need one more vertice on each axis than the amount of mesh's we want to create
        vertices = new Vector3[(mapWidth + 1) * (mapHeight + 1)];

        for (int i = 0, z = 0; z <= mapHeight; z++) 
        { 
            for (int x = 0; x <= mapWidth; x++)
            {
                //.3 multiplication zooms out of noise
                //*5 increased the amplitude, increasing height variations
                float y = Mathf.PerlinNoise(x * scale, z * scale) * amplitude;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[mapWidth * mapHeight * 6];

        int vert = 0;
        int tris = 0;

        for  (int z = 0; z < mapHeight; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + mapWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + mapWidth + 1;
                triangles[tris + 5] = vert + mapWidth + 2;
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
