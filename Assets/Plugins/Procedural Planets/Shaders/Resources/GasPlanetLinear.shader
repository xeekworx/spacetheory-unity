// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ProceduralPlanets/GasPlanetLinear"
{
	Properties
	{
		_BodyTexture("BodyTexture", 2D) = "white" {}
		_CapTexture("CapTexture", 2D) = "white" {}
		_BodyNormal("BodyNormal", 2D) = "white" {}
		_CapNormal("CapNormal", 2D) = "white" {}
		_PaletteLookup("PaletteLookup", 2D) = "white" {}
		_LocalStarPosition("LocalStarPosition", Vector) = (-20,-10,-10,0)
		_Solidness("Solidness", Range( 0 , 1)) = 0.1
		_Roughness("Roughness", Range( 0 , 2)) = 0.5
		_Banding("Banding", Range( 0 , 1)) = 0.7721043
		_LocalStarColor("LocalStarColor", Color) = (1,1,1,1)
		_LocalStarAmbientIntensity("LocalStarAmbientIntensity", Range( 0 , 1)) = 0.005
		_LocalStarIntensity("LocalStarIntensity", Range( 0 , 20)) = 1
		_VTiling("VTiling", Int) = 4
		_HTiling("HTiling", Int) = 4
		_Faintness("Faintness", Range( 0 , 1)) = 0
		_StormMask("StormMask", 2D) = "white" {}
		_StormColor("StormColor", Color) = (0,0,0,0)
		_StormTint("StormTint", Float) = 0
		_AtmosphereFalloff("AtmosphereFalloff", Range( 0 , 20)) = 0
		_AtmosphereColor("AtmosphereColor", Color) = (0.7279412,0.6752282,0.4549632,0)
		_ColorTwilight("ColorTwilight", Color) = (0,0,0,0)
		_FaintnessColor("FaintnessColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _texcoord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float2 uv2_texcoord2;
			float3 worldNormal;
			INTERNAL_DATA
		};

		uniform sampler2D _PaletteLookup;
		uniform float _Banding;
		uniform sampler2D _BodyTexture;
		uniform int _HTiling;
		uniform int _VTiling;
		uniform sampler2D _StormMask;
		uniform sampler2D _CapTexture;
		uniform float4 _StormColor;
		uniform float _StormTint;
		uniform float4 _FaintnessColor;
		uniform float _Faintness;
		uniform float4 _AtmosphereColor;
		uniform float _AtmosphereFalloff;
		uniform sampler2D _BodyNormal;
		uniform float _Roughness;
		uniform sampler2D _CapNormal;
		uniform float3 _LocalStarPosition;
		uniform float4 _LocalStarColor;
		uniform float _LocalStarAmbientIntensity;
		uniform float _LocalStarIntensity;
		uniform float4 _ColorTwilight;
		uniform float _Solidness;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 appendResult114 = (float4((float)_HTiling , (float)_VTiling , 0.0 , 0.0));
			float2 uv_TexCoord2 = i.uv_texcoord * appendResult114.xy;
			float4 tex2DNode39 = tex2D( _BodyTexture, uv_TexCoord2 );
			float4 tex2DNode125 = tex2D( _StormMask, i.uv_texcoord );
			float lerpResult126 = lerp( tex2DNode39.r , tex2DNode39.b , tex2DNode125.r);
			float clampResult21 = clamp( pow( ( abs( ase_vertex3Pos.y ) / 4.5 ) , 50.0 ) , 0.0 , 1.0 );
			float lerpResult12 = lerp( lerpResult126 , tex2D( _CapTexture, i.uv2_texcoord2 ).g , clampResult21);
			float blendOpSrc24 = ( ( ( ase_vertex3Pos.y + 5.0 ) / 10.0 ) * _Banding );
			float blendOpDest24 = lerpResult12;
			float clampResult31 = clamp( ( saturate( ( 1.0 - ( 1.0 - blendOpSrc24 ) * ( 1.0 - blendOpDest24 ) ) )) , 0.0 , 1.0 );
			float2 appendResult30 = (float2(0.5 , clampResult31));
			float4 tex2DNode38 = tex2D( _PaletteLookup, appendResult30 );
			float4 lerpResult144 = lerp( tex2DNode38 , _StormColor , _StormTint);
			float lerpResult140 = lerp( 0.0 , tex2DNode39.a , tex2DNode125.r);
			float4 lerpResult137 = lerp( tex2DNode38 , lerpResult144 , lerpResult140);
			float4 lerpResult121 = lerp( lerpResult137 , _FaintnessColor , _Faintness);
			float4 temp_cast_3 = (0.0).xxxx;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float dotResult69 = dot( ase_normWorldNormal , ase_worldViewDir );
			float temp_output_71_0 = saturate( dotResult69 );
			float4 lerpResult153 = lerp( temp_cast_3 , _AtmosphereColor , pow( ( 1.0 - temp_output_71_0 ) , _AtmosphereFalloff ));
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3 normalizeResult85 = normalize( mul( UnpackScaleNormal( tex2D( _BodyNormal, uv_TexCoord2 ) ,_Roughness ), float3x3(ase_worldTangent, ase_worldBitangent, ase_worldNormal) ) );
			float3 normalizeResult97 = normalize( mul( UnpackScaleNormal( tex2D( _CapNormal, i.uv2_texcoord2 ) ,_Roughness ), float3x3(ase_worldTangent, ase_worldBitangent, ase_worldNormal) ) );
			float3 lerpResult99 = lerp( normalizeResult85 , normalizeResult97 , clampResult21);
			float4 transform66 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float4 normalizeResult64 = normalize( ( float4( _LocalStarPosition , 0.0 ) - transform66 ) );
			float dotResult51 = dot( float4( lerpResult99 , 0.0 ) , normalizeResult64 );
			float dotResult157 = dot( float4( ase_normWorldNormal , 0.0 ) , normalizeResult64 );
			o.Emission = ( ( lerpResult121 + lerpResult153 ) * ( ( ( saturate( ( saturate( dotResult51 ) + ( _LocalStarColor * _LocalStarAmbientIntensity ) ) ) * _LocalStarColor ) * _LocalStarIntensity ) + ( _ColorTwilight * saturate( pow( ( 1.0 - pow( dotResult157 , 2.0 ) ) , 80.0 ) ) ) ) ).rgb;
			o.Alpha = saturate( ( pow( temp_output_71_0 , 4.0 ) * (10.0 + (_Solidness - 0.0) * (10000.0 - 10.0) / (1.0 - 0.0)) ) );
		}

		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15401
-71;811;1906;1004;-379.8912;476.6796;1.73295;True;True
Node;AmplifyShaderEditor.IntNode;115;-852.1404,-383.6132;Float;False;Property;_VTiling;VTiling;12;0;Create;True;0;0;False;0;4;3;0;1;INT;0
Node;AmplifyShaderEditor.IntNode;112;-860.1536,-471.7355;Float;False;Property;_HTiling;HTiling;13;0;Create;True;0;0;False;0;4;4;0;1;INT;0
Node;AmplifyShaderEditor.DynamicAppendNode;114;-639.8451,-436.2481;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;11;-299.4627,343.365;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-597.8133,-70.51899;Float;False;1;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-430.0783,-312.9116;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;4,4;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.AbsOpNode;15;-60.42585,324.7249;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;1625.784,1061.842;Float;False;Property;_Roughness;Roughness;7;0;Create;True;0;0;False;0;0.5;0.486;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;78;1195.049,648.152;Float;True;Property;_BodyNormal;BodyNormal;2;0;Create;True;0;0;False;0;None;e9f528750fe2b4aa1a0fe43669106046;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexTangentNode;86;1726.919,1184.891;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;87;1881.749,1519.32;Float;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;127;-285.4314,-655.92;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;16;119.7645,199.2827;Float;False;2;0;FLOAT;0;False;1;FLOAT;4.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexBinormalNode;94;1720.763,1371.803;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;79;1218.786,895.2606;Float;True;Property;_CapNormal;CapNormal;3;0;Create;True;0;0;False;0;None;e9f528750fe2b4aa1a0fe43669106046;True;1;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.UnpackScaleNormalNode;95;1537.23,769.2903;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.UnpackScaleNormalNode;81;1784.683,569.7411;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.MatrixFromVectors;92;2199.941,1264.718;Float;False;FLOAT3x3;True;4;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;1;FLOAT3x3;0
Node;AmplifyShaderEditor.SamplerNode;125;44.82517,-648.7306;Float;True;Property;_StormMask;StormMask;15;0;Create;True;0;0;False;0;None;;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-14.40786,463.3075;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;17;304.5547,108.1913;Float;False;2;0;FLOAT;0;False;1;FLOAT;50;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;39;-78.40387,-405.0357;Float;True;Property;_BodyTexture;BodyTexture;0;0;Create;True;0;0;False;0;e9f528750fe2b4aa1a0fe43669106046;e9f528750fe2b4aa1a0fe43669106046;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;2169.937,511.1615;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;2216.956,772.1915;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;47;1144.035,-651.9394;Float;False;Property;_LocalStarPosition;LocalStarPosition;5;0;Create;True;0;0;False;0;-20,-10,-10;33.22,8.2,-11.7;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;66;1163.663,-830.8376;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;21;503.8727,-11.01853;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;23;163.1921,421.7075;Float;False;2;0;FLOAT;0;False;1;FLOAT;10;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;40;-154.3855,-110.8289;Float;True;Property;_CapTexture;CapTexture;1;0;Create;True;0;0;False;0;e9f528750fe2b4aa1a0fe43669106046;e9f528750fe2b4aa1a0fe43669106046;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;126;503.4457,-457.7071;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;36;172.2547,552.6863;Float;False;Property;_Banding;Banding;8;0;Create;True;0;0;False;0;0.7721043;0.197;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;97;2358.761,718.4438;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;49;1445.159,-686.1557;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NormalizeNode;85;2328.938,497.5145;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;504.8345,383.9799;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;12;443.8189,-235.9564;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;61;995.6213,-355.7802;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NormalizeNode;64;1616.025,-571.8734;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BlendOpsNode;24;749.7992,48.29329;Float;False;Screen;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;99;2552.821,495.9645;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DotProductOpNode;157;2394.916,-1205.779;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;108;1690.183,-811.9854;Float;False;Property;_LocalStarAmbientIntensity;LocalStarAmbientIntensity;10;0;Create;True;0;0;False;0;0.005;0.003;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;105;1825.734,-1061.774;Float;False;Property;_LocalStarColor;LocalStarColor;9;0;Create;True;0;0;False;0;1,1,1,1;1,1,1,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;31;819.3239,216.6248;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;44;903.7573,-203.4042;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;51;1809.788,-369.2919;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;53;2054.147,-468.1912;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;2041.899,-818.9208;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;158;2689.594,-1183.889;Float;False;2;0;FLOAT;0;False;1;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;30;978.2211,323.4673;Float;False;FLOAT2;4;0;FLOAT;0.5;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;69;1358.656,-241.845;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;71;1516.642,-241.6665;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;2218.023,-666.5807;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;160;2901.768,-1116.532;Float;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;145;1133.535,1260.478;Float;False;Property;_StormTint;StormTint;17;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;142;455.9721,751.1069;Float;False;Constant;_Float0;Float 0;19;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;1174.909,111.6627;Float;True;Property;_PaletteLookup;PaletteLookup;4;0;Create;True;0;0;False;0;e9f528750fe2b4aa1a0fe43669106046;e9f528750fe2b4aa1a0fe43669106046;True;0;False;white;Auto;False;Object;-1;Auto;ProceduralTexture;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;136;689.2529,887.2507;Float;False;Property;_StormColor;StormColor;16;0;Create;True;0;0;False;0;0,0,0,0;0.7794118,0.1260813,0.2747706,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;144;1438.432,1158.31;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;58;2320.06,-494.9277;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;159;3102.151,-1044.127;Float;False;2;0;FLOAT;0;False;1;FLOAT;80;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;150;2382.83,-139.59;Float;False;Property;_AtmosphereFalloff;AtmosphereFalloff;18;0;Create;True;0;0;False;0;0;7.84;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;140;794.6555,591.9299;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;148;2492.939,-332.0264;Float;False;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;106;2542.946,-614.4222;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;161;3361.465,-880.7884;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;2410.897,-827.5826;Float;False;Property;_LocalStarIntensity;LocalStarIntensity;11;0;Create;True;0;0;False;0;1;1.77;0;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;151;2546.216,68.33157;Float;False;Property;_AtmosphereColor;AtmosphereColor;19;0;Create;True;0;0;False;0;0.7279412,0.6752282,0.4549632,0;0.4840531,0.4926471,0.3368836,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;77;1266.968,-58.71362;Float;False;Property;_Solidness;Solidness;6;0;Create;True;0;0;False;0;0.1;0.269;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;162;3325.621,-1118.016;Float;False;Property;_ColorTwilight;ColorTwilight;20;0;Create;True;0;0;False;0;0,0,0,0;0.2224644,0,0.3014706,1;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;137;1646.526,166.545;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.PowerNode;149;2663.231,-226.5652;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;156;2749.796,-23.78285;Float;False;Constant;_Float1;Float 1;22;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;122;1940.257,369.0338;Float;False;Property;_Faintness;Faintness;14;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;166;1694.042,396.0486;Float;False;Property;_FaintnessColor;FaintnessColor;21;0;Create;True;0;0;False;0;0,0,0,0;0.5367647,0.5367647,0.5367647,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;153;2939.043,40.20421;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;121;2119.374,94.5109;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;3662.407,-845.2222;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;110;2735.024,-654.9853;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;76;1718.87,-62.53281;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;10;False;4;FLOAT;10000;False;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;70;1708.229,-179.5339;Float;False;2;0;FLOAT;0;False;1;FLOAT;4;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;165;3542.553,-694.5588;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;1931.748,-209.3366;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;1000;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;154;2968.588,-187.6263;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;155;3167.973,136.8;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;2987.267,-407.0312;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;73;2139.893,-184.191;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3548.367,524.239;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;ProceduralPlanets/GasPlanetLinear;False;False;False;False;True;True;True;True;True;True;True;True;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;-1;False;-1;-1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;114;0;112;0
WireConnection;114;1;115;0
WireConnection;2;0;114;0
WireConnection;15;0;11;2
WireConnection;78;1;2;0
WireConnection;16;0;15;0
WireConnection;79;1;5;0
WireConnection;95;0;79;0
WireConnection;95;1;101;0
WireConnection;81;0;78;0
WireConnection;81;1;101;0
WireConnection;92;0;86;0
WireConnection;92;1;94;0
WireConnection;92;2;87;0
WireConnection;125;1;127;0
WireConnection;22;0;11;2
WireConnection;17;0;16;0
WireConnection;39;1;2;0
WireConnection;84;0;81;0
WireConnection;84;1;92;0
WireConnection;96;0;95;0
WireConnection;96;1;92;0
WireConnection;21;0;17;0
WireConnection;23;0;22;0
WireConnection;40;1;5;0
WireConnection;126;0;39;1
WireConnection;126;1;39;3
WireConnection;126;2;125;1
WireConnection;97;0;96;0
WireConnection;49;0;47;0
WireConnection;49;1;66;0
WireConnection;85;0;84;0
WireConnection;37;0;23;0
WireConnection;37;1;36;0
WireConnection;12;0;126;0
WireConnection;12;1;40;2
WireConnection;12;2;21;0
WireConnection;64;0;49;0
WireConnection;24;0;37;0
WireConnection;24;1;12;0
WireConnection;99;0;85;0
WireConnection;99;1;97;0
WireConnection;99;2;21;0
WireConnection;157;0;61;0
WireConnection;157;1;64;0
WireConnection;31;0;24;0
WireConnection;51;0;99;0
WireConnection;51;1;64;0
WireConnection;53;0;51;0
WireConnection;109;0;105;0
WireConnection;109;1;108;0
WireConnection;158;0;157;0
WireConnection;30;1;31;0
WireConnection;69;0;61;0
WireConnection;69;1;44;0
WireConnection;71;0;69;0
WireConnection;57;0;53;0
WireConnection;57;1;109;0
WireConnection;160;1;158;0
WireConnection;38;1;30;0
WireConnection;144;0;38;0
WireConnection;144;1;136;0
WireConnection;144;2;145;0
WireConnection;58;0;57;0
WireConnection;159;0;160;0
WireConnection;140;0;142;0
WireConnection;140;1;39;4
WireConnection;140;2;125;1
WireConnection;148;1;71;0
WireConnection;106;0;58;0
WireConnection;106;1;105;0
WireConnection;161;0;159;0
WireConnection;137;0;38;0
WireConnection;137;1;144;0
WireConnection;137;2;140;0
WireConnection;149;0;148;0
WireConnection;149;1;150;0
WireConnection;153;0;156;0
WireConnection;153;1;151;0
WireConnection;153;2;149;0
WireConnection;121;0;137;0
WireConnection;121;1;166;0
WireConnection;121;2;122;0
WireConnection;164;0;162;0
WireConnection;164;1;161;0
WireConnection;110;0;106;0
WireConnection;110;1;107;0
WireConnection;76;0;77;0
WireConnection;70;0;71;0
WireConnection;165;0;110;0
WireConnection;165;1;164;0
WireConnection;72;0;70;0
WireConnection;72;1;76;0
WireConnection;154;0;121;0
WireConnection;154;1;153;0
WireConnection;48;0;154;0
WireConnection;48;1;165;0
WireConnection;73;0;72;0
WireConnection;0;2;48;0
WireConnection;0;9;73;0
ASEEND*/
//CHKSM=4B7DF41381808DFF921C74C2B8E9CBA392D6FD0E