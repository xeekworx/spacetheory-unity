// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:35682,y:32435,varname:node_9361,prsc:2|emission-7266-OUT;n:type:ShaderForge.SFN_TexCoord,id:1087,x:32282,y:32621,varname:node_1087,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:6726,x:32728,y:32842,varname:node_6726,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-1087-V;n:type:ShaderForge.SFN_Tex2dAsset,id:4430,x:33437,y:33045,ptovrint:False,ptlb:Main,ptin:_Main,varname:_Main,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Append,id:7676,x:33651,y:32674,varname:node_7676,prsc:2|A-9024-OUT,B-4449-OUT;n:type:ShaderForge.SFN_Add,id:4449,x:33416,y:32813,varname:node_4449,prsc:2|A-1087-V,B-7477-OUT,C-8301-OUT;n:type:ShaderForge.SFN_Multiply,id:7477,x:33224,y:32842,varname:node_7477,prsc:2|A-1250-OUT,B-7619-OUT;n:type:ShaderForge.SFN_Time,id:9438,x:32419,y:33109,varname:node_9438,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:5619,x:34107,y:32685,varname:_node_5619,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7676-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Sign,id:3627,x:32890,y:32842,varname:node_3627,prsc:2|IN-6726-OUT;n:type:ShaderForge.SFN_Negate,id:1250,x:33036,y:32925,varname:node_1250,prsc:2|IN-3627-OUT;n:type:ShaderForge.SFN_Multiply,id:3796,x:33220,y:32407,varname:node_3796,prsc:2|A-1250-OUT,B-7619-OUT,C-2088-OUT;n:type:ShaderForge.SFN_Add,id:2007,x:33385,y:32337,varname:node_2007,prsc:2|A-1087-V,B-3796-OUT,C-8301-OUT;n:type:ShaderForge.SFN_Append,id:8178,x:33620,y:32198,varname:node_8178,prsc:2|A-9024-OUT,B-2007-OUT;n:type:ShaderForge.SFN_Vector1,id:2088,x:32871,y:32328,varname:node_2088,prsc:2,v1:0.6;n:type:ShaderForge.SFN_Multiply,id:765,x:33905,y:32238,varname:node_765,prsc:2|A-9425-OUT,B-8178-OUT;n:type:ShaderForge.SFN_Vector1,id:9425,x:33671,y:32115,varname:node_9425,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:2748,x:34107,y:32511,varname:_node_2748,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-765-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Multiply,id:957,x:34367,y:32562,varname:node_957,prsc:2|A-8265-R,B-5619-R;n:type:ShaderForge.SFN_Tex2d,id:4593,x:33925,y:33148,varname:_node_4593,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-8060-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Add,id:9024,x:33461,y:32550,varname:node_9024,prsc:2|A-1087-U,B-3972-OUT;n:type:ShaderForge.SFN_Multiply,id:3972,x:33162,y:33239,varname:node_3972,prsc:2|A-758-OUT,B-4593-G;n:type:ShaderForge.SFN_Append,id:8060,x:33625,y:33267,varname:node_8060,prsc:2|A-1087-U,B-31-OUT;n:type:ShaderForge.SFN_Multiply,id:9614,x:33437,y:33267,varname:node_9614,prsc:2|A-7619-OUT,B-7941-OUT,C-1250-OUT;n:type:ShaderForge.SFN_Vector1,id:7941,x:33041,y:33581,varname:node_7941,prsc:2,v1:2;n:type:ShaderForge.SFN_Add,id:31,x:33456,y:33435,varname:node_31,prsc:2|A-1087-V,B-9614-OUT,C-8301-OUT;n:type:ShaderForge.SFN_Length,id:6751,x:34056,y:32987,varname:node_6751,prsc:2|IN-7487-OUT;n:type:ShaderForge.SFN_RemapRange,id:8995,x:32814,y:32491,varname:node_8995,prsc:2,frmn:0,frmx:1,tomn:-1.25,tomx:1.25|IN-1087-U;n:type:ShaderForge.SFN_RemapRange,id:2120,x:34195,y:32987,varname:node_2120,prsc:2,frmn:0.7,frmx:1,tomn:1,tomx:0|IN-6751-OUT;n:type:ShaderForge.SFN_Subtract,id:2643,x:34611,y:32759,varname:node_2643,prsc:2|A-959-OUT,B-586-OUT;n:type:ShaderForge.SFN_Multiply,id:586,x:34470,y:32917,varname:node_586,prsc:2|A-926-OUT,B-959-OUT;n:type:ShaderForge.SFN_Append,id:7487,x:33875,y:33398,varname:node_7487,prsc:2|A-8995-OUT,B-6726-OUT;n:type:ShaderForge.SFN_RemapRange,id:4655,x:34637,y:32568,varname:node_4655,prsc:2,frmn:0.4,frmx:1,tomn:0,tomx:1|IN-586-OUT;n:type:ShaderForge.SFN_Clamp01,id:8431,x:34949,y:32549,varname:node_8431,prsc:2|IN-4655-OUT;n:type:ShaderForge.SFN_Append,id:885,x:30632,y:32159,varname:node_885,prsc:2;n:type:ShaderForge.SFN_Clamp01,id:926,x:34373,y:32790,varname:node_926,prsc:2|IN-9890-OUT;n:type:ShaderForge.SFN_DepthBlend,id:211,x:34949,y:32690,varname:node_211,prsc:2|DIST-7548-OUT;n:type:ShaderForge.SFN_OneMinus,id:3808,x:35146,y:32680,varname:node_3808,prsc:2|IN-211-OUT;n:type:ShaderForge.SFN_Multiply,id:7266,x:35315,y:32589,varname:node_7266,prsc:2|A-8431-OUT,B-9042-RGB,C-211-OUT;n:type:ShaderForge.SFN_Vector1,id:7548,x:34732,y:32778,varname:node_7548,prsc:2,v1:0.05;n:type:ShaderForge.SFN_Color,id:9042,x:34975,y:32335,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:758,x:33021,y:33457,varname:node_758,prsc:2,v1:0.1;n:type:ShaderForge.SFN_Append,id:6932,x:30696,y:32223,varname:node_6932,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7619,x:32728,y:33102,varname:node_7619,prsc:2|A-2399-OUT,B-9438-TSL,C-2710-OUT;n:type:ShaderForge.SFN_Slider,id:2399,x:32350,y:33352,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Vector1,id:2710,x:32460,y:32977,varname:node_2710,prsc:2,v1:5;n:type:ShaderForge.SFN_Vector4Property,id:7183,x:32371,y:33818,ptovrint:False,ptlb:ParentRotation,ptin:_ParentRotation,varname:_ParentRotation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Dot,id:7696,x:32604,y:33740,varname:node_7696,prsc:2,dt:4|A-7930-OUT,B-7183-XYZ;n:type:ShaderForge.SFN_NormalVector,id:7930,x:32382,y:33654,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:8301,x:32855,y:33657,varname:node_8301,prsc:2|A-9307-OUT,B-7696-OUT;n:type:ShaderForge.SFN_Vector1,id:9307,x:32711,y:33627,varname:node_9307,prsc:2,v1:1;n:type:ShaderForge.SFN_Add,id:8067,x:32804,y:33407,varname:node_8067,prsc:2|A-7619-OUT,B-8301-OUT;n:type:ShaderForge.SFN_NormalVector,id:4486,x:34352,y:33205,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:7177,x:34566,y:33164,varname:node_7177,prsc:2,dt:0|A-4486-OUT,B-126-OUT;n:type:ShaderForge.SFN_ViewVector,id:126,x:34339,y:33363,varname:node_126,prsc:2;n:type:ShaderForge.SFN_Multiply,id:9890,x:34819,y:33156,varname:node_9890,prsc:2|A-2120-OUT,B-7177-OUT;n:type:ShaderForge.SFN_Tex2d,id:8265,x:34134,y:32277,varname:_node_7420,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7559-OUT,TEX-4430-TEX;n:type:ShaderForge.SFN_Append,id:7559,x:33690,y:31883,varname:node_7559,prsc:2|A-1087-U,B-1340-OUT;n:type:ShaderForge.SFN_Add,id:1340,x:33455,y:32022,varname:node_1340,prsc:2|A-1087-V,B-8075-OUT,C-8301-OUT;n:type:ShaderForge.SFN_Multiply,id:8075,x:33290,y:32092,varname:node_8075,prsc:2|A-576-OUT,B-7619-OUT,C-1250-OUT;n:type:ShaderForge.SFN_Vector1,id:576,x:32941,y:32013,varname:node_576,prsc:2,v1:-0.3;n:type:ShaderForge.SFN_Add,id:959,x:34568,y:32241,varname:node_959,prsc:2|A-2748-R,B-957-OUT;n:type:ShaderForge.SFN_ComponentMask,id:5781,x:33710,y:33607,varname:node_5781,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1;n:type:ShaderForge.SFN_Tangent,id:1891,x:32604,y:33893,varname:node_1891,prsc:2;n:type:ShaderForge.SFN_Dot,id:6394,x:32827,y:33906,varname:node_6394,prsc:2,dt:4|A-7183-XYZ,B-1891-OUT;n:type:ShaderForge.SFN_Add,id:9298,x:33047,y:33817,varname:node_9298,prsc:2|A-7696-OUT,B-6394-OUT;proporder:4430-9042-2399-7183;pass:END;sub:END;*/

