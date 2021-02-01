Shader "Camera/zoom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        // _strength > 0 = zoom out
        // _strength < 0 = zoom in
        // _strength <= -.5 flip the screen!
        _strength("Strength", float) = 0
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

            float _strength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * (2 * _strength + 1);
                o.uv -= 1 * _strength + float2(-.2 * (unity_StereoEyeIndex * 2 - 1), 0) * _strength;
                return o;
            }

            sampler2D _MainTex;

            half4 _MainTex_ST;
            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
            }
            ENDCG
        }
    }
}