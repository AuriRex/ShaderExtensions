Shader "Camera/Basic_Contrast"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_contrast ("Contrast", float) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

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

            half4 _MainTex_ST;
            half4 _PrevMainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _PrevMainTex;
            float _contrast;

            half3 AdjustContrastCurve(half3 color, half contrast) {
                return pow(abs(color * 2 - 1), 1 / max(contrast, 0.0001)) * sign(color - 0.5) + 0.5;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));

                half3 ccol = AdjustContrastCurve(half3(col.r, col.g, col.b), _contrast);

                return fixed4(ccol.r, ccol.g, ccol.b, col.a);
            }
            ENDCG
        }
    }
}
