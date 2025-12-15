// UnlitInstancedTransparent.shader
Shader "Custom/UnlitInstancedTransparent"
{
    Properties
    {
        _MainTex ("_MainTex", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.1
        _UVRect ("UV Rectangle", Vector) = (0,0,1,1)
        [Toggle]_UVClamp ("Clamp UV", Float) = 1
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Transparent" 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
        }
        LOD 100
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma multi_compile _ _ALPHATEST_ON
            #pragma multi_compile _ _UVCLAMP_ON
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _UVRect;
            float _Cutoff;
            
            UNITY_INSTANCING_BUFFER_START(Props)
                UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
            UNITY_INSTANCING_BUFFER_END(Props)

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(i);
                
                float2 remappedUV = i.uv;
                
                float2 uvSize = float2(_UVRect.z - _UVRect.x, _UVRect.w - _UVRect.y);
                float2 uvOffset = float2(_UVRect.x, _UVRect.y);
                
                remappedUV = i.uv * uvSize + uvOffset;
                
                #if defined(_UVCLAMP_ON)
                    remappedUV = clamp(remappedUV, float2(_UVRect.x, _UVRect.y), float2(_UVRect.z, _UVRect.w));
                #endif
                
                fixed4 instanceColor = UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
                fixed4 texColor = tex2D(_MainTex, remappedUV);
                fixed4 finalColor = texColor * instanceColor;
                
                #if defined(_ALPHATEST_ON)
                    clip(finalColor.a - _Cutoff);
                #endif
                
                UNITY_APPLY_FOG(i.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
}