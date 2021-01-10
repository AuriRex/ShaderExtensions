Shader "Camera/FMS_Cat_Unclesam2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _phase ("phase", float) = 1

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
            float _phase;

            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            fixed4 frag (v2f i) : SV_Target
            {
                float3 col = float3(0,0,0);

                  for ( int j = 1; j <= 5; j ++ ) {
                    float fi = float( j );
                    float scale = 1.0 - 0.1 * exp( -4.0 * _phase ) - fi * 0.01;
                    float2 uv = ( i.uv - 0.5 ) * scale + 0.5;
                    col += pow( float3( 1.0, 0.98, 0.9 ), float3(1,1,1) * fi ) * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust(uv, _MainTex_ST) ).rgb;
                  }

                  col *= 0.2 * exp( -0.2 * _phase );

                  return float4( col.r, col.g, col.b, 1.0 );
            }
            ENDCG
        }
    }
}
