using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[CreateAssetMenu()]
public class TextureData : UpdateableData
{
    public Color[] baseColours;
    [Range(0, 1)]
    public float[] baseStartHeights;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("baseColourCount", baseColours.Length);
        material.SetColorArray("baseColours", baseColours);
        material.SetFloatArray("baseStartHeights", baseStartHeights);

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        if (minHeight == savedMinHeight && maxHeight == savedMaxHeight)
            return; // No height change

        // Will throw an error if not running on the main thread
        // We can catch this error & handle it ourselves
        try
        {
            savedMaxHeight = maxHeight;
            savedMinHeight = minHeight;
            material.SetFloat("minHeight", minHeight);
            material.SetFloat("maxHeight", maxHeight);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not update mesh heights: {e.Message}", this);
        }
    }

}
