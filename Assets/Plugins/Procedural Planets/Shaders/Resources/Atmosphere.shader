/*
	Shader: ProceduralPlanets/Atmosphere
	Version: 0.1.1 (alpha release)
	Date: 2018-01-10
	Author: Stefan Persson
	(C) Imphenzia AB
*/

Shader "ProceduralPlanets/Atmosphere"
{
    Properties
    {
		_ColorAtmosphere("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
        _Size("Size", Float) = 0.1
        _Falloff("Falloff", Float) = 5
        _Transparency("Transparency", Float) = 15       
        _LocalStarPosition ("Local Star Position", Vector) = (0,0,0)
		_LocalStarColor("_LocalStarColor", Color) = (1.0,1.0,1.0,1.0)
		_LocalStarIntensity("_LocalStarIntensity", Range(0.01, 20.0)) = 1.0
    }
   
	SubShader
    {  
		Tags {
            	"LightMode" = "ForwardBase"            	
	    		"Queue" = "Transparent+1"
        		"RenderType" = "Transparent"
		}
 		Pass {
            Name "AtmosphereBase"

            Cull Front
			Blend One One

            CGPROGRAM
				#pragma exclude_renderers gles
                #pragma vertex vert
                #pragma fragment frag
               
                #pragma fragmentoption ARB_fog_exp2
                #pragma fragmentoption ARB_precision_hint_fastest
               
                #include "UnityCG.cginc"
               
                uniform float4 _ColorAtmosphere;
                uniform float _Size;
                uniform float _Falloff;
                uniform float _Transparency;
                uniform float3 _LocalStarPosition;
				uniform float4 _LocalStarColor;
				uniform float _LocalStarIntensity;

                struct v2f {
                    float4 pos : SV_POSITION;
                    float3 normal : TEXCOORD0;
                    float3 worldvertpos : TEXCOORD1;
                };
                
                struct appd {
            		float4 vertex : POSITION;
            		float3 normal : NORMAL;
            		float4 tangent : TANGENT;                	
                };
                
                v2f vert(appd v) {
                    v2f o;
                   
                    v.vertex.xyz += v.normal*_Size;
                    o.pos = UnityObjectToClipPos (v.vertex);
                    o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
                    o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;    
                    return o;
                }
              
                float4 frag(v2f i) : COLOR {
					if (length(i.normal) > 1.0)
					return float4 (0,0,0,0);

					i.normal = normalize(i.normal);
                    float3 viewdir = normalize(i.worldvertpos-_WorldSpaceCameraPos);
                    float4 color = _ColorAtmosphere;
					float f = pow(saturate(dot(viewdir, i.normal)), _Falloff);
					f = pow(f, 2.6 - (f * 18));
					float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
					f *= 1.2 * _Transparency*dot(normalize(_LocalStarPosition.xyz - objectOrigin), i.normal);
					color.rgb = saturate(color.rgb * f * 2);
					color.rgb *= min(f,1);
					return float4((color * _LocalStarIntensity * _LocalStarColor).rgb, f);
                }
            ENDCG
        }
    }
}