Shader "Camera/Basic_Dithering"{
    //show values to edit in inspector
    Properties{
        _MainTex ("Texture", 2D) = "white" {}
        _DitherPattern ("Dithering Pattern", 2D) = "white" {}
        _strength ("Strength", Range(0,1)) = 1
    }

    // ######
    // https://github.com/ronja-tutorials/ShaderTutorials
    // ######

    SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType"="Opaque" "Queue"="Geometry"}

        Pass{
            CGPROGRAM

            //include useful shader functions
            #include "UnityCG.cginc"

            //define vertex and fragment shader
            #pragma vertex vert
            #pragma fragment frag

            //texture and transforms of the texture
            sampler2D _MainTex;
            float4 _MainTex_ST;

            //The dithering pattern
            sampler2D _DitherPattern;
            float4 _DitherPattern_TexelSize;

            //Dither colors
            float4 _Color1;
            float4 _Color2;

            //the object data that's put into the vertex shader
            struct appdata{
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            //the data that's used to generate fragments and can be read by the fragment shader
            struct v2f{
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPosition : TEXCOORD1;
            };

            //the vertex shader
            v2f vert(appdata v){
                v2f o;
                //convert the vertex positions from object space to clip space so they can be rendered
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.screenPosition = ComputeScreenPos(o.position);
                return o;
            }

            float _strength;
            //the fragment shader
            fixed4 frag(v2f i) : SV_TARGET{
                //texture value the dithering is based on
                float4 texColor = tex2D(_MainTex, i.uv);
                float texColorR = texColor.r;
                float texColorG = texColor.g;
                float texColorB = texColor.b;// ??

                //value from the dither pattern
                float2 screenPos = i.screenPosition.xy / i.screenPosition.w;
                float2 ditherCoordinate = screenPos * _ScreenParams.xy * _DitherPattern_TexelSize.xy;
                float ditherValue = tex2D(_DitherPattern, ditherCoordinate).r;

                //combine dither pattern with texture value to get final result
                float ditheredValue = (step(ditherValue, texColorR) + step(ditherValue, texColorG) + step(ditherValue, texColorB)) / 3;
                float4 mainCol = tex2D(_MainTex, i.uv);
                float4 col = _strength * ditheredValue * mainCol + (1-_strength) * mainCol;
                return col;
            }

            ENDCG
        }
    }

    Fallback "Standard"
}