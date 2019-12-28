// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:1,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:2,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:14,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:35310,y:32841,varname:node_9361,prsc:2|emission-1024-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:7651,x:33535,y:33006,ptovrint:False,ptlb:Corona,ptin:_Corona,varname:_Corona,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:298cdb3f26d4cd54eabcfca3ee0407e3,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2dAsset,id:3034,x:32629,y:32623,ptovrint:False,ptlb:Noise,ptin:_Noise,varname:_Noise,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:1664,x:34295,y:32993,varname:_node_1664,prsc:0,tex:298cdb3f26d4cd54eabcfca3ee0407e3,ntxv:0,isnm:False|UVIN-1329-OUT,TEX-7651-TEX;n:type:ShaderForge.SFN_TexCoord,id:9300,x:31994,y:32870,varname:node_9300,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ArcTan2,id:2029,x:32572,y:32865,varname:node_2029,prsc:0,attp:2|A-791-R,B-791-G;n:type:ShaderForge.SFN_RemapRange,id:4377,x:32191,y:32863,varname:node_4377,prsc:0,frmn:0,frmx:1,tomn:-1,tomx:1|IN-9300-UVOUT;n:type:ShaderForge.SFN_ComponentMask,id:791,x:32355,y:32810,varname:node_791,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-4377-OUT;n:type:ShaderForge.SFN_Length,id:9043,x:32610,y:33004,varname:node_9043,prsc:0|IN-4377-OUT;n:type:ShaderForge.SFN_Append,id:7454,x:33535,y:32804,varname:node_7454,prsc:2|A-7184-OUT,B-1770-OUT;n:type:ShaderForge.SFN_Add,id:1770,x:33146,y:32948,varname:node_1770,prsc:2|A-9043-OUT,B-6142-OUT,C-5105-OUT;n:type:ShaderForge.SFN_Vector1,id:6142,x:32337,y:33806,varname:node_6142,prsc:2,v1:0.7152757;n:type:ShaderForge.SFN_Floor,id:312,x:33277,y:33327,varname:node_312,prsc:2|IN-9043-OUT;n:type:ShaderForge.SFN_OneMinus,id:1624,x:33479,y:33327,varname:node_1624,prsc:2|IN-312-OUT;n:type:ShaderForge.SFN_Multiply,id:2764,x:34590,y:33096,varname:node_2764,prsc:2|A-1664-A,B-6620-OUT;n:type:ShaderForge.SFN_Tex2d,id:1867,x:32904,y:32560,varname:_node_1867,prsc:0,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-389-OUT,TEX-3034-TEX;n:type:ShaderForge.SFN_Append,id:389,x:32635,y:32420,varname:node_389,prsc:0|A-4089-OUT,B-2029-OUT;n:type:ShaderForge.SFN_Time,id:6379,x:31722,y:33192,varname:node_6379,prsc:0;n:type:ShaderForge.SFN_Multiply,id:5105,x:33145,y:32463,varname:node_5105,prsc:0|A-6485-OUT,B-1867-R,C-8096-OUT;n:type:ShaderForge.SFN_Vector1,id:6485,x:32909,y:32430,varname:node_6485,prsc:2,v1:0.25;n:type:ShaderForge.SFN_Slider,id:8096,x:32826,y:32310,ptovrint:False,ptlb:Waves,ptin:_Waves,varname:_Waves,prsc:0,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Sin,id:6322,x:33269,y:33633,varname:node_6322,prsc:0|IN-4646-OUT;n:type:ShaderForge.SFN_Add,id:4646,x:33085,y:33610,varname:node_4646,prsc:2|A-5397-OUT,B-9239-OUT,C-5174-OUT;n:type:ShaderForge.SFN_Multiply,id:9239,x:32997,y:33393,varname:node_9239,prsc:2|A-6993-OUT,B-9043-OUT;n:type:ShaderForge.SFN_Vector1,id:6993,x:32910,y:33313,varname:node_6993,prsc:2,v1:20;n:type:ShaderForge.SFN_Negate,id:1500,x:32676,y:33410,varname:node_1500,prsc:2|IN-4089-OUT;n:type:ShaderForge.SFN_Multiply,id:5397,x:32672,y:33261,varname:node_5397,prsc:2|A-4089-OUT,B-1143-OUT;n:type:ShaderForge.SFN_Vector1,id:1143,x:32474,y:33373,varname:node_1143,prsc:2,v1:-20;n:type:ShaderForge.SFN_Tex2d,id:7719,x:32890,y:33809,varname:_node_7719,prsc:0,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-4036-OUT,TEX-3034-TEX;n:type:ShaderForge.SFN_Multiply,id:5174,x:33136,y:33752,varname:node_5174,prsc:2|A-4787-OUT,B-7719-R;n:type:ShaderForge.SFN_Vector1,id:4787,x:32873,y:33693,varname:node_4787,prsc:2,v1:5;n:type:ShaderForge.SFN_Multiply,id:9045,x:33620,y:33613,varname:node_9045,prsc:0|A-6322-OUT,B-2553-OUT,C-155-OUT;n:type:ShaderForge.SFN_Vector1,id:2553,x:33453,y:33701,varname:node_2553,prsc:2,v1:0.03;n:type:ShaderForge.SFN_ComponentMask,id:4614,x:33861,y:32679,varname:node_4614,prsc:0,cc1:0,cc2:1,cc3:-1,cc4:-1|IN-7454-OUT;n:type:ShaderForge.SFN_Add,id:1815,x:34082,y:32699,varname:node_1815,prsc:2|A-6501-OUT,B-4614-R;n:type:ShaderForge.SFN_Add,id:6501,x:34003,y:32492,varname:node_6501,prsc:2|A-9045-OUT,B-4614-R;n:type:ShaderForge.SFN_Append,id:1329,x:34337,y:32711,varname:node_1329,prsc:0|A-1815-OUT,B-4614-G;n:type:ShaderForge.SFN_Slider,id:155,x:33375,y:33832,ptovrint:False,ptlb:Ripple,ptin:_Ripple,varname:_Ripple,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Append,id:4036,x:32716,y:33937,varname:node_4036,prsc:0|A-2029-OUT,B-9043-OUT;n:type:ShaderForge.SFN_Multiply,id:4089,x:32400,y:33174,varname:node_4089,prsc:2|A-2064-OUT,B-3630-OUT;n:type:ShaderForge.SFN_Slider,id:3630,x:32167,y:32162,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Color,id:1204,x:34741,y:32634,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2132353,c2:0.9023327,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:1024,x:35056,y:32822,varname:node_1024,prsc:2|A-1204-RGB,B-3549-OUT;n:type:ShaderForge.SFN_OneMinus,id:1435,x:33145,y:32611,varname:node_1435,prsc:2|IN-2029-OUT;n:type:ShaderForge.SFN_Max,id:27,x:33263,y:32307,varname:node_27,prsc:2|A-2029-OUT,B-1435-OUT;n:type:ShaderForge.SFN_Subtract,id:2861,x:33662,y:32390,varname:node_2861,prsc:2|A-27-OUT,B-3481-OUT;n:type:ShaderForge.SFN_Vector1,id:3481,x:33435,y:32463,varname:node_3481,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:3923,x:33435,y:32323,varname:node_3923,prsc:2|A-27-OUT,B-27-OUT,C-27-OUT,D-27-OUT;n:type:ShaderForge.SFN_RemapRange,id:7665,x:33831,y:32361,varname:node_7665,prsc:2,frmn:0,frmx:0.15,tomn:0,tomx:1|IN-2861-OUT;n:type:ShaderForge.SFN_Clamp01,id:4487,x:34046,y:32179,varname:node_4487,prsc:2|IN-7665-OUT;n:type:ShaderForge.SFN_Max,id:730,x:32956,y:32825,varname:node_730,prsc:2|A-4487-OUT,B-2029-OUT;n:type:ShaderForge.SFN_OneMinus,id:8396,x:34358,y:33541,varname:node_8396,prsc:2|IN-9043-OUT;n:type:ShaderForge.SFN_Add,id:1183,x:34779,y:33227,varname:node_1183,prsc:2|A-2764-OUT,B-9372-OUT;n:type:ShaderForge.SFN_RemapRange,id:9372,x:34986,y:33452,varname:node_9372,prsc:2,frmn:0,frmx:0.55,tomn:0,tomx:0.3|IN-1899-OUT;n:type:ShaderForge.SFN_Vector1,id:6406,x:34295,y:33445,varname:node_6406,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:1899,x:34804,y:33469,varname:node_1899,prsc:2|A-8396-OUT,B-8396-OUT,C-8396-OUT;n:type:ShaderForge.SFN_Clamp01,id:3549,x:34945,y:33200,varname:node_3549,prsc:2|IN-1183-OUT;n:type:ShaderForge.SFN_Vector4Property,id:3088,x:31228,y:33579,ptovrint:False,ptlb:ParentRotation,ptin:_ParentRotation,varname:_ParentRotation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Dot,id:654,x:31541,y:33528,varname:node_654,prsc:2,dt:0|A-9756-OUT,B-3088-XYZ;n:type:ShaderForge.SFN_NormalVector,id:9756,x:31323,y:33396,prsc:2,pt:False;n:type:ShaderForge.SFN_Add,id:2064,x:32129,y:33266,varname:node_2064,prsc:2|A-6379-TSL,B-654-OUT;n:type:ShaderForge.SFN_Add,id:7184,x:33322,y:32827,varname:node_7184,prsc:2|A-654-OUT,B-2029-OUT;n:type:ShaderForge.SFN_Multiply,id:8547,x:33063,y:32891,varname:node_8547,prsc:2|A-405-OUT,B-2064-OUT;n:type:ShaderForge.SFN_Vector1,id:405,x:32988,y:32754,varname:node_405,prsc:2,v1:0.1;n:type:ShaderForge.SFN_RemapRange,id:383,x:33150,y:33170,varname:node_383,prsc:2,frmn:0.8,frmx:1,tomn:1,tomx:0|IN-9043-OUT;n:type:ShaderForge.SFN_Clamp01,id:6620,x:33393,y:33167,varname:node_6620,prsc:2|IN-383-OUT;proporder:7651-3034-8096-155-3630-1204-3088;pass:END;sub:END;*/

