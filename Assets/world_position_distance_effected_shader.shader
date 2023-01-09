Shader "Unlit/world_position_distance_effected_shader"
{
    Properties
    {
        _MainTex ("Sprite", 2D) = "white" {}
        _AirColor ("Air Color", Color) = (0, 0, 0, 1)
        _DistEffectLevel ("Dist Effect Level", Float) = 1
    }
    SubShader
    {
        Tags { 
            "RenderType"="Transparent"
			"Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha Cull Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD3;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _AirColor;
            float _DistEffectLevel;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 tex = tex2D(_MainTex, i.uv);
                fixed4 col = fixed4(lerp(tex.rgb, _AirColor.rgb, abs(i.worldPos.z) * _DistEffectLevel),tex.a);

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
