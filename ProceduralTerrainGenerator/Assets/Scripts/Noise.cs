using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapHeight; y++) 
        { 
            for (int x = 0; x < mapWidth; x++) 
            {
                float sampleX = x / scale;
                float sampleY = y / scale;

                float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
                noiseMap[x, y] = perlinValue;
            }
        }
        return noiseMap;
    }


    public static float[,] CompleteGenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        // We have scale division below so this is to make sure we don't get a division by 0 error
        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float[,] noiseMap = new float[mapWidth, mapHeight];

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;


        // Loop through map height values
        for (int y = 0; y < mapHeight; y++)
        {
            // For each height value, loop through all the width values
            for (int x = 0; x < mapWidth; x++)
            {
                // Refers to y axis
                float amplitude = 1;
                // Refers to X axis
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    // Divide x and y by scale to get some non integer values as perlin noise repeats at whole numbers
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x * frequency;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight < minValue)
                {
                    minValue = noiseHeight;
                }
                if (noiseHeight > maxValue)
                {
                    maxValue = noiseHeight;
                }

                noiseMap[x, y] = noiseHeight;
            }
        }

        // Normalize Values
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minValue, maxValue, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
