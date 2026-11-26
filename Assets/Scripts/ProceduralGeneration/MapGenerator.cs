using UnityEngine;
using Unity.Mathematics;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int width = 100;
    [SerializeField] private int height = 100;
    [SerializeField] private int noiseScale = 10;
    [SerializeField] private int terraceSteps = 15;
    [SerializeField] private float noiseResolution = 0.05f;
    [SerializeField] private float caveResolution = 0.1f;
    [SerializeField] private bool visualizeNoise;
    [SerializeField] private AnimationCurve caveDensityCurve;
    [SerializeField] private AnimationCurve mountainDensityCurve;
    [SerializeField] private AnimationCurve mountainHeightCurve;
    private float[,,] heights;
    private MarchingCubes marchingCubes;

    private void Start()
    {
        if (TryGetComponent<MarchingCubes>(out MarchingCubes cubes))
        {
            marchingCubes = cubes;
        }

        if (marchingCubes == null) return;

        SetCanyonBase();
        SetMountains();
        FlattenTops();
        CreateTunnels();
        TerraceTerrain();
        marchingCubes.GenerateMesh(heights, noiseScale);

    }

    private void SetCanyonBase()
    {
        heights = new float[width + 1, height + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float newHeight = 1f;

                    float heightPercent = (float)y / (float)(height + 1);
                    if (heightPercent < 0.7f)
                    {
                        newHeight = 0f;
                    }

                    Vector3Int currentPoint = new Vector3Int(x, y, z);
                    Vector3Int middlePoint = new Vector3Int(width / 2, height / 2, z);
                    float radius = width * 0.4f;
                    if (Vector3Int.Distance(middlePoint, currentPoint) < radius)
                    {
                        newHeight = 1f;
                    }

                    if (heightPercent < 0.2f)
                    {
                        newHeight = 0f;
                    }
                

                    heights[x, y, z] = newHeight;
                }
            }
        }
    }

    private void SetMountains()
    {
        float[,] heightsY = new float[width + 1, width + 1];

        for (int x = 0; x < width + 1; x++)
        {
            for (int z = 0; z < width + 1; z++)
            {
                for (int y = 0; y < height + 1; y++)
                {
                    if (y == height) continue;
                    if (heights[x, y + 1, z] == 1f)
                    {
                        heightsY[x, z] = (float)y / (float)(height + 1);
                    }
                }
            }
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float heightNoise = Mathf.PerlinNoise(transform.position.x + x * noiseResolution, transform.position.z +  z * noiseResolution);
                    float additionalHeight = height * mountainHeightCurve.Evaluate(heightNoise);
                    additionalHeight = additionalHeight * mountainDensityCurve.Evaluate((float)x / (float)width) * 0.02f;
                

                    heightsY[x, z] = additionalHeight;
                }
            }
        }

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float heightPercent = (float)y / (float)(height + 1);
                    if (heightPercent < heightsY[x, z])
                    {
                        heights[x, y, z] = 0f;
                    }
                }
            }
        }
    }

    private void FlattenTops()
    {
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float heightPercent = (float)y / (float)(height + 1);
                    if (heightPercent > 0.7f)
                    {
                        heights[x, y, z] = 1f;
                    }
                }
            }
        }
    }

    private void CreateTunnels()
    {
        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    float currentHeight = height * noise.snoise((float3)transform.position + new float3(x, y, z) * caveResolution);
                    currentHeight *= caveDensityCurve.Evaluate((float)x / (float)(width + 1));
                    currentHeight *= caveDensityCurve.Evaluate((float)y / (float)(height + 1));
                    currentHeight = Mathf.RoundToInt(currentHeight);
                    if (heights[x, y, z] <= 0f)
                    {
                        heights[x, y, z] = currentHeight;
                    }


                }
            }
        }
    }

    private void TerraceTerrain()
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (!visualizeNoise || !Application.isPlaying) return;

        for (int x = 0; x < width + 1; x++)
        {
            for (int y = 0; y < height + 1; y++)
            {
                for (int z = 0; z < width + 1; z++)
                {
                    Gizmos.color = new Color(heights[x, y, z], heights[x, y, z], heights[x, y, z], 1f);
                    Gizmos.DrawCube(new Vector3(x, y, z) * noiseScale, Vector3.one);
                }
            }
        }
    }


}
