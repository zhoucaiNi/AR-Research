Shader "Custom/RotatableTextureShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Rotation ("Rotation Angle (Degrees)", Float) = 0
        _XTiling ("X Tiling", Float) = 1.0
        _YTiling ("Y Tiling", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        float _Rotation;
        float _XTiling;
        float _YTiling;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Apply tiling before rotating
            float2 uv = IN.uv_MainTex;
            uv.x *= _XTiling;
            uv.y *= _YTiling;

            // Convert rotation from degrees to radians
            float rad = _Rotation * (3.14159265 / 180.0);

            // Rotate texture coordinates around the center
            float2 centeredUV = uv - 0.5;
            float cosA = cos(rad);
            float sinA = sin(rad);
            float2x2 rotMatrix = float2x2(cosA, -sinA, sinA, cosA);
            float2 rotatedUV = mul(rotMatrix, centeredUV) + 0.5;

            // Sample the texture with rotated and tiled UV coordinates
            fixed4 c = tex2D(_MainTex, rotatedUV);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