Shader "Star/Jet" {
    Properties {
        _Main ("Main", 2D) = "white" {}
        [HDR]_Color ("Color", Color) = (0.5,0.5,0.5,1)
        _Speed ("Speed", Range(0, 1)) = 1
        _ParentRotation ("ParentRotation", Vector) = (0,0,0,0)
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _CameraDepthTexture;
            uniform float4 _TimeEditor;
            uniform sampler2D _Main; uniform float4 _Main_ST;
            uniform float4 _Color;
            uniform float _Speed;
            uniform float4 _ParentRotation;
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
                float node_6726 = (i.uv0.g*2.0+-1.0);
                float4 node_9438 = _Time + _TimeEditor;
                float node_7619 = (_Speed*node_9438.r*5.0);
                float node_1250 = (-1*sign(node_6726));
                float node_7696 = 0.5*dot(i.normalDir,_ParentRotation.rgb)+0.5;
                float node_8301 = (1.0*node_7696);
                float2 node_8060 = float2(i.uv0.r,(i.uv0.g+(node_7619*2.0*node_1250)+node_8301));
                float4 _node_4593 = tex2D(_Main,TRANSFORM_TEX(node_8060, _Main));
                float node_9024 = (i.uv0.r+(0.1*_node_4593.g));
                float2 node_765 = (0.5*float2(node_9024,(i.uv0.g+(node_1250*node_7619*0.6)+node_8301)));
                float4 _node_2748 = tex2D(_Main,TRANSFORM_TEX(node_765, _Main));
                float2 node_7559 = float2(i.uv0.r,(i.uv0.g+((-0.3)*node_7619*node_1250)+node_8301));
                float4 _node_7420 = tex2D(_Main,TRANSFORM_TEX(node_7559, _Main));
                float2 node_7676 = float2(node_9024,(i.uv0.g+(node_1250*node_7619)+node_8301));
                float4 _node_5619 = tex2D(_Main,TRANSFORM_TEX(node_7676, _Main));
                float node_959 = (_node_2748.r+(_node_7420.r*_node_5619.r));
                float node_586 = (saturate(((length(float2((i.uv0.r*2.5+-1.25),node_6726))*-3.333333+3.333333)*dot(i.normalDir,viewDirection)))*node_959);
                float node_211 = saturate((sceneZ-partZ)/0.05);
                float3 emissive = (saturate((node_586*1.666667+-0.6666667))*_Color.rgb*node_211);
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
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
