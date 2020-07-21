Shader "Holistic/PrimitiveSphere3"
{
    Properties
    {
        _Color ("Color",Color) = (1,1,1,1)
        _Radius ("Radius", Range(0.1,1.5)) = 1
        _Radius2 ("Radius", Range(0.1,1.5)) = 1
        _Type ("Type", Int) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float3 wPos : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o = (v2f)0;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.wPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                
                return o;
            }

            #define STEPS 256
            #define STEP_SIZE 0.01

            fixed3 _Color;

            int _Type;
            float _Radius;
            float _Radius2;

            bool BoxHit (float3 p, float3 center,float3 size)
            {
                return  (p.x > center.x - size.x /2) && (p.x < center.x + size.x /2) &&
                        (p.y > center.y - size.y /2) && (p.y < center.y + size.y /2) &&
                        (p.z > center.z - size.z /2) && (p.z < center.z + size.z /2);
            }
            bool SphereHit (float3 p, float3 center, float radius)
            {
                return distance(p,center) < radius;
            }
            bool TorusHit (float3 p, float3 center, float radius, float radiusTor)
            {
                float3 torDir = normalize(p - center) * radius;
                float3 circleCenter = center + float3(torDir.x, 0 ,torDir.z);

                return distance(p,circleCenter) < radiusTor;
            }

            float3 RaymarchingHit (float3 position, float3 dirction)
            {
                
                for (int i = 0 ; i < STEPS; i ++)
                {
                    if(_Type == 1)
                    {
                        if(BoxHit(position, float3(0,0,0), _Radius)) return(position);
                    }
                    else if (_Type == 2)
                    {
                        if(SphereHit(position, float3(0,0,0), _Radius)) return (position) ;
                    }
                    else if (_Type == 3)
                    {
                        if(TorusHit(position, float3(0,0,0), _Radius , _Radius2)) return (position) ;
                    }
                    position += dirction * STEP_SIZE;
                }
                
                return float3(0,0,0);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float3 viewDirction = normalize(i.wPos - _WorldSpaceCameraPos);
                float3 worldPosition = i.wPos;
                float3 depth = RaymarchingHit(worldPosition,viewDirction);

                float3 wNormal = depth - float3(0,0,0);
                half nl = max(0,dot(wNormal,_WorldSpaceLightPos0.xyz));
                fixed3 diff = _Color.rgb * nl * _LightColor0;

                if(length(depth) != 0)
                {                     
                    return fixed4(diff,1) ;
                }
                else return fixed4 (1,1,1,0);
            }
            ENDCG
        }
    }
}
