using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public enum NormaliseMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, NormaliseMode normalizeMode)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //prng = pseudo random number generator
        //Using this to track different maps
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        float maxPossibleHeight = 0;
        //Think y axis for amplitude
        float amplitude = 1;
        //Think x axis for frequency
        float frequency = 1;


        for (int i = 0; i < octaves; i++)
        {
            //Don't want to give the perlin noise a value too high so we keep within a range
            float offsetX = prng.Next(-100000, 1000000) + offset.x;
            float offsetY = prng.Next(-100000, 1000000) - offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight += amplitude;
            amplitude *= persistance;
        }

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxLocalNoiseHeight = float.MinValue;
        float minLocalNoiseHeight = float.MaxValue;

        //Allow ourselves to zoom into the center instead of top right in editor
        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight/ 2f;



        for (int y = 0; y < mapHeight; y++) 
        { 
            for (int x = 0; x < mapWidth; x++) 
            {
                amplitude = 1;
                frequency = 1;
                //Want to keep track of our current height value
                //This is so when we add octaves, the new value doesn't just reset the height but
                //builds off the current height
                float noiseHeight = 0;

                //Loop through octaves
                for (int i = 0; i < octaves; i++)
                {
                    // Divide x and y by scale to get some non integer values as perlin noise repeats at whole numbers
                    //The higher the frequency, the further apart the sample points will be
                    //Which will mean that the height values will change more rapidly
                    float sampleX = (x-halfWidth + octaveOffsets[i].x) / scale * frequency;
                    float sampleY = (y-halfHeight + octaveOffsets[i].y) / scale * frequency;

                    //* 2 - 1 gives us perlin values that are occasionally in the negative to give us more variation in our land
                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseMap[x, y] = perlinValue;

                    noiseHeight += perlinValue * amplitude;

                    //At the end of each octave
                    //Persistance is 0 - 1 range so it decreases the amplitude
                    amplitude *= persistance;

                    //Frequency increases each octave as the lacunarity should be greater than one
                    frequency *= lacunarity;
                
                }

                if (noiseHeight > maxLocalNoiseHeight)
                {
                    maxLocalNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minLocalNoiseHeight)
                {
                    minLocalNoiseHeight = noiseHeight;
                }

                //Apply the noiseHeight
                noiseMap[x, y] = noiseHeight;
            }
        }


        //There is a preferred way to do this if not continually generating map. Sebastian Lague E10: 4:35
        //Normalise noise map
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (normalizeMode == NormaliseMode.Local)
                {
                    // Inverse lerp gives us a value between 0 and 1, so if the value is above or below zero
                    // it will snap them back to 1 or 0
                    noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                }
                else
                {
                    float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight); ;
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
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
