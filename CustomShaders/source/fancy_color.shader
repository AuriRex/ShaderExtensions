Shader "Camera/fancy_color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            half4 _MainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = UnityStereoScreenSpaceUVAdjust(v.uv,_MainTex_ST);
                return o;
            }

            sampler2D _MainTex;
            float _strength;
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uvR = float2(i.uv.x+_strength*0.05,i.uv.y);
                float2 uvG = i.uv;
                float2 uvB = float2(i.uv.x-_strength*0.05,i.uv.y);
                fixed4 R = tex2D(_MainTex, uvR);
                fixed4 G = tex2D(_MainTex, uvG);
                fixed4 B = tex2D(_MainTex, uvB);

                fixed4 col = fixed4(R.r,G.g,B.b,(R.a+G.a+B.a)/3);
                return col;
            }
            ENDCG
        }
    }
}