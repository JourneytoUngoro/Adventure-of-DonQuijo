Shader "Custom/SpriteGaussianBlur_SoftAlpha_Smoothstep"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurSize ("Blur Radius", Range(0.0, 10.0)) = 2.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

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
                float2 uv = i.uv;
                float blur = _BlurSize;

                float3 col = 0;
                float alpha = 0;
                float totalWeight = 0;

                for (int x = -4; x <= 4; x++)
                {
                    for (int y = -4; y <= 4; y++)
                    {
                        float2 offset = float2(x, y) * _MainTex_TexelSize.xy * blur;
                        float weight = exp(-(x * x + y * y) / (2.0 * blur * blur));

                        fixed4 sample = tex2D(_MainTex, uv + offset);
                        col += sample.rgb * weight;
                        alpha += sample.a * weight;
                        totalWeight += weight;
                    }
                }

                col /= totalWeight;
                alpha /= totalWeight;

                float softAlpha = smoothstep(0.0, 1.0, alpha);

                return fixed4(col, softAlpha);
            }
            ENDCG
        }
    }
}
