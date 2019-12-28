// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Particles/Space Dust" {

	Properties
	{
		_TintColor("Tint Color", Color) = (1,1,1,1)
		_MainTex("Particle Texture", 2D) = "white" {}
		_FadeDistance("Fade Distance", float) = 0.5
	}

	SubShader
	{
		
		Tags{ "Queue" = "Transparent" }

		ColorMask RGB
		Lighting Off
		ZWrite Off
		AlphaTest Greater .01
		Blend SrcAlpha One

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4    _MainTex_ST,
			_TintColor;
			uniform float _FadeDistance;

			struct appdata_vert 
			{
				float4 vertex : POSITION;
				float4 texcoord : TEXCOORD0;
				float4 color : COLOR;
			};

			uniform sampler2D _MainTex;

			struct v2f 
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			v2f vert(appdata_vert v) 
			{

				v2f o;
				o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pos = UnityObjectToClipPos(v.vertex);
				float4 viewPos = mul(UNITY_MATRIX_MV, v.vertex);

				float alpha = (-viewPos.z - _ProjectionParams.y) / _FadeDistance;
				if (alpha > 1)
				{
					alpha = max(0, 2 - alpha);
				}

				o.color = float4(v.color.rgb, v.color.a*alpha);
				o.color *= _TintColor * 2;

				return o;

			}

			float4 frag(v2f i) : COLOR
			{
				return tex2D(_MainTex, i.uv)*i.color;
			}

			ENDCG
		}
	}

		Fallback "Particles/Additive"
}