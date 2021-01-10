Shader "Camera/FMS_Cat_SpaceFB"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _alpha ("alpha", float) = 1

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _alpha;

            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = float3(0,0,0);

                float2 uv = ( i.uv - 0.5 ) * 0.99 + 0.5;
                uv.y += 4.0 / _MainTex_TexelSize.w;
                float3 fbCol = float3( 1.0, 0.0, 0.0 );
                col += fbCol * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST) ).rgb;
                 
                uv = ( i.uv - 0.5 ) * 0.98 + 0.5;
                uv.y += 7.0 / _MainTex_TexelSize.w;
                fbCol = float3( 0.0, 1.0, 0.0 );
                col += fbCol * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST) ).rgb;
                  
                uv = ( i.uv - 0.5 ) * 0.97 + 0.5;
                uv.y += 10.0 / _MainTex_TexelSize.w;
                fbCol = float3( 0.0, 0.0, 1.0 );
                col += fbCol * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST) ).rgb;

                col *= _alpha;
                return float4( col.r, col.g, col.b, 1.0 );
            }
            ENDCG
        }
    }
}
