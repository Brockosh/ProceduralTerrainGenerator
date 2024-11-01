Shader "Custom/Terrain"
{
    Properties
    {
        _minHeight ("minHeight", Float) = 0.0
        _maxHeight ("maxHeight", Float) = 100.0
        testTexture("Texture", 2D) = "white"{}
        testScale("Scale", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM

//COMMENTING THIS LINE OUT DOES ALLOW FOR COLOUR BUT STILL DOESNT WORK AS INTENDED
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
//#pragma exclude_renderers d3d11 gles
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int maxLayerCount = 8;
        const static float epsilon = 1E-4;

        int layerCount;
        float3 baseColours[maxLayerCount];
        float baseStartHeights[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColourStrength[maxLayerCount];
        float baseTextureScales[maxLayerCount];

        float _minHeight;
        float _maxHeight;

        sampler2D testTexture;
        float testScale;

        UNITY_DECLARE_TEX2DARRAY(baseTextures);

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)


        float inverseLerp(float a, float b, float value)
        {
            return saturate((value-a) / (b-a));
        }

        float3 triplanar(float3 worldPos, float scale, float3 blendAxes, int textureIndex)
        {
            float3 scaledWorldPos = worldPos / scale;

            float3 xProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxes.x;
            float3 yProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxes.y;
            float3 zProjection = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxes.z;
            return xProjection + yProjection + zProjection;
        }


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(_minHeight, _maxHeight, IN.worldPos.y);
            float3 blendAxes = abs(IN.worldNormal);
            blendAxes /= blendAxes.x + blendAxes.y + blendAxes.z;
            for (int i = 0; i < layerCount; i++)
            {
                float drawStrength = inverseLerp(-baseBlends[i]/2 - epsilon, baseBlends[i]/2, heightPercent - baseStartHeights[i]);

                float3 baseColour = baseColours[i] * baseColourStrength[i];
                float3 textureColour = triplanar(IN.worldPos, baseTextureScales[i], blendAxes, i) * (1 - baseColourStrength[i]);

                o.Albedo = o.Albedo * (1-drawStrength) + (baseColour + textureColour) * drawStrength;
            }


        }

        // void surf (Input IN, inout SurfaceOutputStandard o)
        // {
        //     float heightPercent = inverseLerp(_minHeight, _maxHeight, IN.worldPos.y);

        //     Need to handle the first colour outside the loop (because we compare against minHeight)
        //     float drawStrength = inverseLerp(_minHeight, baseStartHeights[0], heightPercent);
        //     o.Albedo = o.Albedo * (1-drawStrength) + baseColours[0] * drawStrength;
            
        //     for (int i = 1; i < baseColourCount; i++)
        //     {
        //         float drawStrength = inverseLerp(baseStartHeights[i-1], baseStartHeights[i], heightPercent);
        //         o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
        //     }
        // }

        ENDCG
    }
    FallBack "Diffuse"
}
