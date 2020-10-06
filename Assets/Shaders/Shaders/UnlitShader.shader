Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _WarpingTex("Warping Texture", 2D) = "white" {}
        _WarpingValue("Warping Value", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
            sampler2D _WarpingTex;
            float4 _MainTex_ST;
            float _WarpingValue;

            v2f vert (appdata v)
            {
                v2f o;
                float4 warp = tex2Dlod(_WarpingTex, float4(v.uv, 0, 0));
                float4 newV = v.vertex;
                newV.y = _WarpingValue * warp.x - _WarpingValue * 0.5;
                o.vertex = UnityObjectToClipPos(newV);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
