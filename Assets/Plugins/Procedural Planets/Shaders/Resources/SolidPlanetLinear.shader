// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

/*
	Shader: ProceduralPlanets/SolidPlanetLinear
*/


Shader "ProceduralPlanets/SolidPlanetLinear" {
    Properties
    {
    	// Textures
    	_TexMaps ("Planet Maps (RGBA)" , 2D) = "black" {}
    	_TexBiome1DiffSpec ("Biome 1 Diffuse + Specular (RGB+A)", 2D) = "black" {}
    	_TexBiome1Normal ("Biome 1 Normal (Bump)", 2D) = "bump" {}
    	_TexBiome2DiffSpec ("Biome 2 Diffuse + Specular (RGB+A)", 2D) = "black" {}
    	_TexBiome2Normal ("Biome 2 Normal (Bump)", 2D) = "bump" {}
		_TexIceDiffuse ("Ice Diffuse (RGB)", 2D) = "black" {}
		_TexClouds ("Clouds (RGB)", 2D) = "black" {}
    	_TexCities ("Cities Emmision (RGB)", 2D) = "black" {}
    	_TexLavaDiffuse ("Lava Diffuse (RGB)", 2D) = "black" {}   	
		_TexLavaFlow ("Lava Flow (RGB)", 2D) = "black" {}
    	
    	// Tiling    	
    	_TilingHeightBase ("Tiling Height Base", Int) = 2    	
    	_TilingHeightDetail ("Tiling Height Detail", Int) = 40
    	_TilingLavaBase ("Tiling Lava Base", Int) = 2
    	_TilingLavaDetail ("Tiling Lava Detail", Int) = 40
    	_TilingBiome ("Tiling Biome", Int) = 2
    	_TilingClouds ("Tiling Clouds", Int) = 1
    	_TilingCities ("Tiling Cities", Int) = 10
    	_TilingSurface ("Tiling Surface", Int) = 10
    	
    	// Details
    	_DetailHeight ("Detail Height", float) = 0.02
    	_DetailLava ("Detail Lava", float) = 0.02    	
    	
    	// Surface Properties
    	_SurfaceRoughness ("Surface Roughness", Range(0.5, 10.0)) = 1
		_LiquidOpacity ("Liquid Opacity", Range(0.0, 1.0)) = 1

    	// Colors    	    	    	    	
    	_ColorLiquid ("Liquid Color", Color) = (0.2, 0.4, 0.8, 1.0)
    	_ColorIce ("Ice Color", Color) = (1.0, 1.0, 1.0, 1.0)        
    	_ColorClouds ("Clouds Color" , Color) = (1.0, 1.0, 1.0, 1.0)
    	_ColorLavaGlow ("Lava Glow Color", Color) = (1.0, 0.4, 0, 1.0)
       	_ColorCities ("Cities Color", Color) = (1.0, 1.0, 0.8, 1.0)
		_ColorTwilight ("Twilight Color", Color) = (1.0,0.5,0.0,1.0)
       	_ColorAtmosphere ("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1.0)
       	_ColorSpecular ("Specular Color", Color) = (1.0,0.8,0.5, 1.0) 
       	
       	// Cloud Properties
        _CloudOpacity ("Cloud Opacity", float) = 1.0        
        _CloudHeight ("Cloud Height", Range(0.0, 10.0)) = 1
        _CloudSpeed ("Cloud Speed", Range(0.0,20.0)) = 0.1
        _CloudShadow ("Cloud Shadow Strength", Range(0.0, 1.0)) = 0.35

		// Lava Properties
		_LavaFlowSpeed ("Lava Flow Speed", float) = 0.2

		// Specular Properties
        _SpecularPowerLiquid("Specular Power Liquid", Range(1.0, 20.0)) = 10        
        _SpecularPowerSurface("Specular Power Surface", Range(1.0, 20.0)) = 10        
        
        // Atmosphere Properties
        _AtmosphereFalloff("Atmosphere Falloff", Range(1.0, 5.0)) = 5                
                	
		// Script Generated Textures
        _TexLookupLiquid ("Liquid Lookup (GENERATED)" , 2D) = "black" {}
    	_TexLookupPolar ("Polar Lookup (GENERATED)", 2D) = "black" {}
    	_TexLookupLava ("Lava Lookup (GENERATED)", 2D) = "black" {}
    	_TexLookupLavaGlow ("Glow Lookup (GENERATED)", 2D) = "black" {}

		// Script Updated Variables
		_LocalStarPosition("_LocalStarPosition", Vector) = (0,0,0,0)
		_LocalStarColor("_LocalStarColor", Color) = (1.0,1.0,1.0,1.0)
		_LocalStarIntensity("_LocalStarIntensity", Range(0.01, 20.0)) = 1.0
		_LocalStarAmbientIntensity("_LocalStarAmbientIntensity", Range(0.01, 20.0)) = 0.01
    }
 
	SubShader {		
        Pass {        	
        	Tags {"LightMode" = "Always"}
            Cull Back
 
            CGPROGRAM
            	#pragma target 3.0
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile CLOUDS_ON CLOUDS_OFF
                #pragma multi_compile LAVA_ON LAVA_OFF
                
                #include "UnityCG.cginc"
 
 				uniform sampler2D _TexMaps;
 				uniform sampler2D _TexBiome1DiffSpec;
 				uniform sampler2D _TexBiome1Normal;
 				uniform sampler2D _TexBiome2DiffSpec;
 				uniform sampler2D _TexBiome2Normal;
 				uniform sampler2D _TexIceDiffuse;
 				uniform sampler2D _TexClouds;
 				uniform sampler2D _TexCities;
 				uniform sampler2D _TexLavaDiffuse;
				uniform sampler2D _TexLavaFlow;
 				uniform int _TilingHeightBase;
 				uniform int _TilingHeightDetail;
 				uniform int _TilingLavaBase;
 				uniform int _TilingLavaDetail;
 				uniform int _TilingBiome;
 				uniform int _TilingClouds;
 				uniform int _TilingCities;
 				uniform int _TilingSurface;
				uniform float _DetailHeight;
				uniform float _DetailLava;
				uniform float _SurfaceRoughness;
				uniform float _LiquidOpacity;				
				uniform float4 _ColorLiquid;
				uniform float4 _ColorIce;
				uniform float4 _ColorClouds;
				uniform float4 _ColorLavaGlow;
				uniform float4 _ColorCities;
				uniform float4 _ColorTwilight;
				uniform float4 _ColorAtmosphere;
				uniform float4 _ColorSpecular;
				uniform float _CloudOpacity;
				uniform float _CloudHeight;
				uniform float _CloudSpeed;
				uniform float _CloudShadow;				
				uniform float _LavaFlowSpeed;
				uniform float _SpecularPowerLiquid;				
				uniform float _AtmosphereFalloff;		
				uniform sampler2D _TexLookupLiquid;
				uniform sampler2D _TexLookupPolar;
				uniform sampler2D _TexLookupLava;
				uniform sampler2D _TexLookupLavaGlow;				
				uniform float4 _LocalStarPosition;
				uniform float4 _LocalStarColor;
				uniform float _LocalStarIntensity;
				uniform float _LocalStarAmbientIntensity;
				
	 	        struct v2v {
	           		 float4 vertex : POSITION;
	           		 float4 texcoord  : TEXCOORD0;	            
	           		 float4 texcoord1 : TEXCOORD1;
	           		 float3 normal : NORMAL;
	           		 float4 tangent : TANGENT;
		        };
	
                struct v2f {
                    float4 pos : SV_POSITION;
                    float4 texcoord : TEXCOORD0;                    
                    float4 heightBody : TEXCOORD1;
                    float4 heightCap : TEXCOORD2;
                    float4 lavaBody : TEXCOORD3;
                    float4 lavaCap : TEXCOORD4;
                    float4 surface : TEXCOORD5;                    
                    float4 biome : TEXCOORD6;                                      
                    float3 normal : TEXCOORD7;
                    float3 worldvertpos : TEXCOORD8;
                    float3 tangentWorld : TEXCOORD9;
                    float3 normalWorld : TEXCOORD10;
                    float3 binormalWorld : TEXCOORD11;                
                    float3 tangentWorld2 : TEXCOORD12;
                    float3 binormalWorld2 : TEXCOORD13;                
                    float4 col : COLOR;
                };
   
                v2f vert(v2v v) {					
                    v2f o;       
					UNITY_INITIALIZE_OUTPUT(v2f, o);
 
 					// Calculate Vertex Position in Model View  Projection
                    o.pos = UnityObjectToClipPos (v.vertex);
                    
                    // Calculate Normal
                    o.normal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                    
                    // Calculate Vertex Position in World Space
                    o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                    
                    // Pack original UV1 and UV2 coordinates in texcoord xy and zw and pass to fragment program
                    o.texcoord.xy = v.texcoord.xy;
                    o.texcoord.zw = v.texcoord1.xy;
					
					// Calculate Height Body and Polar Cap coordinates in vertex program for optimization, pass to fragment program in float4 xy + zw
                    o.heightBody.xy = v.texcoord.xy * float2(_TilingHeightBase, _TilingHeightBase * 0.5);
                    o.heightBody.zw = v.texcoord.xy * float2(_TilingHeightDetail, _TilingHeightDetail * 0.5);
					o.heightCap.xy = v.texcoord1.xy * _TilingHeightBase * 0.5;
					o.heightCap.zw = v.texcoord1.xy * _TilingHeightDetail * 0.5;
					
					// Calculate Lava Body and Polar Cap coordinates in vertex program for optimization, pass to fragment program in float4 xy + zw
					#if LAVA_ON
					o.lavaBody.xy = v.texcoord.xy * float2(_TilingLavaBase, _TilingLavaBase * 0.5);
					o.lavaBody.zw = v.texcoord.xy * float2(_TilingLavaDetail, _TilingLavaDetail * 0.5);					
					o.lavaCap.xy = v.texcoord1.xy * _TilingLavaBase * 0.5;
					o.lavaCap.zw = v.texcoord1.xy * _TilingLavaDetail * 0.5;
					#endif

					// Calculate Biome Body and Polar Cap coordinates in vertex program for optimization, pass to fragment program in float4 xy + zw
					o.biome.xy = v.texcoord.xy * float2(_TilingBiome, _TilingBiome * 0.5);
					o.biome.zw = v.texcoord1.xy * _TilingBiome * 0.5;
	
					// Calculate Surface Body and Polar Cap coordinates in vertex program for optimization, pass to fragment program in float4 xy + zw
					o.surface.xy = v.texcoord.xy * float2(_TilingSurface, _TilingSurface * 0.5);
					o.surface.zw = v.texcoord1.xy * _TilingSurface * 0.5;

					// Calculate view direction for atmosphere calculation (can't pass this to fragment shader as specular reflection would be vertex based instead of pixel based)
					float3 viewDir = normalize(_WorldSpaceCameraPos-o.worldvertpos);					
					
					// Calculate internal atmosphere
					o.col.b = pow(1.0-saturate(dot(viewDir, o.normal)), _AtmosphereFalloff);
					
					// Calculate cap to body fading and store in col.a
					o.col.a = saturate(-3+abs(v.vertex.y));
										
					// Calculate tangent, normal and binormal used for normal mapping (bump)								
                    o.tangentWorld = normalize ( float3( mul( unity_ObjectToWorld, float4( float3( v.tangent.xyz), 0.0)).xyz));
                    o.normalWorld = normalize ( mul( float4( v.normal.xyz, 0.0), unity_WorldToObject).xyz );
                    o.binormalWorld = normalize ( cross( o.normalWorld, o.tangentWorld). xyz * v.tangent.w); 
                 
					o.tangentWorld2 = normalize ( float3( mul( unity_ObjectToWorld, float4( float3(-1,0,-1), 0.0)).xyz));
                    o.binormalWorld2 = normalize ( cross( o.normalWorld, o.tangentWorld2). xyz * -1); 

                    // Return the output struct
                    return o;
                }
 
 
                float4 frag(v2f i) : COLOR {          

					// Get the object origin - used to calculate light direction
					float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0,0.0,0.0,1.0));

                    // Calculate light direction by normalizing the local star position
                    float3 lightDirection  = normalize(_LocalStarPosition.xyz - objectOrigin);
  					
 					// Define value for flat normal map surface - used to crossfade normal map for water and cloud areas
					float4 normalFlat = float4(0.5, 0.5,1,1);
 					
					// LIQUID MAP (stored in _TexMaps Red Channel (Edge Details also from Red Channel))
 					float blendHeightBody = tex2D(_TexMaps, i.heightBody.xy).r + tex2D(_TexMaps, i.heightBody.zw).r * _DetailHeight;
 					float blendHeightCap = tex2D(_TexMaps,  i.heightCap.xy).r + tex2D(_TexMaps, i.heightCap.zw).r * _DetailHeight; 					
					float blendHeight = lerp(blendHeightBody, blendHeightCap, i.col.a);
					float mapLiquid = tex2D(_TexLookupLiquid, float2(blendHeight, 0));	

					// LAVA MAP (stored in _TexMaps Blue Channel (Edge Details from Red Channel))
					#if LAVA_ON
 					float blendLavaBody = tex2D(_TexMaps, i.lavaBody.xy).b + tex2D(_TexMaps, i.lavaBody.zw).b * _DetailLava;
 					float blendLavaCap = tex2D(_TexMaps, i.lavaCap.xy).b + tex2D(_TexMaps, i.lavaCap.zw).b * _DetailLava;
					float blendLava = pow(lerp(blendLavaBody, blendLavaCap, i.col.a),0.5);
					float mapLava = tex2D(_TexLookupLava, float2(blendLava, 0));
					float mapGlow = tex2D(_TexLookupLavaGlow, float2(blendLava, 0));
					#endif
										
					// BIOME MAP (stored in _TexMaps Green Channel)																		
					float blendBiomeBody = tex2D(_TexMaps, i.biome.xy).g;
					float blendBiomeCap = tex2D(_TexMaps, i.biome.zw).g;
					float mapBiome = lerp(blendBiomeBody, blendBiomeCap, i.col.a);
			
					// POLAR COVERAGE (stored in _TexMaps Alpha Channel)
					float mapPolar = tex2D(_TexMaps, i.texcoord.zw).a;
					mapPolar = tex2D(_TexLookupPolar, float2(mapPolar, 0)).r;						
					
					// CLOUDS
					#if CLOUDS_ON
					float4 cloudsBody = tex2D(_TexClouds, float2(_TilingClouds * i.texcoord.x - _Time.x * _CloudSpeed * 0.001, _TilingClouds * 0.5 * i.texcoord.y));
					float4 cloudsCap = tex2D(_TexClouds, float2(_TilingClouds * i.texcoord.z * 0.5, _TilingClouds * i.texcoord.w * 0.5));
					float4 clouds = lerp(cloudsBody, cloudsCap, i.col.a);
					
					#endif
					
					// CITY LIGHTS
					float cities = tex2D(_TexCities, float2(i.texcoord.x * _TilingCities, i.texcoord.y * _TilingCities * 0.5)).g * (1-i.col.a);

					// Get body surface and normal colors from spherical section of planet
					float4 bodyDiffSpec1 = tex2D(_TexBiome1DiffSpec, i.surface.xy);
					float4 bodyDiffSpec2 = tex2D(_TexBiome2DiffSpec, i.surface.xy);
					float4 bodyIceDiffSpec = tex2D( _TexIceDiffuse, i.surface.xy);					
					float4 bodyNormal1 = tex2D(_TexBiome1Normal, i.surface.xy);
					float4 bodyNormal2 = tex2D(_TexBiome2Normal, i.surface.xy);
										
					// Get cap surface and normal colors from polar cap sections of planet
					float4 capDiffSpec1 = tex2D(_TexBiome1DiffSpec, i.surface.zw);
					float4 capDiffSpec2 = tex2D(_TexBiome2DiffSpec, i.surface.zw);
					float4 capIceDiffSpec = tex2D(_TexIceDiffuse, i.surface.zw);					
					float4 capNormal1 = 1-tex2D(_TexBiome1Normal, i.surface.zw);
					float4 capNormal2 = 1-tex2D(_TexBiome2Normal, i.surface.zw);					

					// LAVA
					// Get the flow texture from the alpha channel and animate it by panning it
					#if LAVA_ON
					float3 bodyFlowLava = tex2D(_TexLavaFlow, i.surface.xy - (_Time.x * 0.2 * _LavaFlowSpeed)).rgb;
					float3 capFlowLava = tex2D(_TexLavaFlow, i.surface.zw - (_Time.x * 0.2 * _LavaFlowSpeed)).rgb;
					float3 flowLava = lerp(bodyFlowLava, capFlowLava, i.col.a);

					// Get the body and cap  textures from the RGB channel and blend it between polar caps and planet body
					float4 bodyLavaDiffuse = tex2D(_TexLavaDiffuse, i.surface.xy * 0.25 + float4(flowLava * 0.125, 0) - _Time.x*0.1 * _LavaFlowSpeed) * 0.5;
					float4 capLavaDiffuse = tex2D(_TexLavaDiffuse, i.surface.zw * 0.25 + float4(flowLava * 0.125, 0) -_Time.x*0.1 * _LavaFlowSpeed) * 0.5;

					float4 lavaDiffuse = lerp(bodyLavaDiffuse, capLavaDiffuse, i.col.a);
					#endif
					
					// Combine diffuse and specular maps for body and cap based on biome map
					float4 bodyDiffSpec = lerp(bodyDiffSpec1, bodyDiffSpec2, mapBiome);
					
					// Combine normal maps for body based on biome map
					float4 bodyNormal = lerp(bodyNormal1, bodyNormal2, mapBiome);
					
					// Combine diffuse and specular maps for caps based on biome map
					float4 capDiffSpec = lerp(capDiffSpec1, capDiffSpec2, mapBiome);					
					// Blend polar cap ice based on polar map
					capDiffSpec = lerp (capDiffSpec, capIceDiffSpec, mapPolar);										
										
					// Combine Normal 1 and Normal 2 based on biome map
					float4 capNormal = lerp(capNormal1, capNormal2, mapBiome);					

					// Crossfade between body and cap diffuse maps and normal maps based on polar to body transition
					float4 colorDiffuse = float4(lerp(bodyDiffSpec, capDiffSpec, i.col.a).rgb,1);					
					float4 colorNormal = float4(lerp(bodyNormal, capNormal, i.col.a).rgba);					
					
					// Set final diffuse color to water or diffuse land color based on liquid map
					float4 colorFinal = lerp(colorDiffuse, lerp(_ColorLiquid, colorDiffuse, mapLiquid), _LiquidOpacity);
					colorFinal = lerp(colorFinal, _ColorIce, lerp(0, 1-mapLiquid, mapPolar));					

					// Set final normal color to flat or normal land color based on water map
					float4 normalFinal = lerp(normalFlat, colorNormal, mapLiquid);
					#if CLOUDS_ON
					// POW( ?, 0.454545) IS USED IN LINEAR TO CONVERT FROM GAMMA, OTHERWISE NORMAL MAP IS WRONG
					//normalFinal = lerp(normalFinal, lerp(normalFlat, float4(1, pow(clouds.b, 0.454545), 1, pow(clouds.g, 0.454545)), _CloudOpacity) , saturate(clouds.r));
					//normalFinal = lerp(normalFinal, lerp(normalFlat, float4(1, clouds.b, 1, clouds.g), _CloudOpacity), saturate(clouds.r));
					#endif														

                    // Unpack Normals
                    //float3 localCoords = float3(2 * normalFinal.ag - float2(1.0, 1.0), 0.0);                    
					float3 localCoords = float3(2 * normalFinal.rg - float2(1.0, 1.0), 0.0);
                    localCoords.z = _SurfaceRoughness;                    
                    
                    // Normal Transpose Matrix
                    float3x3 local2WorldTranspose = float3x3 (
                    	i.tangentWorld,
                    	i.binormalWorld,
                    	i.normalWorld
                    );
                    
                    // Normal Transpose Matrix 2
                    float3x3 local2WorldTranspose2 = float3x3 (
                    	i.tangentWorld2,
                    	i.binormalWorld2,
                    	i.normalWorld
                    );                    
                                        
                    // Calculate Normal Direction
                   	float3 normalDirection = normalize(mul(localCoords, local2WorldTranspose));
                   	float3 normalDirection2 = normalize(mul(localCoords, local2WorldTranspose2));

					// Calculate View Direction (this is done in fragmentation program for pixel precision instead of vertex reflection
					float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldvertpos);                    

					// ***** LIGHTING *****                                        
                    // Calculate diffuse reflection and lighting based on normal direction and light direction
					float3 diffuseReflection = lerp(saturate(dot(normalDirection, lightDirection)), saturate(dot(normalDirection2, lightDirection)), i.col.a);// +float3(0.005, 0.005, 0.005);

                    // Calculate specular reflection - remove specular reflection for clouds
                    #if CLOUDS_ON
                    float3 specularReflection = (1.0 - clouds.r * _CloudOpacity) * (1.0 - mapLiquid) * 0.5 * _ColorSpecular * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDir)), _SpecularPowerLiquid);
                    #else
                    float3 specularReflection = (1.0 - mapLiquid) * 0.5 * _ColorSpecular * pow(saturate(dot(reflect(-lightDirection, normalDirection), viewDir)), _SpecularPowerLiquid);
                    #endif	
                    // ADD SPECULAR FOR SURFACE
                    
                    // Combine scene ambient light, diffuse reflection, and specular reflection to finel light
					float3 lightFinal = (_LocalStarAmbientIntensity) + diffuseReflection + specularReflection;					

					// Twilight Zone (dusk / dawn)
					float3 twilight = saturate(pow(1 - pow(dot(i.normal, lightDirection),2), 80) * 0.1);
					lightFinal.rgb = lightFinal.rgb + (twilight * _ColorTwilight);					

					// Water Specular Reflection **** FIX COLOR?
                    colorFinal.rgb = lerp(colorFinal.rgb, float3(1.0,1.0,1.0), specularReflection.r);
                   
                    // Caching cross product of light and normal used to calculate cloud height offset
  			    	float3 lCrossN = cross(lightDirection, normalDirection);  			    	  			    	
  			    	
  			    	#if CLOUDS_ON
	            	float colorCloudShadow = tex2D(_TexClouds, float2(_TilingClouds * i.texcoord.x - _Time.x * 0.001 * _CloudSpeed + (lCrossN.y * _CloudHeight * 0.001), _TilingClouds * 0.5 * i.texcoord.y - (lCrossN.x * _CloudHeight * 0.001))).r;
					colorCloudShadow = saturate((4 * colorCloudShadow) - 1);
	            	// Darken the planet surface texture by the cloud and the offset cloud UV coordinates (crossfaded between spherical and polar UV coordinates)
	            	colorFinal -= _CloudShadow * float4(1.0,1.0,1.0,1.0) * colorCloudShadow * _CloudOpacity * (1-i.col.a);
	            	// Add clouds to final color	            	
					colorFinal.rgb = lerp(colorFinal.rgb, (colorFinal.rgb * (1 - _CloudOpacity)) + _ColorClouds.rgb * 2 * _CloudOpacity, clouds.r * 1);
	                #endif
                       
                    // Calculate and add atmosphere color and falloff (atmosphere is calculated in vertex shader and passed through i.col.b component                    
                    colorFinal.rgb = lerp(colorFinal.rgb, _ColorAtmosphere.rgb, i.col.b)  ;
                    colorFinal.rgb += colorFinal.rgb * i.col.b * 2; 
										

 					// Calculate city lights effect on shadow side of planet 					
 					float3 colorCities = saturate(1 - (diffuseReflection * 5)) * lerp(float4(0,0,0,1), _ColorCities, lerp(0, cities, mapLiquid));
 					
					colorFinal.rgb = (colorFinal.rgb * lightFinal) + colorCities.rgb;					
 					
 					#if LAVA_ON && CLOUDS_ON
					colorFinal.rgb = saturate(lerp(colorFinal.rgb, lavaDiffuse, lerp(0, 1-mapLava, saturate(mapLiquid - clouds.r * _CloudOpacity)))); 
					colorFinal.rgb += saturate(lerp(0, _ColorLavaGlow * (1 - mapGlow), mapLiquid - clouds.r * _CloudOpacity));
 					#endif
 					
 					#if LAVA_ON && CLOUDS_OFF
					colorFinal.rgb = saturate(lerp(colorFinal.rgb, lavaDiffuse, lerp(0, 1 - mapLava, mapLiquid)));
 					colorFinal.rgb += saturate(lerp(0, _ColorLavaGlow * (1 - mapGlow), mapLiquid)); 					
 					#endif 					
 					 
 					return float4(colorFinal.rgb * _LocalStarIntensity * _LocalStarColor, 1.0); 				
                }				
            ENDCG
        }
     }
 }
 
