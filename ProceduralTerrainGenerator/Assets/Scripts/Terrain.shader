Shader "Custom/Terrain"
{
    Properties
    {
        _minHeight ("minHeight", Float) = 0.0
        _maxHeight ("maxHeight", Float) = 100.0
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

        const static int maxColorCount = 8;

        int baseColourCount;
        float3 baseColours[maxColorCount];
        float baseStartHeights[maxColorCount];

        float _minHeight;
        float _maxHeight;

        struct Input
        {
            float3 worldPos;
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

        // void surf (Input IN, inout SurfaceOutputStandard o)
        // {
        //     float heightPercent = inverseLerp(_minHeight, _maxHeight, IN.worldPos.y);
        //     for (int i = 0; i < baseColourCount; i++)
        //     {
        //         float drawStrength = saturate(sign(heightPercent - baseStartHeights[i]));
        //         o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
        //     }

        // }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float heightPercent = inverseLerp(_minHeight, _maxHeight, IN.worldPos.y);

            // Need to handle the first colour outside the loop (because we compare against minHeight)
            float drawStrength = inverseLerp(_minHeight, baseStartHeights[0], heightPercent);
            o.Albedo = o.Albedo * (1-drawStrength) + baseColours[0] * drawStrength;
            
            for (int i = 1; i < baseColourCount; i++)
            {
                float drawStrength = inverseLerp(baseStartHeights[i-1], baseStartHeights[i], heightPercent);
                o.Albedo = o.Albedo * (1-drawStrength) + baseColours[i] * drawStrength;
            }
        }

        ENDCG
    }
    FallBack "Diffuse"
}
