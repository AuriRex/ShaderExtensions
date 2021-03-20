Shader "Camera/Freeze_Frame"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrevMainTex ("Prev Texture", 2D) = "white" {}
		_freeze ("Freeze", Range(0,1)) = 1
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
            half4 _PrevMainTex_ST;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _PrevMainTex;
            float _freeze;
            
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));

                fixed4 fFrame = tex2D(_PrevMainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
                col.rgb = _freeze * col.rgb + (1 - _freeze)*(fFrame.rgb);
                return col;
            }
            ENDCG
        }
    }
}
