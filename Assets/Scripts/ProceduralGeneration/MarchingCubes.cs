using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MarchingCubes : MonoBehaviour
{
    [SerializeField] private float heightThreshold = 0.5f;
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshFilter = GetComponent<MeshFilter>();
    }


    public void GenerateMesh(float[,,] heightMap, int noiseScale = 10)
    {
        MarchCubes(heightMap, noiseScale);
        SetMesh();
    }

    private void SetMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        meshFilter.mesh = mesh;

        MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;

    }

    private void MarchCubes(float[,,] heightMap, int noiseScale = 10)
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < heightMap.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < heightMap.GetLength(1) - 1; y++)
            {
                for (int z = 0; z < heightMap.GetLength(2) - 1; z++)
                {
                    float[] cubeCorners = new float[8];

                    for (int i = 0; i < 8; i++)
                    {
                        Vector3Int corner = new Vector3Int(x, y, z) + MarchingTable.Corners[i];
                        cubeCorners[i] = heightMap[corner.x, corner.y, corner.z];
                    }

                    MarchCube(new Vector3(x, y, z) * noiseScale, GetConfigurationIndex(cubeCorners), noiseScale);
                }
            }
        }

    }

    private void MarchCube(Vector3 position, int configIndex, int noiseScale = 10)
    {
        if (configIndex == 0 || configIndex == 255) return;

        int edgeIndex = 0;

        for (int t = 0; t < 5; t++)
        {
            for (int v = 0; v < 3; v++)
            {
                int triTableValue = MarchingTable.Triangles[configIndex, edgeIndex];

                if (triTableValue == -1) return;

                Vector3 edgeStart = position + (MarchingTable.Edges[triTableValue, 0] * noiseScale);
                Vector3 edgeEnd = position + (MarchingTable.Edges[triTableValue, 1] * noiseScale);

                Vector3 vertex = (edgeStart + edgeEnd) / 2;

                vertices.Add(vertex);
                triangles.Add(vertices.Count - 1);

                edgeIndex++;
            }
        }
    }

    private int GetConfigurationIndex(float[] cubeCorners)
    {
        int configIndex = 0;

        for (int i = 0; i < 8; i++)
        {
            if (cubeCorners[i] > heightThreshold)
            {
                configIndex |= 1 << i;
            }
           
        }

        return configIndex;
    }

    
}