Shader "Star/Corona simple" {
    Properties {
        _Corona ("Corona", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _Waves ("Waves", Range(0, 1)) = 1
        _Ripple ("Ripple", Range(0, 1)) = 1
        _Speed ("Speed", Range(0, 1)) = 1
        _Color ("Color", Color) = (0.2132353,0.9023327,1,1)
        _ParentRotation ("ParentRotation", Vector) = (0,0,0,0)
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "DisableBatching"="True"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            ZWrite Off
            ColorMask RGB
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Corona; uniform float4 _Corona_ST;
            uniform sampler2D _Noise; uniform float4 _Noise_ST;
            uniform fixed _Waves;
            uniform float _Ripple;
            uniform float _Speed;
            uniform float4 _Color;
            uniform float4 _ParentRotation;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
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
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
////// Lighting:
////// Emissive:
                fixed4 node_6379 = _Time + _TimeEditor;
                float node_654 = dot(i.normalDir,_ParentRotation.rgb);
                float node_2064 = (node_6379.r+node_654);
                float node_4089 = (node_2064*_Speed);
                fixed2 node_4377 = (i.uv0*2.0+-1.0);
                fixed node_9043 = length(node_4377);
                fixed2 node_791 = node_4377.rg;
                fixed node_2029 = ((atan2(node_791.r,node_791.g)/6.28318530718)+0.5);
                fixed2 node_4036 = float2(node_2029,node_9043);
                fixed4 _node_7719 = tex2D(_Noise,TRANSFORM_TEX(node_4036, _Noise));
                fixed2 node_389 = float2(node_4089,node_2029);
                fixed4 _node_1867 = tex2D(_Noise,TRANSFORM_TEX(node_389, _Noise));
                fixed2 node_4614 = float2((node_654+node_2029),(node_9043+0.7152757+(0.25*_node_1867.r*_Waves))).rg;
                fixed2 node_1329 = float2((((sin(((node_4089*(-20.0))+(20.0*node_9043)+(5.0*_node_7719.r)))*0.03*_Ripple)+node_4614.r)+node_4614.r),node_4614.g);
                fixed4 _node_1664 = tex2D(_Corona,TRANSFORM_TEX(node_1329, _Corona));
                float node_8396 = (1.0 - node_9043);
                float3 emissive = (_Color.rgb*saturate(((_node_1664.a*saturate((node_9043*-5.0+5.0)))+((node_8396*node_8396*node_8396)*0.5454546+0.0))));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
