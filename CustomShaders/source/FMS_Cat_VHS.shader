Shader "Camera/FMS_Cat_VHS"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _strength ("strength", float) = 1
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

            #define SMRES float2(_MainTex_TexelSize.z / 3, _MainTex_TexelSize.w / 3)
            #define PI 3.14159265

            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float _strength;
            // FUNCTIONS START

            float2 img2tex( float2 v ) {
                return v;
            }

            //float v2random (float2 uv)
            //{
            //    return frac(sin(dot(uv,float2(12.9898,78.233)))*43758.5453123);
            //}

            float v2random (float2 uv) {
                return tex2D(_NoiseTex, float2(1,1) % uv).x;     
			}

            float3 rgb2yiq( float3 rgb ) {
                return mul(float3x3( 0.299, 0.596, 0.211, 0.587, -0.274, -0.523, 0.114, -0.322, 0.312 ), rgb);
            }

            float3 yiq2rgb( float3 yiq ) {
                return mul(float3x3( 1.000, 1.000, 1.000, 0.956, -0.272, -1.106, 0.621, -0.647, 1.703 ), yiq);
            }

            float3 vhsTex2D( float2 uv ) {
                [flatten] if ((abs(uv.x-0.5)<0.5&&abs(uv.y-0.5)<0.5)) {
                    float3 col = float3( 0, 0, 0 );
                    for ( int i = 0; i < 4; i ++ ) {
                      float3 tex = tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust( uv - float( i ) * 1.1 / SMRES * float2(0,1) , _MainTex_ST) ).xyz;
                      col += lerp( float3(1,0,0), float3(0,1,1), float( i ) / 3.0 ) * rgb2yiq( tex ) / 2.0;
                    }
                    col = float3( 0.1, -0.1, 0.0 ) + float3( 0.7, 1.1, 1.9 ) * col;
                    return yiq2rgb( col );
			    }
                return float3(0.1,0.1,0.1);
            }

            // FUNCTIONS END

            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float2 uvn = uv;
                float3 col = float3( 0.0, 0.0, 0.0 );

                // tape wave
                uvn.x += ( v2random( float2( uvn.y / 10, _Time.y / 10 ) / 1.0 ) - 0.5 ) / SMRES.x * 2.0;
                uvn.x += ( v2random( float2( uvn.y, _Time.y * 10.0 ) ) - 0.5 ) / SMRES.x * 2.0;

                // tape crease
                float tcPhase = smoothstep( 0.9, 0.96, sin( uvn.y * 8.0 - _Time.y * PI * 1.2 ) );
                float tcNoise = smoothstep( 0.3, 1.0, v2random( float2( uvn.y * 4.77, _Time.y ) ) );
                float tc = tcPhase * tcNoise;
                uvn.x = uvn.x - tc / SMRES.x * 16.0;

                // switching noise V
                float snPhase = smoothstep( 0.01, 0.0, uvn.y );
                uvn.y += snPhase * 0.3;
                uvn.x += snPhase * ( ( v2random( float2( uv.y * 100.0, _Time.y * 10.0 ) ) - 0.5 ) / SMRES.x * 24.0 );

                // fetch
                col = vhsTex2D( uvn );

                // crease noise
                float cn = tcNoise * ( 0.3 + 0.7 * tcPhase );
                [flatten] if ( 0.29 < cn ) {
                    float2 uvt = ( uvn + float2(1,0) * v2random( float2( uvn.y, _Time.y ) ) ) * float2( 0.25, 1.0 );
                    float n0 = v2random( 0.25 * uvt );
                    float n1 = v2random( 0.25 * ( uvt + float2(1,0) / SMRES.x ) );
                    [flatten] if ( n1 < n0 ) {
                        col = lerp( col, 2.0 * float3(1,1,1), pow( n0, 12.0 ) );
                    }
                }

                // switching color modification
                col = lerp(
                col,
                col.gbr,
                snPhase * 0.4
                );

                // ac beat
                col *= 1.0 + 0.1 * smoothstep( 0.4, 0.6, v2random( float2( 0.0, 0.1 * ( uv.y + _Time.y * 0.2 ) ) / 10.0 ) );

                // color noise
                col *= 0.9 + 0.1 * tex2D( _NoiseTex,  float2(1,1) % ( uvn * float2( 1.0, 1.0 ) + _Time.y * float2( 2.97, 2.45 ))).rgb;
                col = clamp( col , 0, 1);

                return (_strength * float4(col.r, col.g, col.b, 1)) + (1-_strength) * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust( i.uv, _MainTex_ST ));
            }
            ENDCG
        }
    }
}
