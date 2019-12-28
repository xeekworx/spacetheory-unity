// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:2,hqsc:True,nrmq:0,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:34384,y:32369,varname:node_9361,prsc:2|emission-9416-OUT;n:type:ShaderForge.SFN_TexCoord,id:5902,x:32363,y:32813,varname:node_5902,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4813,x:32595,y:32657,varname:node_4813,prsc:2|A-5902-UVOUT,B-8106-OUT;n:type:ShaderForge.SFN_Pi,id:8106,x:32409,y:32639,varname:node_8106,prsc:2;n:type:ShaderForge.SFN_ComponentMask,id:7798,x:32938,y:32628,varname:node_7798,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-1462-OUT;n:type:ShaderForge.SFN_Sin,id:1462,x:32751,y:32628,varname:node_1462,prsc:2|IN-4813-OUT;n:type:ShaderForge.SFN_Multiply,id:5373,x:33163,y:32603,varname:node_5373,prsc:0|A-7798-R,B-7798-G;n:type:ShaderForge.SFN_Multiply,id:5421,x:33395,y:32628,varname:node_5421,prsc:2|A-5373-OUT,B-5373-OUT,C-5373-OUT,D-5373-OUT;n:type:ShaderForge.SFN_Clamp01,id:9416,x:34006,y:32463,varname:node_9416,prsc:2|IN-6527-OUT;n:type:ShaderForge.SFN_Multiply,id:6527,x:33794,y:32474,varname:node_6527,prsc:2|A-9710-RGB,B-2840-OUT,C-8276-OUT;n:type:ShaderForge.SFN_Color,id:9710,x:33436,y:32320,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.3970588,c3:0.3970588,c4:1;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:2840,x:34115,y:32809,varname:node_2840,prsc:2|IN-3056-OUT,IMIN-9843-OUT,IMAX-9185-OUT,OMIN-9843-OUT,OMAX-1939-OUT;n:type:ShaderForge.SFN_Add,id:5955,x:33551,y:32911,varname:node_5955,prsc:2|A-9136-OUT,B-9893-OUT;n:type:ShaderForge.SFN_Slider,id:470,x:32781,y:32871,ptovrint:False,ptlb:Contrast,ptin:_Contrast,varname:_Contrast,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_OneMinus,id:3394,x:33097,y:32796,varname:node_3394,prsc:2|IN-470-OUT;n:type:ShaderForge.SFN_Subtract,id:2624,x:33551,y:33041,varname:node_2624,prsc:2|A-9893-OUT,B-9136-OUT;n:type:ShaderForge.SFN_Divide,id:9136,x:33278,y:32881,varname:node_9136,prsc:0|A-3394-OUT,B-9487-OUT;n:type:ShaderForge.SFN_Vector1,id:9487,x:33097,y:33013,varname:node_9487,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:9843,x:33961,y:33105,varname:node_9843,prsc:0,v1:0;n:type:ShaderForge.SFN_Vector1,id:1939,x:34051,y:33248,varname:node_1939,prsc:0,v1:1;n:type:ShaderForge.SFN_RemapRange,id:3319,x:33228,y:32381,varname:node_3319,prsc:2,frmn:0.3,frmx:1,tomn:0,tomx:1|IN-5373-OUT;n:type:ShaderForge.SFN_Clamp01,id:3923,x:33758,y:32208,varname:node_3923,prsc:2|IN-3319-OUT;n:type:ShaderForge.SFN_OneMinus,id:9893,x:33197,y:33121,varname:node_9893,prsc:0|IN-3285-OUT;n:type:ShaderForge.SFN_RemapRange,id:6668,x:32482,y:33029,varname:node_6668,prsc:0,frmn:0,frmx:1,tomn:-1,tomx:1|IN-5902-UVOUT;n:type:ShaderForge.SFN_Length,id:4487,x:32461,y:33285,varname:node_4487,prsc:2|IN-6668-OUT;n:type:ShaderForge.SFN_OneMinus,id:3056,x:32802,y:33282,varname:node_3056,prsc:2|IN-2619-OUT;n:type:ShaderForge.SFN_RemapRange,id:1933,x:32970,y:33282,varname:node_1933,prsc:2,frmn:0,frmx:0.2,tomn:0,tomx:1|IN-3056-OUT;n:type:ShaderForge.SFN_Clamp01,id:8276,x:33135,y:33282,varname:node_8276,prsc:2|IN-1933-OUT;n:type:ShaderForge.SFN_Clamp01,id:4861,x:33752,y:32911,varname:node_4861,prsc:2|IN-5955-OUT;n:type:ShaderForge.SFN_Clamp01,id:4134,x:33752,y:33051,varname:node_4134,prsc:0|IN-2624-OUT;n:type:ShaderForge.SFN_Vector1,id:3285,x:32905,y:33035,varname:node_3285,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:1209,x:32628,y:33419,varname:node_1209,prsc:2|A-4487-OUT,B-4487-OUT;n:type:ShaderForge.SFN_RemapRange,id:9185,x:33560,y:32750,varname:node_9185,prsc:2,frmn:0,frmx:1,tomn:0,tomx:1.5|IN-3394-OUT;n:type:ShaderForge.SFN_Sqrt,id:2619,x:32645,y:33246,varname:node_2619,prsc:2|IN-4487-OUT;proporder:9710-470;pass:END;sub:END;*/

Shader "Star/Flare" {
    Properties {
        _Color ("Color", Color) = (1,0.3970588,0.3970588,1)
        _Contrast ("Contrast", Range(0, 1)) = 0
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
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x xboxone ps4 
            #pragma target 3.0
            uniform fixed4 _Color;
            uniform fixed _Contrast;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                float4x4 bbmv = UNITY_MATRIX_MV;
                bbmv._m00 = -1.0/length(unity_WorldToObject[0].xyz);
                bbmv._m10 = 0.0f;
                bbmv._m20 = 0.0f;
                bbmv._m01 = 0.0f;
                bbmv._m11 = -1.0/length(unity_WorldToObject[1].xyz);
                bbmv._m21 = 0.0f;
                bbmv._m02 = 0.0f;
                bbmv._m12 = 0.0f;
                bbmv._m22 = -1.0/length(unity_WorldToObject[2].xyz);
                o.pos = mul( UNITY_MATRIX_P, mul( bbmv, v.vertex ));
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float node_4487 = length((i.uv0*2.0+-1.0));
                float node_3056 = (1.0 - sqrt(node_4487));
                fixed node_9843 = 0.0;
                float node_3394 = (1.0 - _Contrast);
                float3 emissive = saturate((_Color.rgb*(node_9843 + ( (node_3056 - node_9843) * (1.0 - node_9843) ) / ((node_3394*1.5+0.0) - node_9843))*saturate((node_3056*5.0+0.0))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
