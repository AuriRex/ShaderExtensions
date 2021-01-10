Shader "Camera/Basic_Color"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _color ("Color", Color) = (1,1,1,1)
        _weight ("Weight", Range(0,1)) = 1
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
            float4 _color;
            float _weight;

            half4 _MainTex_ST;
            half4 _MainTex_TexelSize;
            fixed4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
                return _weight * col * _color + (1-_weight) * (col + _color);
            }
            ENDCG
        }
    }
}
