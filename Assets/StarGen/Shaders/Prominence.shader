// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:35451,y:32578,varname:node_9361,prsc:2|emission-4843-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:4511,x:32819,y:33086,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:aaa9393ea1cdb854699fa8c25858e8a5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:4431,x:33585,y:32893,varname:_node_4431,prsc:2,tex:aaa9393ea1cdb854699fa8c25858e8a5,ntxv:0,isnm:False|UVIN-164-UVOUT,TEX-4511-TEX;n:type:ShaderForge.SFN_TexCoord,id:4829,x:32279,y:32764,varname:node_4829,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ArcTan2,id:3929,x:32759,y:32727,varname:node_3929,prsc:2,attp:2|A-8588-OUT,B-8552-OUT;n:type:ShaderForge.SFN_RemapRange,id:8588,x:32463,y:32662,varname:node_8588,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-4829-U;n:type:ShaderForge.SFN_RemapRange,id:8552,x:32463,y:32826,varname:node_8552,prsc:2,frmn:0,frmx:1,tomn:0,tomx:1|IN-4829-V;n:type:ShaderForge.SFN_Append,id:8669,x:32720,y:32588,varname:node_8669,prsc:2|A-8588-OUT,B-8552-OUT;n:type:ShaderForge.SFN_Length,id:8663,x:32861,y:32486,varname:node_8663,prsc:2|IN-8669-OUT;n:type:ShaderForge.SFN_Append,id:7306,x:33205,y:32655,varname:node_7306,prsc:2|A-1842-OUT,B-5586-OUT;n:type:ShaderForge.SFN_Multiply,id:7389,x:33029,y:32463,varname:node_7389,prsc:2|A-3044-OUT,B-8663-OUT;n:type:ShaderForge.SFN_Vector1,id:3044,x:32910,y:32365,varname:node_3044,prsc:2,v1:5;n:type:ShaderForge.SFN_Subtract,id:9304,x:33342,y:32446,varname:node_9304,prsc:2|A-7389-OUT,B-6631-OUT;n:type:ShaderForge.SFN_Subtract,id:6243,x:33342,y:32319,varname:node_6243,prsc:2|A-6631-OUT,B-9093-OUT;n:type:ShaderForge.SFN_Vector1,id:4444,x:33185,y:32248,varname:node_4444,prsc:2,v1:1;n:type:ShaderForge.SFN_Subtract,id:7372,x:33582,y:32306,varname:node_7372,prsc:2|A-7389-OUT,B-6243-OUT;n:type:ShaderForge.SFN_Multiply,id:1842,x:32964,y:32765,varname:node_1842,prsc:2|A-3929-OUT,B-9414-OUT;n:type:ShaderForge.SFN_OneMinus,id:6743,x:33582,y:32466,varname:node_6743,prsc:2|IN-9304-OUT;n:type:ShaderForge.SFN_Multiply,id:7071,x:33873,y:32452,varname:node_7071,prsc:2|A-7372-OUT,B-6743-OUT,C-8665-OUT;n:type:ShaderForge.SFN_Vector1,id:6631,x:33019,y:32291,varname:node_6631,prsc:2,v1:4;n:type:ShaderForge.SFN_Tex2d,id:9911,x:33585,y:33069,varname:_node_9911,prsc:2,tex:aaa9393ea1cdb854699fa8c25858e8a5,ntxv:0,isnm:False|UVIN-1695-UVOUT,TEX-4511-TEX;n:type:ShaderForge.SFN_Panner,id:164,x:33440,y:32738,varname:node_164,prsc:2,spu:0.2,spv:0|UVIN-7306-OUT,DIST-7891-OUT;n:type:ShaderForge.SFN_Panner,id:1695,x:33292,y:33012,varname:node_1695,prsc:2,spu:0.05,spv:0|UVIN-7306-OUT,DIST-7891-OUT;n:type:ShaderForge.SFN_Subtract,id:9064,x:34362,y:32619,varname:node_9064,prsc:2|A-7071-OUT,B-5257-OUT;n:type:ShaderForge.SFN_Multiply,id:1767,x:33969,y:32737,varname:node_1767,prsc:2|A-1669-OUT,B-834-OUT;n:type:ShaderForge.SFN_Vector1,id:834,x:33751,y:32658,varname:node_834,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Clamp01,id:4678,x:34577,y:32562,varname:node_4678,prsc:2|IN-9064-OUT;n:type:ShaderForge.SFN_Add,id:7049,x:33084,y:33356,varname:node_7049,prsc:2|A-3929-OUT,B-5545-OUT;n:type:ShaderForge.SFN_Slider,id:8442,x:32519,y:33395,ptovrint:False,ptlb:T,ptin:_T,varname:_T,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.1941748,max:1;n:type:ShaderForge.SFN_Multiply,id:2710,x:33439,y:33421,varname:node_2710,prsc:2|A-7049-OUT,B-7807-OUT;n:type:ShaderForge.SFN_Pi,id:7807,x:33147,y:33470,varname:node_7807,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:8665,x:34117,y:33288,varname:node_8665,prsc:2|IN-1237-OUT;n:type:ShaderForge.SFN_Multiply,id:1237,x:33968,y:33343,varname:node_1237,prsc:2|A-4712-OUT,B-4712-OUT,C-4712-OUT;n:type:ShaderForge.SFN_NormalVector,id:5350,x:34597,y:32830,prsc:2,pt:False;n:type:ShaderForge.SFN_ViewVector,id:3237,x:34580,y:32968,varname:node_3237,prsc:2;n:type:ShaderForge.SFN_Dot,id:9003,x:34775,y:32874,varname:node_9003,prsc:2,dt:0|A-5350-OUT,B-3237-OUT;n:type:ShaderForge.SFN_Multiply,id:3452,x:35021,y:32743,varname:node_3452,prsc:2|A-4678-OUT,B-9003-OUT,C-4189-OUT;n:type:ShaderForge.SFN_Add,id:1669,x:33861,y:32975,varname:node_1669,prsc:2|A-4431-R,B-9911-G;n:type:ShaderForge.SFN_Vector1,id:9414,x:32737,y:32918,varname:node_9414,prsc:2,v1:2;n:type:ShaderForge.SFN_Color,id:7201,x:35124,y:33006,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9779412,c2:0.7656902,c3:0.2085316,c4:1;n:type:ShaderForge.SFN_Multiply,id:4843,x:35221,y:32794,varname:node_4843,prsc:2|A-3452-OUT,B-7201-RGB,C-4932-OUT;n:type:ShaderForge.SFN_Time,id:4319,x:32359,y:33135,varname:node_4319,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7891,x:32587,y:33135,varname:node_7891,prsc:2|A-8939-OUT,B-4319-TTR;n:type:ShaderForge.SFN_Slider,id:8939,x:31994,y:33066,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_DepthBlend,id:4189,x:34754,y:33061,varname:node_4189,prsc:2|DIST-9339-OUT;n:type:ShaderForge.SFN_Cos,id:4642,x:33665,y:33548,varname:node_4642,prsc:2|IN-2710-OUT;n:type:ShaderForge.SFN_OneMinus,id:4712,x:33845,y:33548,varname:node_4712,prsc:2|IN-4642-OUT;n:type:ShaderForge.SFN_RemapRange,id:5545,x:32925,y:33395,varname:node_5545,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:1.5|IN-8442-OUT;n:type:ShaderForge.SFN_Vector1,id:4932,x:34994,y:32644,varname:node_4932,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:9339,x:34564,y:33136,varname:node_9339,prsc:2,v1:0.04;n:type:ShaderForge.SFN_Multiply,id:5586,x:33497,y:32604,varname:node_5586,prsc:2|A-9304-OUT,B-8007-OUT;n:type:ShaderForge.SFN_Vector1,id:4667,x:33275,y:32604,varname:node_4667,prsc:2,v1:0.15;n:type:ShaderForge.SFN_RemapRange,id:3809,x:33914,y:32585,varname:node_3809,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.1|IN-4829-V;n:type:ShaderForge.SFN_Add,id:5257,x:34133,y:32727,varname:node_5257,prsc:2|A-3809-OUT,B-1767-OUT;n:type:ShaderForge.SFN_Slider,id:8432,x:33164,y:32110,ptovrint:False,ptlb:Width,ptin:_Width,varname:_Width,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Slider,id:4848,x:32673,y:32101,ptovrint:False,ptlb:Density,ptin:_Density,varname:_Density,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_RemapRange,id:8007,x:33004,y:32101,varname:node_8007,prsc:2,frmn:0,frmx:1,tomn:0.1,tomx:0.5|IN-4848-OUT;n:type:ShaderForge.SFN_RemapRange,id:9093,x:33517,y:32013,varname:node_9093,prsc:2,frmn:0,frmx:1,tomn:0,tomx:0.3|IN-8432-OUT;proporder:4511-8442-7201-8939-8432-4848;pass:END;sub:END;*/

