Shader "Camera/pixelate"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_height ("y-Pixel", int) = 1
		_length ("x-Pixel", int) = 1
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
            int _height;
            int _length;

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv.x = i.uv.x - i.uv.x % (1./_length) + 1./_length/2;
                i.uv.y = i.uv.y - i.uv.y % (1./_height) + 1./_height/2;
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}
