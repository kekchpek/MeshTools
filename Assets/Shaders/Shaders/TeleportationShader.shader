Shader "Unlit/TeleportationShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}

        _Color("Color", Color) = (1,1,1,1)

        _MinMeshY("Min mesh Y", float) = -1
        _MaxMeshY("Max mesh Y", float) = -1
        _ClipBorderMax("Clip border max", Range(0, 1)) = 0
        _ClipBorderMin("Clip border min", Range(0, 1)) = 0

        _BorderRange("BorderRange", Range(0,1)) = 0.03

        _BorderColor("BorderColor", Color) = (1,1,1,1)
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
                float2 coords : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            float _MinMeshY;
            float _MaxMeshY;
            float _ClipBorderMax;
            float _ClipBorderMin;
            float _BorderRange;
            float4 _Color;
            float4 _BorderColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.coords = v.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float max = (1 + 2 * _BorderRange) * _ClipBorderMax - _BorderRange;
                float min = (1 + 2 * _BorderRange) * _ClipBorderMin - _BorderRange;
                clip((i.coords.y - _MinMeshY) - ((_MaxMeshY - _MinMeshY) * min));
                clip(((_MaxMeshY - _MinMeshY) * max) - (i.coords.y - _MinMeshY));
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                if ((i.coords.y - _MinMeshY) - ((_MaxMeshY - _MinMeshY) * min) < _BorderRange)
                    col = _BorderColor;
                if (((_MaxMeshY - _MinMeshY) * max) - (i.coords.y - _MinMeshY) < _BorderRange)
                    col = _BorderColor;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
