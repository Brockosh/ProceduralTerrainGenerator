using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FallOffGenerator 
{
    public static float[,] GenerateFallOffMap(int size)
    {
        float[,] map = new float[size,size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }
        return map;
    }


    public static float[,] Generate(Vector2Int size, float falloffStart, float falloffEnd)
    {
        float[,] heightMap = new float[size.x, size.y];

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                // Calculate position normalized between -1 and 1
                Vector2 position = new Vector2(
                    (float)x / size.x * 2 - 1,
                    (float)y / size.y * 2 - 1
                );

                // Find which edge is closer
                float t = Mathf.Max(Mathf.Abs(position.x), Mathf.Abs(position.y));

                // Work from the edges inward (reverse the falloff effect)
                if (t > falloffEnd)
                {
                    heightMap[x, y] = 1;
                }
                else if (t < falloffStart)
                {
                    heightMap[x, y] = 0;
                }
                else
                {
                    heightMap[x, y] = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(falloffStart, falloffEnd, t));
                }
            }
        }

        return heightMap;
    }





    static float Evaluate(float value)
    {
        float a = 3f;
        float b = 2.2f;

        return Mathf.Pow(value,a) / (Mathf.Pow(value,a) + Mathf.Pow(b - b * value, a));
    }
}
