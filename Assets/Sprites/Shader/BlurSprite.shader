// Shader "Custom/BlurSprite"
// {
//     Properties
//     {
//         _MainTex ("Texture", 2D) = "white" {}
//         _BlurSize ("Blur Size", Float) = 1.0
//     }

//     SubShader
//     {
//         Tags { "RenderType"="Opaque" }
//         LOD 100

//         GrabPass { "_GrabTex" }

//         Pass
//         {
//             Name "GaussianBlur"
//             ZTest Always Cull Off ZWrite Off

//             CGPROGRAM
//             #pragma vertex vert
//             #pragma fragment frag
//             #include "UnityCG.cginc"

//             sampler2D _GrabTex;
//             float4 _GrabTex_TexelSize;
//             float _BlurSize;

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             struct v2f
//             {
//                 float4 pos : SV_POSITION;
//                 float2 uv : TEXCOORD0;
//             };

//             v2f vert (appdata v)
//             {
//                 v2f o;
//                 o.pos = UnityObjectToClipPos(v.vertex);
//                 o.uv = v.uv;
//                 return o;
//             }

//             fixed4 frag (v2f i) : SV_Target
//             {
//                 float2 uv = i.uv;
//                 float2 texelSize = _GrabTex_TexelSize.xy * _BlurSize;

//                 fixed4 color = fixed4(0,0,0,0);

//                 color += tex2D(_GrabTex, uv + texelSize * -2.0) * 0.05;
//                 color += tex2D(_GrabTex, uv + texelSize * -1.0) * 0.09;
//                 color += tex2D(_GrabTex, uv)                 * 0.62;
//                 color += tex2D(_GrabTex, uv + texelSize * 1.0) * 0.09;
//                 color += tex2D(_GrabTex, uv + texelSize * 2.0) * 0.05;

//                 return color;
//             }
//             ENDCG
//         }
//     }
// }
