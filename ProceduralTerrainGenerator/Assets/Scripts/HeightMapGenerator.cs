using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public static class HeightMapGenerator 
//{
//    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
//    {
//        //EPISODE 19 around minute 11, SEBASTIAN EXPLAINS THAT THE FALLOFF MAP WILL BE IMPLEMENTED HERE BUT AT A LATER DATE
//        // I HAVE A FEELING THAT NEVER HAPPENS

//        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

//        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

//        float minValue = float.MaxValue;
//        float maxValue = float.MinValue;

//        for (int i = 0; i < width; i++)
//        {
//            for (int j = 0; j < height; j++)
//            {
//                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

//                if (values[i, j] > maxValue)
//                {
//                    maxValue = values[i, j];
//                }
//                if (values[i, j] < minValue)
//                {
//                    minValue = values[i, j];
//                }
//            }
//        }
//        return new HeightMap(values, minValue, maxValue);
//    }
//}


//CHUCK THIS IN AFTER FINAL EP IF I WANT TO USE FALLOFF:

public static class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCentre)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCentre);

        if (settings.useFalloff)
        {
            // Generate the falloff map using falloffStart and falloffEnd
            float[,] falloffMap = FallOffGenerator.Generate(new Vector2Int(width, height), settings.falloffStart, settings.falloffEnd);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    values[i, j] = Mathf.Clamp01(values[i, j] - falloffMap[i, j]);
                }
            }
        }

        AnimationCurve heightCurve_threadsafe = new AnimationCurve(settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }
                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }
            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
    {
        public readonly float[,] values;
        public readonly float minValue;
        public readonly float maxValue;

        public HeightMap(float[,] values, float minValue, float maxValue)
        {
            this.values = values;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }
    }
