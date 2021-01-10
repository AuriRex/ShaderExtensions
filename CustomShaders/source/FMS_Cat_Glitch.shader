Shader "Camera/FMS_Cat_Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _randomTex ("RandomNoise", 2D) = "white" {}
        _amp ("ShiftX", float) = 0
        _amp2 ("ShiftY", float) = 0
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

            #define saturate(i) clamp(i,0.,1.)
            #define lofi(i,j) (floor((i)/(j))*(j))

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

            sampler2D _MainTex;
            float _ShiftX;
            float _ShiftY;
            float _Amount;

            float rand( float2 co )
            {
                return frac(sin(dot(co.xy,float2(12.9898,78.233))) * 43758.5453);
            }

            half4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = UnityStereoScreenSpaceUVAdjust(v.uv, _MainTex_ST);
                return o;
            }
            float _amp;
            float _amp2;
            sampler2D _randomTex;

            bool validuv( float2 v ) 
            { 
                return 0.0 < v.x && v.x < 1.0 && 0.0 < v.y && v.y < 1.0; 
            }
            float4 randomV4( float i ) {
                return tex2D( _randomTex, frac( i / float2( 7.38, 9.24 ) ) );
            }

            fixed4 frag (v2f i) : SV_Target
            {  
                float2 uv = i.uv;

                float4 tex = 1;
                for ( int i = 0; i < 3; i ++ ) {
                    float2 uvn = uv;
                    float y = lofi( uvn.y, 1.0 / 80.0 );
                    float dice = randomV4( y ).x;
                    uvn.x -= _amp * sin( 100.0 * dice + 0.8 * float( i ) + 4.0 * randomV4( _Time[1] ).x );
                    tex[ i ] = tex2D( _MainTex,  saturate( uvn )  )[ i ];
                    if ( validuv( uvn ) ) {
                    uvn.y = lerp( uvn.y, sin( 10.0 * tex[ i ] ), _amp2 * ( 1.0 - tex[ i ] ) );
                    tex[ i ] = tex2D( _MainTex,  saturate( uvn )  )[ i ];
                    }
                }

                return float4( tex.xyz, 1.0 );
            }
            ENDCG
        }
    }
}
