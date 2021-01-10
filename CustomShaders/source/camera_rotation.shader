    Shader "Camera/Camera_Rotation" {
        Properties {
            _MainTex ("Texture", 2D) = "white" {}
            _RotationValue ("Rotation Value", Float) = 0.0
        }

        // #####
        // https://forum.unity.com/threads/rotation-of-texture-uvs-directly-from-a-shader.150482/
        // #####

        SubShader {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always
           
            Pass {
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

                sampler2D _MainTex;
     
                float _RotationValue;
                half4 _MainTex_ST;

                v2f vert (appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;

                    o.uv.xy -= 0.5;

                    float rotation = _RotationValue * UNITY_PI * 2;

                    float s = sin ( rotation );
                    float c = cos ( rotation );
                    float2x2 rotationMatrix = float2x2( c, -s, s, c);
                    
                    rotationMatrix *= 0.5;
                    rotationMatrix += 0.5;

                    rotationMatrix = rotationMatrix * 2-1;

                    // Rotate UVs but shift the Pivot point .1 to the right for the Left Eye and .1 to the left for the Right Eye
                    o.uv.xy = mul (o.uv.xy - float2(-.1, 0) * (unity_StereoEyeIndex * 2 - 1) , rotationMatrix) + float2(-.1, 0) * (unity_StereoEyeIndex * 2 - 1);

                    o.uv.xy += 0.5;

                    return o;
                }

                fixed4 frag (v2f i) : SV_Target
                {
                    return tex2D (_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
                }
                ENDCG
			}
            
        }
    }