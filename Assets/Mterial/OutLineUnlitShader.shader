Shader "Custom/Wireframe"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,0,1)
        _Thickness ("Thickness", Range(0.5, 3)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            Cull Off
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Thickness;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.normal = normalize(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float edge = abs(i.normal.x) + abs(i.normal.y) + abs(i.normal.z);
                edge = saturate(edge * _Thickness);
                return _Color * edge;
            }
            ENDCG
        }
    }
}
