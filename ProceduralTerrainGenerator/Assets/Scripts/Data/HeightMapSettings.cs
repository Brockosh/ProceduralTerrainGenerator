using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdateableData
//{
//    public NoiseSettings noiseSettings;

//    public bool useFalloff;

//    public float heightMultiplier;
//    public AnimationCurve heightCurve;

//    public float minHeight
//    {
//        get
//        {
//            return heightMultiplier * heightCurve.Evaluate(0);
//        }
//    }

//    public float maxHeight
//    {
//        get
//        {
//            return heightMultiplier * heightCurve.Evaluate(1);
//        }
//    }

{
    public NoiseSettings noiseSettings;

    public bool useFalloff;

    [Range(0f, 1f)]
    public float falloffStart = 0.3f; // Controls how near the falloff starts
    [Range(0f, 1f)]
    public float falloffEnd = 0.6f; // Controls how far the falloff ends

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR

    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();
    }
#endif
}
