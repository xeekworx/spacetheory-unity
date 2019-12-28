// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:0,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:False,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:6,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:False,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:37694,y:34401,varname:node_9361,prsc:2|emission-4376-OUT;n:type:ShaderForge.SFN_Tex2dAsset,id:6109,x:32891,y:34859,ptovrint:False,ptlb:Main,ptin:_Main,varname:_Main,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:2001,x:31261,y:33855,varname:node_2001,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_RemapRange,id:9283,x:31432,y:33855,varname:node_9283,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-2001-UVOUT;n:type:ShaderForge.SFN_Length,id:7103,x:31628,y:33855,varname:node_7103,prsc:2|IN-9283-OUT;n:type:ShaderForge.SFN_OneMinus,id:7238,x:31797,y:33855,varname:node_7238,prsc:2|IN-7103-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:1223,x:33367,y:33859,varname:node_1223,prsc:2|IN-7238-OUT,IMIN-2090-OUT,IMAX-4520-OUT,OMIN-2090-OUT,OMAX-4911-OUT;n:type:ShaderForge.SFN_Vector1,id:2090,x:33004,y:33963,varname:node_2090,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:4911,x:33004,y:34072,varname:node_4911,prsc:2,v1:1;n:type:ShaderForge.SFN_Slider,id:4520,x:32799,y:33879,ptovrint:False,ptlb:Border,ptin:_Border,varname:_Border,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.2558521,max:1;n:type:ShaderForge.SFN_Clamp01,id:5776,x:33556,y:33859,varname:node_5776,prsc:2|IN-1223-OUT;n:type:ShaderForge.SFN_RemapRangeAdvanced,id:5157,x:33436,y:33635,varname:node_5157,prsc:2|IN-7238-OUT,IMIN-774-OUT,IMAX-6257-OUT,OMIN-2090-OUT,OMAX-7747-OUT;n:type:ShaderForge.SFN_Slider,id:4712,x:32817,y:33643,ptovrint:False,ptlb:InnerRadius,ptin:_InnerRadius,varname:_InnerRadius,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.288689,max:1;n:type:ShaderForge.SFN_Vector1,id:6257,x:33159,y:33584,varname:node_6257,prsc:2,v1:1;n:type:ShaderForge.SFN_OneMinus,id:774,x:33159,y:33640,varname:node_774,prsc:2|IN-4712-OUT;n:type:ShaderForge.SFN_Clamp01,id:6970,x:33616,y:33635,varname:node_6970,prsc:2|IN-5157-OUT;n:type:ShaderForge.SFN_RemapRange,id:5524,x:33811,y:33633,varname:node_5524,prsc:2,frmn:0,frmx:0.5,tomn:0,tomx:1|IN-6970-OUT;n:type:ShaderForge.SFN_Clamp01,id:5262,x:34034,y:33623,varname:node_5262,prsc:2|IN-5524-OUT;n:type:ShaderForge.SFN_OneMinus,id:1468,x:34228,y:33624,varname:node_1468,prsc:2|IN-5262-OUT;n:type:ShaderForge.SFN_Multiply,id:4249,x:34489,y:33703,varname:node_4249,prsc:2|A-1468-OUT,B-5776-OUT;n:type:ShaderForge.SFN_Multiply,id:1730,x:32732,y:34575,varname:node_1730,prsc:2|A-8711-OUT,B-1951-OUT;n:type:ShaderForge.SFN_Multiply,id:1852,x:32187,y:34138,varname:node_1852,prsc:2|A-8299-OUT,B-7238-OUT,C-8411-OUT;n:type:ShaderForge.SFN_Slider,id:8299,x:31871,y:33699,ptovrint:False,ptlb:Twist,ptin:_Twist,varname:_Twist,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.9378453,max:1;n:type:ShaderForge.SFN_Vector1,id:8411,x:31946,y:34230,varname:node_8411,prsc:2,v1:10;n:type:ShaderForge.SFN_Color,id:6891,x:37413,y:32811,ptovrint:False,ptlb:Color3,ptin:_Color3,varname:_Color3,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.7732251,c3:0.3676471,c4:1;n:type:ShaderForge.SFN_Slider,id:6210,x:31891,y:33180,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:_Speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.219689,max:1;n:type:ShaderForge.SFN_Set,id:4361,x:32256,y:33179,varname:Speed,prsc:2|IN-7930-OUT;n:type:ShaderForge.SFN_Get,id:7328,x:33649,y:34431,varname:node_7328,prsc:2|IN-4361-OUT;n:type:ShaderForge.SFN_Time,id:2936,x:33649,y:34480,varname:node_2936,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1000,x:33836,y:34406,varname:node_1000,prsc:2|A-7328-OUT,B-2936-T;n:type:ShaderForge.SFN_Vector1,id:8711,x:32550,y:34599,varname:node_8711,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Tex2d,id:9214,x:33183,y:34301,varname:_node_9214,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9089-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Multiply,id:1067,x:33470,y:34273,varname:node_1067,prsc:2|A-9214-R,B-6380-OUT;n:type:ShaderForge.SFN_Vector1,id:6380,x:33195,y:34475,varname:node_6380,prsc:2,v1:0.9;n:type:ShaderForge.SFN_Multiply,id:8567,x:32746,y:34078,varname:node_8567,prsc:2|A-7238-OUT,B-8921-OUT;n:type:ShaderForge.SFN_Vector1,id:8921,x:32586,y:34112,varname:node_8921,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:865,x:34305,y:34430,varname:_node_865,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-1987-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Append,id:6757,x:33703,y:34237,varname:node_6757,prsc:2|A-1067-OUT,B-3915-OUT;n:type:ShaderForge.SFN_TexCoord,id:9472,x:32408,y:34138,varname:node_9472,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Rotator,id:9089,x:32879,y:34257,varname:node_9089,prsc:2|UVIN-7987-UVOUT,SPD-1730-OUT;n:type:ShaderForge.SFN_Panner,id:1987,x:34160,y:34181,varname:node_1987,prsc:2,spu:-0.3,spv:-0.2|UVIN-6757-OUT,DIST-4956-OUT;n:type:ShaderForge.SFN_Multiply,id:3915,x:33256,y:34086,varname:node_3915,prsc:2|A-8567-OUT,B-1278-OUT,C-8299-OUT;n:type:ShaderForge.SFN_Vector1,id:1278,x:33004,y:34132,varname:node_1278,prsc:2,v1:2;n:type:ShaderForge.SFN_Rotator,id:7987,x:32633,y:34237,varname:node_7987,prsc:2|UVIN-9472-UVOUT,ANG-9339-OUT;n:type:ShaderForge.SFN_Multiply,id:9339,x:32408,y:34286,varname:node_9339,prsc:2|A-1852-OUT,B-4947-OUT;n:type:ShaderForge.SFN_Vector1,id:4947,x:32187,y:34320,varname:node_4947,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Get,id:1951,x:32529,y:34648,varname:node_1951,prsc:2|IN-4361-OUT;n:type:ShaderForge.SFN_Vector1,id:7747,x:33159,y:33829,varname:node_7747,prsc:2,v1:1;n:type:ShaderForge.SFN_Tex2d,id:8043,x:33222,y:34988,varname:_node_7869,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9089-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Multiply,id:586,x:33549,y:34960,varname:node_586,prsc:2|A-8043-R,B-3900-OUT;n:type:ShaderForge.SFN_Vector1,id:3900,x:33234,y:35162,varname:node_3900,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Tex2d,id:370,x:34324,y:34952,varname:_node_407,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-4864-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Append,id:8265,x:33742,y:34924,varname:node_8265,prsc:2|A-586-OUT,B-7932-OUT;n:type:ShaderForge.SFN_Panner,id:4864,x:34139,y:34959,varname:node_4864,prsc:2,spu:-0.25,spv:-0.15|UVIN-8265-OUT,DIST-4956-OUT;n:type:ShaderForge.SFN_Multiply,id:7932,x:33549,y:34703,varname:node_7932,prsc:2|A-3915-OUT,B-5429-OUT;n:type:ShaderForge.SFN_Vector1,id:5429,x:33302,y:34768,varname:node_5429,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:1159,x:34560,y:34700,varname:node_1159,prsc:2,v1:0.75;n:type:ShaderForge.SFN_Add,id:6338,x:34582,y:34566,varname:node_6338,prsc:2|A-865-R,B-9286-OUT;n:type:ShaderForge.SFN_Multiply,id:5406,x:34830,y:34667,varname:node_5406,prsc:2|A-6338-OUT,B-1159-OUT;n:type:ShaderForge.SFN_Tex2d,id:3142,x:34310,y:35284,varname:_node_4971,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-4068-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Panner,id:4068,x:34114,y:35236,varname:node_4068,prsc:2,spu:-0.2,spv:-0.1|UVIN-6703-OUT,DIST-4956-OUT;n:type:ShaderForge.SFN_Append,id:6703,x:33728,y:35256,varname:node_6703,prsc:2|A-4065-OUT,B-7932-OUT;n:type:ShaderForge.SFN_Multiply,id:4065,x:33535,y:35292,varname:node_4065,prsc:2|A-6756-R,B-676-OUT;n:type:ShaderForge.SFN_Tex2d,id:6756,x:33208,y:35320,varname:_node_247,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9089-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Vector1,id:676,x:33220,y:35494,varname:node_676,prsc:2,v1:0.3;n:type:ShaderForge.SFN_OneMinus,id:9390,x:34667,y:33689,varname:node_9390,prsc:2|IN-4249-OUT;n:type:ShaderForge.SFN_Multiply,id:7930,x:32172,y:33321,varname:node_7930,prsc:2|A-6210-OUT,B-7961-OUT;n:type:ShaderForge.SFN_Vector1,id:7961,x:31891,y:33404,varname:node_7961,prsc:2,v1:0.3;n:type:ShaderForge.SFN_Color,id:3465,x:35932,y:34445,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.3676471,c3:0.3676471,c4:1;n:type:ShaderForge.SFN_NormalVector,id:5988,x:35443,y:34336,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:789,x:35708,y:34314,varname:node_789,prsc:2,dt:0|A-5988-OUT,B-5809-OUT;n:type:ShaderForge.SFN_ViewVector,id:5809,x:35450,y:34204,varname:node_5809,prsc:2;n:type:ShaderForge.SFN_Add,id:4956,x:34083,y:34551,varname:node_4956,prsc:2|A-1000-OUT,B-1491-OUT;n:type:ShaderForge.SFN_Vector4Property,id:2055,x:33823,y:34637,ptovrint:False,ptlb:ParentRotation,ptin:_ParentRotation,varname:_ParentRotation,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0,v2:0,v3:0,v4:0;n:type:ShaderForge.SFN_Dot,id:1204,x:34063,y:34727,varname:node_1204,prsc:2,dt:0|A-2055-XYZ,B-5343-OUT;n:type:ShaderForge.SFN_Multiply,id:1491,x:34269,y:34712,varname:node_1491,prsc:2|A-1204-OUT,B-729-OUT;n:type:ShaderForge.SFN_Vector1,id:729,x:34113,y:34911,varname:node_729,prsc:2,v1:50;n:type:ShaderForge.SFN_Tangent,id:5343,x:33809,y:34843,varname:node_5343,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4600,x:35930,y:34806,varname:node_4600,prsc:2|A-4249-OUT,B-865-R,C-7168-OUT,D-6905-OUT,E-9286-OUT;n:type:ShaderForge.SFN_OneMinus,id:9286,x:34564,y:35154,varname:node_9286,prsc:2|IN-3142-R;n:type:ShaderForge.SFN_OneMinus,id:6905,x:35639,y:35032,varname:node_6905,prsc:2|IN-370-R;n:type:ShaderForge.SFN_OneMinus,id:8583,x:35302,y:34883,varname:node_8583,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4159,x:36142,y:34594,varname:node_4159,prsc:2|A-3465-A,B-4600-OUT;n:type:ShaderForge.SFN_Multiply,id:416,x:36450,y:34118,varname:node_416,prsc:2|A-3465-RGB,B-4159-OUT;n:type:ShaderForge.SFN_Multiply,id:3836,x:35988,y:34179,varname:node_3836,prsc:2|A-6720-OUT,B-789-OUT;n:type:ShaderForge.SFN_Vector1,id:6720,x:35802,y:34090,varname:node_6720,prsc:2,v1:20;n:type:ShaderForge.SFN_Clamp01,id:5412,x:36201,y:33910,varname:node_5412,prsc:2|IN-3836-OUT;n:type:ShaderForge.SFN_Vector1,id:7168,x:35233,y:34624,varname:node_7168,prsc:2,v1:1;n:type:ShaderForge.SFN_RemapRange,id:2482,x:33866,y:33991,varname:node_2482,prsc:2,frmn:0.85,frmx:1,tomn:0,tomx:1|IN-1302-R;n:type:ShaderForge.SFN_Clamp01,id:6498,x:36003,y:35054,varname:node_6498,prsc:2|IN-2482-OUT;n:type:ShaderForge.SFN_Panner,id:9451,x:33448,y:33978,varname:node_9451,prsc:2,spu:-0.1,spv:-0.1|UVIN-9089-UVOUT,DIST-4956-OUT;n:type:ShaderForge.SFN_Tex2d,id:1302,x:33610,y:34061,varname:_node_1302,prsc:2,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-9451-UVOUT,TEX-6109-TEX;n:type:ShaderForge.SFN_Multiply,id:1438,x:36377,y:34951,varname:node_1438,prsc:2|A-6498-OUT,B-7760-RGB,C-4249-OUT;n:type:ShaderForge.SFN_Color,id:7760,x:36241,y:35133,ptovrint:False,ptlb:Color2,ptin:_Color2,varname:_Color2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.959432,c2:1,c3:0.2647059,c4:1;n:type:ShaderForge.SFN_Blend,id:6894,x:36580,y:34348,varname:node_6894,prsc:2,blmd:2,clmp:True|SRC-416-OUT,DST-1438-OUT;n:type:ShaderForge.SFN_Append,id:1606,x:36607,y:34972,varname:node_1606,prsc:2|A-7760-RGB,B-6498-OUT;n:type:ShaderForge.SFN_Append,id:9290,x:36328,y:34365,varname:node_9290,prsc:2|A-3465-RGB,B-4159-OUT;n:type:ShaderForge.SFN_Lerp,id:1958,x:36669,y:34207,varname:node_1958,prsc:2|A-416-OUT,B-1438-OUT,T-6498-OUT;n:type:ShaderForge.SFN_Lerp,id:5471,x:37002,y:33968,varname:node_5471,prsc:2|A-2228-OUT,B-3465-RGB,T-4159-OUT;n:type:ShaderForge.SFN_Multiply,id:925,x:36846,y:34112,varname:node_925,prsc:2|A-3127-OUT,B-4159-OUT;n:type:ShaderForge.SFN_Vector1,id:3127,x:36663,y:34128,varname:node_3127,prsc:2,v1:2;n:type:ShaderForge.SFN_Vector1,id:2228,x:36634,y:33919,varname:node_2228,prsc:2,v1:0;n:type:ShaderForge.SFN_Lerp,id:4376,x:37570,y:33994,varname:node_4376,prsc:2|A-5471-OUT,B-7760-RGB,T-8618-OUT;n:type:ShaderForge.SFN_Subtract,id:4601,x:37163,y:34085,varname:node_4601,prsc:2|A-925-OUT,B-866-OUT;n:type:ShaderForge.SFN_Vector1,id:866,x:36917,y:34390,varname:node_866,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Clamp01,id:8618,x:37389,y:34055,varname:node_8618,prsc:2|IN-4601-OUT;proporder:6109-4520-4712-8299-6210-3465-2055-7760;pass:END;sub:END;*/

