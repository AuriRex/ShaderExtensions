Shader "Camera/FMS_Cat_Weirdshit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _speed ("speed", float) = 1
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
                //o.vertex = ComputeNonStereoScreenPos(o.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            //float _amount;
            float2 _imageCord;
            float _speed;
            float _strength;

            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = float4(0,0,0,0);
                float2 uv = mul(float2(1,1),float2x2(cos( 0.01 ), sin( 0.01 ), -sin( 0.01 ), cos( 0.01 ))) * ( i.uv - 0.5 ) * 0.98 + 0.5;
                uv.x += 0.008 * sin( 20.0 * uv.y - _Time * 4.0 * _speed ) * _strength;
                uv.y += 0.008 * sin( 20.0 * uv.x - _Time * 4.0 * _speed ) * _strength;
                float4 fbCol = float4( 0.99, 0.95, 0.97 , 1);
                col += fbCol * tex2D( _MainTex, UnityStereoScreenSpaceUVAdjust( uv, _MainTex_ST ) ).rgba;
                return col;
            }
            ENDCG
        }
    }
}
