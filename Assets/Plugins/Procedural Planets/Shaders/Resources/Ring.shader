/*
	Shader: ProceduralPlanets/Ring
	Version: 0.1.1 (alpha release)
	Date: 2018-01-10
	Author: Stefan Persson
	(C) Imphenzia AB	
*/

Shader "ProceduralPlanets/Ring"
{
    Properties
    {
		_MainTex ("Ring Maps (RGBA)" , 2D) = "black" {}
		_Transparency("Transparency", Float) = 15
		_LocalStarPosition("Local Star Position", Vector) = (0,0,0)
		_LocalStarRadius("Local Star Radius", Float) = 100
		_PlanetPosition("Planet Position", Vector) = (0,0,0)
		_PlanetRadius("Planet Radius", Float) = 1
		_Tint("Tint", COLOR) = (1,1,1,1)
	}

		SubShader
		{
			Tags {
					"LightMode" = "ForwardBase"
					"Queue" = "Transparent"
					"RenderType" = "Transparent"
			}
			Pass {
				Name "RingsFront"

				Cull Back
				Blend SrcAlpha OneMinusSrcAlpha
				
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile ALPHA1_OFF ALPHA1_ON
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
               

                #include "UnityCG.cginc"
               
				uniform sampler2D _MainTex;
                uniform float _Transparency;
                uniform float3 _LocalStarPosition;
				uniform float3 _LocalStarRadius;
				uniform float3 _PlanetPosition;
				uniform float _PlanetRadius;
				uniform float4 _Tint;

                struct v2f {
                    float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
					float4 posWorld : TEXCOORD2;
					float4 diff : COLOR0;
                };
                
                struct appd {
            		float4 vertex : POSITION;					
					float2 uv : TEXCOORD0;
					float2 uv2 : TEXCOORD1;
					float4 normal : NORMAL;
                };
                
                v2f vert(appd v) {					
                    v2f o;

					float4x4 modelMatrix = unity_ObjectToWorld;
					o.posWorld = mul(modelMatrix, v.vertex);

                    o.pos = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv2 = v.uv2;
					half3 worldNormal = UnityObjectToWorldNormal(v.normal);
					half n = max(0, saturate(dot(worldNormal, _LocalStarPosition.xyz)*0.1)+0.02);					
					o.diff = n;
                    return o;
                }
              
                float4 frag(v2f i) : COLOR {
					fixed4 color = tex2D(_MainTex, i.uv) * half4(i.diff.rgb, 1);

					float3 lightDirection;
					float lightDistance;
					float attenuation;

					lightDirection = _LocalStarPosition.xyz - i.posWorld.xyz;
					lightDistance = length(lightDirection);
					attenuation = 1.0 / lightDistance;
					lightDirection = lightDirection / lightDistance;
					
					float3 planetDirection = _PlanetPosition.xyz - i.posWorld.xyz;
					float planetDistance = length(planetDirection);
					planetDirection = planetDirection / planetDistance;
					float d = lightDistance * (asin(min(1.0, length(cross(lightDirection, planetDirection)))) - asin(min(1.0, _PlanetRadius / planetDistance)));
					float w = smoothstep(-1.0, 1.0, -d / 1);//_LocalStarRadius);
					w = w * smoothstep(0.0, 0.2, dot(lightDirection, planetDirection));					
					w = w * smoothstep(0.0, _PlanetRadius, lightDistance - planetDistance);
				
#if ALPHA1_ON
					return float4(color.rgb * (1 - w) * _Tint, pow(color.a, 0.34454545));
#endif
					return float4(color.rgb * (1 - w) * _Tint, color.a);
                }
            ENDCG
        }

			Pass{
					Name "RingsBack"

					Cull Front

					Blend SrcAlpha OneMinusSrcAlpha										
					CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					#include "UnityCG.cginc"
					#include "UnityLightingCommon.cginc"


					#include "UnityCG.cginc"

					uniform sampler2D _MainTex;
					uniform float _Transparency;
					uniform float3 _LocalStarPosition;
					uniform float3 _LocalStarRadius;
					uniform float3 _PlanetPosition;
					uniform float _PlanetRadius;
					uniform float4 _Tint;

					struct v2f {
						float4 pos : SV_POSITION;
						float2 uv : TEXCOORD0;
						float2 uv2 : TEXCOORD1;
						float4 posWorld : TEXCOORD2;
						float4 diff : COLOR0;
					};

					struct appd {
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
						float2 uv2 : TEXCOORD1;
						float4 normal : NORMAL;
					};

					v2f vert(appd v) {
						v2f o;

						float4x4 modelMatrix = unity_ObjectToWorld;
						o.posWorld = mul(modelMatrix, v.vertex);

						o.pos = UnityObjectToClipPos(v.vertex);
						o.uv = v.uv;
						o.uv2 = v.uv2;
						half3 worldNormal = -UnityObjectToWorldNormal(v.normal);
						half n = max(0, saturate(dot(worldNormal, _LocalStarPosition.xyz)*0.1) + 0.02);
						o.diff = n;
						return o;
					}

					float4 frag(v2f i) : COLOR {
						fixed4 color = tex2D(_MainTex, i.uv) * half4(i.diff.rgb, 1);

						float3 lightDirection;
						float lightDistance;
						float attenuation;

						lightDirection = _LocalStarPosition.xyz - i.posWorld.xyz;
						lightDistance = length(lightDirection);
						attenuation = 1.0 / lightDistance;
						lightDirection = lightDirection / lightDistance;

						float3 planetDirection = _PlanetPosition.xyz - i.posWorld.xyz;
						float planetDistance = length(planetDirection);
						planetDirection = planetDirection / planetDistance;
						float d = lightDistance * (asin(min(1.0, length(cross(lightDirection, planetDirection)))) - asin(min(1.0, _PlanetRadius / planetDistance)));
						float w = smoothstep(-1.0, 1.0, -d / 1);//_LocalStarRadius);
						w = w * smoothstep(0.0, 0.2, dot(lightDirection, planetDirection));
						w = w * smoothstep(0.0, _PlanetRadius, lightDistance - planetDistance);
#if ALPHA1_ON												
						return float4(color.rgb * (1 - w) * _Tint, pow(color.a, 0.34454545));
#endif												
						return float4(color.rgb * (1 - w) * _Tint, color.a);
					}
					ENDCG
				}
    }
   
    FallBack "Diffuse"
}