Shader "Star/Prominence" {
    Properties {
        _Noise ("Noise", 2D) = "white" {}
        _T ("T", Range(0, 1)) = 0.1941748
        [HDR]_Color ("Color", Color) = (0.9779412,0.7656902,0.2085316,1)
        _Speed ("Speed", Range(0, 1)) = 1
        _Width ("Width", Range(0, 1)) = 1
        _Density ("Density", Range(0, 1)) = 1
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform float _T;
            uniform float4 _Color;
            uniform float _Speed;
            uniform float _Width;
            uniform float _Density;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float4 projPos : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                o.projPos = ComputeScreenPos (o.pos);
                COMPUTE_EYEDEPTH(o.projPos.z);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float sceneZ = max(0,LinearEyeDepth (UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)))) - _ProjectionParams.g);
                float partZ = max(0,i.projPos.z - _ProjectionParams.g);
////// Lighting:
////// Emissive:
                float node_8588 = (i.uv0.r*2.0+-1.0);
                float node_8552 = (i.uv0.g*1.0+0.0);
                float node_7389 = (5.0*length(float2(node_8588,node_8552)));
                float node_6631 = 4.0;
                float node_9304 = (node_7389-node_6631);
                float node_3929 = ((atan2(node_8588,node_8552)/6.28318530718)+0.5);
                float node_4712 = (1.0 - cos(((node_3929+(_T*2.0+-0.5))*3.141592654)));
                float4 node_4319 = _Time + _TimeEditor;
                float node_7891 = (_Speed*node_4319.a);
                float2 node_7306 = float2((node_3929*2.0),(node_9304*(_Density*0.4+0.1)));
                float2 node_164 = (node_7306+node_7891*float2(0.2,0));
                float4 _node_4431 = tex2D(_Noise,TRANSFORM_TEX(node_164, _Noise));
                float2 node_1695 = (node_7306+node_7891*float2(0.05,0));
                float4 _node_9911 = tex2D(_Noise,TRANSFORM_TEX(node_1695, _Noise));
                float3 emissive = ((saturate((((node_7389-(node_6631-(_Width*0.3+0.0)))*(1.0 - node_9304)*saturate((node_4712*node_4712*node_4712)))-((i.uv0.g*0.1+0.0)+((_node_4431.r+_node_9911.g)*0.2))))*dot(i.normalDir,viewDirection)*saturate((sceneZ-partZ)/0.04))*_Color.rgb*2.0);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
