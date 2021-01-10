Shader "Camera/circle_gray"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_speed ("Circle Speed", float) = 1
		_size ("Circle Size", float) = 1
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
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _size;
            float _speed;

            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvMid = i.uv - 0.5;
                float dist = distance(uvMid,(0,0));
                float timing = ( sign( sin(_Time[1] * _speed+dist * _size)) + 1)/2.;

                fixed4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv,_MainTex_ST));

                float3 gray = (col.r + col.g + col.b) / 3;
                col.rgb = timing * col.rgb + (1-timing)*(gray);
                return col;
            }
            ENDCG
        }
    }
}
