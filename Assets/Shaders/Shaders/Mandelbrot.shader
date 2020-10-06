Shader "MyShaders/Mandelbrot"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _SetColor("Set Color", Color) = (1,1,1,1)
        
        _MinMeshY("Min mesh Y", float) = -1
        _MaxMeshY("Max mesh Y", float) = -1
        _MinMeshX("Min mesh X", float) = -1
        _MaxMeshX("Max mesh X", float) = -1
        _CenterX("Center X", float) = 0
        _CenterY("Center Y", float) = 0
        _Scale("Scale", float) = 1
        _Iterations("Iterations", int) = 100
        _ColorScale("Color Scale", float) = 1
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
            #pragma fragmentoption ARB_precision_hint_nicest

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 coords : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };
            
            fixed4 _Color;
            fixed4 _SetColor;
        
            float _MinMeshY;
            float _MaxMeshY;
            float _MinMeshX;
            float _MaxMeshX;
            float _CenterX;
            float _CenterY;
            float _Scale;
            uint _Iterations;
            float _ColorScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.coords = v.vertex;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float x = _CenterX + i.coords.x * _Scale;
                float y = _CenterY + i.coords.z * _Scale;
                fixed4 col = {0, 0, 0, 1};
                
                float z_i = 0;
                float z_r = 0;
                uint deep = 0;
                bool isFractal = 1;
                if (1 - _Scale)
                    _Scale = 1;
                for(uint i=0; i < _Iterations; i++) {
                    float new_z_i = 2.0 * z_i * z_r + y;
                    float new_z_r =  (z_r * z_r) - (z_i * z_i) + x;
                    z_i = new_z_i;
                    z_r = new_z_r;
                    float r2 = z_i * z_i + z_r * z_r;
                    if (r2 >= 4.0)
                    {
                        deep = i;
                        isFractal = 0;
                        break;
                    }
                }

                if (1 - isFractal)
                    col = _Color * deep / _Iterations * _ColorScale;

                if  (isFractal)
                    col = _SetColor;

                return col;
            }
            ENDCG
        }
    }
}
