using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    [TabGroup("Dependencies"), SerializeField]
    private TerrainData terrainData;

    [TabGroup("Dependencies"), SerializeField]
    private Terrain terrain;

    [TabGroup("Dependencies"), SerializeField]
    private TerrainCollider terrainCollider;

    [TabGroup("Generation"), SerializeField]
    private float frequency;

    [TabGroup("Generation"), SerializeField]
    private float amplitude;

    [TabGroup("Generation"), SerializeField]
    private float canyonAmplitude;

    [TabGroup("Generation"), SerializeField]
    private float canyonThreshold;

    private float[,] targetHeightArray;
    private float[,] currentHeightArray;
    private float[,] prevHeightArray;

    private float updateTimer;
    private float generateNewTimer;

    private float lastGenTime;

    private void FixedUpdate()
    {
        terrainCollider.enabled = false;
        terrainCollider.enabled = true;
    }

    private void Awake()
    {
        GenerateTerrain();
    }

    [Button("Generate New Terrain")]
    private void GenerateTerrainTest()
    {
        GenerateTerrain();
        terrainData.SetHeights(0, 0, targetHeightArray);
    }

    private void GenerateTerrain()
    {
        var terrainWidth = terrainData.size.x;
        var terrainLength = terrainData.size.z;
        var terrainHeight = terrainData.size.y;

        var resolution = terrainData.heightmapResolution;

        if (targetHeightArray == null || targetHeightArray.GetLength(0) != resolution)
        {
            targetHeightArray = new float[resolution, resolution];
        }

        if (currentHeightArray == null || currentHeightArray.GetLength(0) != resolution)
        {
            currentHeightArray = new float[resolution, resolution];
        }

        if (prevHeightArray == null || prevHeightArray.GetLength(0) != resolution)
        {
            prevHeightArray = new float[resolution, resolution];
        }

        GenerateNewHeights(ref targetHeightArray, resolution, terrainWidth, terrainLength, terrainHeight, amplitude, frequency);

        for (var i = 0; i < currentHeightArray.GetLength(0); i++)
        {
            for (var j = 0; j < currentHeightArray.GetLength(1); j++)
            {
                prevHeightArray[i, j] = currentHeightArray[i, j];
            }
        }

        lastGenTime = Time.time;
    }

    private void GenerateNewHeights(ref float[,] heightArray, int resolution, float width, float length, float height, float amplitude, float frequency)
    {
        var baseNoise = new FastNoiseLite(DateTime.Now.Millisecond);
        baseNoise.SetNoiseType(FastNoiseLite.NoiseType.Perlin);

        var canyonNoise = new FastNoiseLite(DateTime.Now.Millisecond);
        canyonNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        canyonNoise.SetFrequency(0.029f);
        canyonNoise.SetFractalType(FastNoiseLite.FractalType.Ridged);
        canyonNoise.SetFractalOctaves(2);
        canyonNoise.SetFractalGain(23);
        canyonNoise.SetFractalWeightedStrength(0.14f);
        canyonNoise.SetFractalLacunarity(0.2f);

        var canyonBiomeNoise = new FastNoiseLite(DateTime.Now.Millisecond + 1);
        canyonBiomeNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        canyonBiomeNoise.SetFrequency(0.001f);

        for (var i = 0; i < resolution; i++)
        {
            for (var j = 0; j < resolution; j++)
            {
                var x = j * width * frequency / resolution;
                var y = i * length * frequency / resolution;

                var baseNoiseVal = (baseNoise.GetNoise(x, y) + 1) * 0.5f;
                var canyonNoiseVal = (canyonNoise.GetNoise(x, y) + 1) * 0.5f;
                var canyonBiomeNoiseVal = Mathf.Clamp01((canyonBiomeNoise.GetNoise(x, y) * 2 + 1) * 0.5f);
                canyonNoiseVal = canyonNoiseVal > canyonThreshold ? 1 : 0f;

                heightArray[i, j] = (baseNoiseVal * amplitude - canyonNoiseVal * canyonBiomeNoiseVal * canyonAmplitude + 5) / height;
            }
        }
    }
}