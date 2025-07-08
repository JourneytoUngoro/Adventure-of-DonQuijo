Shader "Custom/VignetteSprite"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (0, 0, 0, 1)
        _Intensity ("Vignette Intensity", Range(0, 1)) = 0.8
        _Smoothness ("Vignette Smoothness", Range(0.01, 1)) = 0.4
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha // Src * SrcAlpha + Dst * (1 - SrcAlpha)
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;
            float _Intensity;
            float _Smoothness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, i.uv) * _Color;

                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center) / 0.7071;
                float vignette = smoothstep(_Intensity, _Intensity + _Smoothness, dist);

                baseColor.rgb = lerp(baseColor.rgb, _Color.rgb, vignette);
                baseColor.a *= (1.0 - vignette) * _Color.a;

                return baseColor;

            }
            ENDCG
        }
    }
}