Shader "Star/Accresion Disk" {
    Properties {
        _Main ("Main", 2D) = "white" {}
        _Border ("Border", Range(0, 1)) = 0.2558521
        _InnerRadius ("InnerRadius", Range(0, 1)) = 0.288689
        _Twist ("Twist", Range(0, 1)) = 0.9378453
        _Speed ("Speed", Range(0, 1)) = 0.219689
        [HDR]_Color ("Color", Color) = (1,0.3676471,0.3676471,1)
        _ParentRotation ("ParentRotation", Vector) = (0,0,0,0)
        _Color2 ("Color2", Color) = (0.959432,1,0.2647059,1)
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
            Blend One OneMinusSrcColor
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 xboxone ps4 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Main; uniform float4 _Main_ST;
            uniform float _Border;
            uniform float _InnerRadius;
            uniform float _Twist;
            uniform float _Speed;
            uniform float4 _Color;
            uniform float4 _ParentRotation;
            uniform float4 _Color2;
            struct VertexInput {
                float4 vertex : POSITION;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float3 tangentDir : TEXCOORD1;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_2228 = 0.0;
                float node_7238 = (1.0 - length((i.uv0*2.0+-1.0)));
                float node_774 = (1.0 - _InnerRadius);
                float node_2090 = 0.0;
                float node_4249 = ((1.0 - saturate((saturate((node_2090 + ( (node_7238 - node_774) * (1.0 - node_2090) ) / (1.0 - node_774)))*2.0+0.0)))*saturate((node_2090 + ( (node_7238 - node_2090) * (1.0 - node_2090) ) / (_Border - node_2090))));
                float Speed = (_Speed*0.3);
                float4 node_2936 = _Time + _TimeEditor;
                float node_4956 = ((Speed*node_2936.g)+(dot(_ParentRotation.rgb,i.tangentDir)*50.0));
                float4 node_1454 = _Time + _TimeEditor;
                float node_9089_ang = node_1454.g;
                float node_9089_spd = (0.5*Speed);
                float node_9089_cos = cos(node_9089_spd*node_9089_ang);
                float node_9089_sin = sin(node_9089_spd*node_9089_ang);
                float2 node_9089_piv = float2(0.5,0.5);
                float node_7987_ang = ((_Twist*node_7238*10.0)*0.3);
                float node_7987_spd = 1.0;
                float node_7987_cos = cos(node_7987_spd*node_7987_ang);
                float node_7987_sin = sin(node_7987_spd*node_7987_ang);
                float2 node_7987_piv = float2(0.5,0.5);
                float2 node_7987 = (mul(i.uv0-node_7987_piv,float2x2( node_7987_cos, -node_7987_sin, node_7987_sin, node_7987_cos))+node_7987_piv);
                float2 node_9089 = (mul(node_7987-node_9089_piv,float2x2( node_9089_cos, -node_9089_sin, node_9089_sin, node_9089_cos))+node_9089_piv);
                float4 _node_9214 = tex2D(_Main,TRANSFORM_TEX(node_9089, _Main));
                float node_3915 = ((node_7238*2.0)*2.0*_Twist);
                float2 node_1987 = (float2((_node_9214.r*0.9),node_3915)+node_4956*float2(-0.3,-0.2));
                float4 _node_865 = tex2D(_Main,TRANSFORM_TEX(node_1987, _Main));
                float4 _node_7869 = tex2D(_Main,TRANSFORM_TEX(node_9089, _Main));
                float node_7932 = (node_3915*0.3);
                float2 node_4864 = (float2((_node_7869.r*0.4),node_7932)+node_4956*float2(-0.25,-0.15));
                float4 _node_407 = tex2D(_Main,TRANSFORM_TEX(node_4864, _Main));
                float4 _node_247 = tex2D(_Main,TRANSFORM_TEX(node_9089, _Main));
                float2 node_4068 = (float2((_node_247.r*0.3),node_7932)+node_4956*float2(-0.2,-0.1));
                float4 _node_4971 = tex2D(_Main,TRANSFORM_TEX(node_4068, _Main));
                float node_9286 = (1.0 - _node_4971.r);
                float node_4159 = (_Color.a*(node_4249*_node_865.r*1.0*(1.0 - _node_407.r)*node_9286));
                float3 emissive = lerp(lerp(float3(node_2228,node_2228,node_2228),_Color.rgb,node_4159),_Color2.rgb,saturate(((2.0*node_4159)-0.5)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
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
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 xboxone ps4 
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
