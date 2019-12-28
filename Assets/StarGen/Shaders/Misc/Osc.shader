// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:1,cusa:True,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:True,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:1,stmr:255,stmw:255,stcp:2,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:1873,x:34697,y:32399,varname:node_1873,prsc:2|emission-6046-OUT,alpha-8480-OUT;n:type:ShaderForge.SFN_TexCoord,id:2988,x:31826,y:32585,varname:node_2988,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Multiply,id:28,x:32478,y:32543,varname:node_28,prsc:2|A-9059-OUT,B-5900-OUT,C-6390-OUT;n:type:ShaderForge.SFN_Vector1,id:9059,x:32282,y:32483,varname:node_9059,prsc:2,v1:5;n:type:ShaderForge.SFN_Pi,id:6390,x:32272,y:32753,varname:node_6390,prsc:2;n:type:ShaderForge.SFN_Add,id:7522,x:32717,y:32464,varname:node_7522,prsc:2|A-9196-TDB,B-28-OUT;n:type:ShaderForge.SFN_Time,id:9196,x:32478,y:32000,varname:node_9196,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:2216,x:32478,y:32686,varname:node_2216,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:2|IN-2988-V;n:type:ShaderForge.SFN_Sin,id:5543,x:32861,y:32484,varname:node_5543,prsc:2|IN-7522-OUT;n:type:ShaderForge.SFN_RemapRange,id:6243,x:33472,y:32759,varname:node_6243,prsc:2,frmn:0,frmx:0.55,tomn:0,tomx:1|IN-7854-OUT;n:type:ShaderForge.SFN_RemapRange,id:3905,x:33472,y:32590,varname:node_3905,prsc:2,frmn:0.45,frmx:1,tomn:0,tomx:1|IN-7854-OUT;n:type:ShaderForge.SFN_Clamp01,id:7572,x:33661,y:32596,varname:node_7572,prsc:2|IN-3905-OUT;n:type:ShaderForge.SFN_Clamp01,id:9043,x:33651,y:32764,varname:node_9043,prsc:2|IN-6243-OUT;n:type:ShaderForge.SFN_Subtract,id:9633,x:33818,y:32719,varname:node_9633,prsc:2|A-9043-OUT,B-7572-OUT;n:type:ShaderForge.SFN_Add,id:4530,x:32741,y:32250,varname:node_4530,prsc:2|A-7676-OUT,B-28-OUT,C-7869-OUT;n:type:ShaderForge.SFN_Sin,id:2104,x:32905,y:32250,varname:node_2104,prsc:2|IN-4530-OUT;n:type:ShaderForge.SFN_Multiply,id:3832,x:33089,y:32522,varname:node_3832,prsc:2|A-2104-OUT,B-5543-OUT,C-6083-OUT;n:type:ShaderForge.SFN_Multiply,id:7869,x:32492,y:32274,varname:node_7869,prsc:2|A-4486-OUT,B-5900-OUT,C-6390-OUT;n:type:ShaderForge.SFN_Slider,id:4486,x:32162,y:32149,ptovrint:False,ptlb:Shape2,ptin:_Shape2,varname:_Shape2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:6,max:6;n:type:ShaderForge.SFN_Add,id:7854,x:33291,y:32421,varname:node_7854,prsc:2|A-3832-OUT,B-2216-OUT;n:type:ShaderForge.SFN_Multiply,id:7676,x:32677,y:32126,varname:node_7676,prsc:2|A-6191-OUT,B-9196-T;n:type:ShaderForge.SFN_Slider,id:6191,x:32421,y:31865,ptovrint:False,ptlb:Freq2,ptin:_Freq2,varname:_Freq2,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Multiply,id:5900,x:32091,y:32483,varname:node_5900,prsc:2|A-2805-R,B-2988-U;n:type:ShaderForge.SFN_ObjectScale,id:4190,x:31608,y:32394,varname:node_4190,prsc:2,rcp:False;n:type:ShaderForge.SFN_Transform,id:2674,x:31808,y:32394,varname:node_2674,prsc:2,tffrom:0,tfto:1|IN-4190-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:2805,x:31980,y:32301,varname:node_2805,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-2674-XYZ;n:type:ShaderForge.SFN_Multiply,id:4267,x:32460,y:32929,varname:node_4267,prsc:2|A-2988-U,B-6390-OUT;n:type:ShaderForge.SFN_Sin,id:6083,x:32829,y:32772,varname:node_6083,prsc:2|IN-4267-OUT;n:type:ShaderForge.SFN_OneMinus,id:1386,x:33472,y:32917,varname:node_1386,prsc:2|IN-6083-OUT;n:type:ShaderForge.SFN_Subtract,id:9340,x:33959,y:32889,varname:node_9340,prsc:2|A-9633-OUT,B-7034-OUT;n:type:ShaderForge.SFN_Multiply,id:7034,x:33693,y:32931,varname:node_7034,prsc:2|A-1386-OUT,B-2107-OUT;n:type:ShaderForge.SFN_Vector1,id:2107,x:33636,y:33153,varname:node_2107,prsc:2,v1:1;n:type:ShaderForge.SFN_Color,id:2221,x:34129,y:32640,ptovrint:False,ptlb:Color,ptin:_Color,varname:_Color,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.2352941,c3:0.2352941,c4:1;n:type:ShaderForge.SFN_Multiply,id:6046,x:34291,y:32764,varname:node_6046,prsc:2|A-2221-RGB,B-5313-OUT;n:type:ShaderForge.SFN_RemapRange,id:274,x:33502,y:32009,varname:node_274,prsc:2,frmn:0.3,frmx:1,tomn:0,tomx:1|IN-361-OUT;n:type:ShaderForge.SFN_Clamp01,id:8299,x:33691,y:32015,varname:node_8299,prsc:2|IN-274-OUT;n:type:ShaderForge.SFN_Add,id:2249,x:32710,y:31449,varname:node_2249,prsc:2|A-5733-TDB,B-3896-OUT;n:type:ShaderForge.SFN_Sin,id:3615,x:32854,y:31469,varname:node_3615,prsc:2|IN-2249-OUT;n:type:ShaderForge.SFN_Add,id:4158,x:32734,y:31235,varname:node_4158,prsc:2|A-5846-OUT,B-3896-OUT,C-9975-OUT;n:type:ShaderForge.SFN_Sin,id:3281,x:32898,y:31235,varname:node_3281,prsc:2|IN-4158-OUT;n:type:ShaderForge.SFN_Multiply,id:132,x:33082,y:31507,varname:node_132,prsc:2|A-3281-OUT,B-3615-OUT,C-6083-OUT;n:type:ShaderForge.SFN_Add,id:361,x:33324,y:31880,varname:node_361,prsc:2|A-132-OUT,B-7372-OUT;n:type:ShaderForge.SFN_Pi,id:9982,x:32265,y:31738,varname:node_9982,prsc:2;n:type:ShaderForge.SFN_RemapRange,id:7372,x:32471,y:31671,varname:node_7372,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:4|IN-1322-V;n:type:ShaderForge.SFN_Multiply,id:3896,x:32471,y:31528,varname:node_3896,prsc:2|A-9775-OUT,B-2967-OUT,C-9982-OUT;n:type:ShaderForge.SFN_Multiply,id:2967,x:32084,y:31468,varname:node_2967,prsc:2|A-6983-R,B-1322-U;n:type:ShaderForge.SFN_Vector1,id:9775,x:32275,y:31468,varname:node_9775,prsc:2,v1:5;n:type:ShaderForge.SFN_TexCoord,id:1322,x:31819,y:31570,varname:node_1322,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Transform,id:338,x:31801,y:31379,varname:node_338,prsc:2,tffrom:0,tfto:1|IN-428-XYZ;n:type:ShaderForge.SFN_ComponentMask,id:6983,x:31973,y:31286,varname:node_6983,prsc:2,cc1:0,cc2:1,cc3:2,cc4:-1|IN-338-XYZ;n:type:ShaderForge.SFN_ObjectScale,id:428,x:31601,y:31379,varname:node_428,prsc:2,rcp:False;n:type:ShaderForge.SFN_Slider,id:8486,x:32155,y:31134,ptovrint:False,ptlb:Shape1,ptin:_Shape1,varname:_Shape1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:6;n:type:ShaderForge.SFN_Time,id:5733,x:32451,y:31163,varname:node_5733,prsc:2;n:type:ShaderForge.SFN_Slider,id:7312,x:32414,y:30850,ptovrint:False,ptlb:Freq1,ptin:_Freq1,varname:_Freq1,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:5,max:5;n:type:ShaderForge.SFN_Multiply,id:5846,x:32670,y:31111,varname:node_5846,prsc:2|A-7312-OUT,B-5733-T;n:type:ShaderForge.SFN_Subtract,id:8348,x:34047,y:32213,varname:node_8348,prsc:2|A-5403-OUT,B-7034-OUT;n:type:ShaderForge.SFN_Add,id:3132,x:34062,y:32394,varname:node_3132,prsc:2|A-8348-OUT,B-9340-OUT,C-4915-OUT;n:type:ShaderForge.SFN_Multiply,id:9975,x:32471,y:31361,varname:node_9975,prsc:2|A-2967-OUT,B-9982-OUT,C-8486-OUT;n:type:ShaderForge.SFN_Multiply,id:5313,x:34344,y:32358,varname:node_5313,prsc:2|A-2483-OUT,B-3132-OUT;n:type:ShaderForge.SFN_Vector1,id:2483,x:34473,y:32225,varname:node_2483,prsc:2,v1:1.5;n:type:ShaderForge.SFN_Clamp01,id:8480,x:34297,y:32489,varname:node_8480,prsc:2|IN-3132-OUT;n:type:ShaderForge.SFN_Multiply,id:8394,x:32266,y:33024,varname:node_8394,prsc:2|A-2988-V,B-6390-OUT;n:type:ShaderForge.SFN_Sin,id:7827,x:33144,y:33111,varname:node_7827,prsc:2|IN-8394-OUT;n:type:ShaderForge.SFN_Multiply,id:4915,x:34071,y:33018,varname:node_4915,prsc:2|A-6083-OUT,B-7827-OUT,C-5059-OUT;n:type:ShaderForge.SFN_Subtract,id:2053,x:33854,y:33354,varname:node_2053,prsc:2|A-6285-OUT,B-5059-OUT;n:type:ShaderForge.SFN_Clamp01,id:6285,x:33687,y:33399,varname:node_6285,prsc:2|IN-1539-OUT;n:type:ShaderForge.SFN_RemapRange,id:1539,x:33508,y:33394,varname:node_1539,prsc:2,frmn:0,frmx:0.95,tomn:0,tomx:1|IN-7854-OUT;n:type:ShaderForge.SFN_Clamp01,id:5059,x:33697,y:33231,varname:node_5059,prsc:2|IN-7018-OUT;n:type:ShaderForge.SFN_RemapRange,id:7018,x:33508,y:33225,varname:node_7018,prsc:2,frmn:0.05,frmx:1,tomn:0,tomx:1|IN-7854-OUT;n:type:ShaderForge.SFN_OneMinus,id:8803,x:33982,y:33238,varname:node_8803,prsc:2|IN-5059-OUT;n:type:ShaderForge.SFN_Multiply,id:5403,x:33836,y:32373,varname:node_5403,prsc:2|A-4395-OUT,B-6083-OUT,C-7827-OUT;n:type:ShaderForge.SFN_OneMinus,id:4395,x:33945,y:31952,varname:node_4395,prsc:2|IN-8299-OUT;proporder:4486-6191-2221-7312-8486;pass:END;sub:END;*/

Shader "UI/Osc" {
    Properties {
        _Shape2 ("Shape2", Range(0, 6)) = 6
        _Freq2 ("Freq2", Range(0, 5)) = 5
        _Color ("Color", Color) = (1,0.2352941,0.2352941,1)
        _Freq1 ("Freq1", Range(0, 5)) = 5
        _Shape1 ("Shape1", Range(0, 6)) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
            "PreviewType"="Plane"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal xboxone ps4 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float _Shape2;
            uniform float _Freq2;
            uniform float4 _Color;
            uniform float _Shape1;
            uniform float _Freq1;
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
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
                o.pos = UnityObjectToClipPos(v.vertex );
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float3 recipObjScale = float3( length(unity_WorldToObject[0].xyz), length(unity_WorldToObject[1].xyz), length(unity_WorldToObject[2].xyz) );
                float3 objScale = 1.0/recipObjScale;
////// Lighting:
////// Emissive:
                float4 node_5733 = _Time + _TimeEditor;
                float node_2967 = (mul( unity_WorldToObject, float4(objScale,0) ).xyz.rgb.rgb.r*i.uv0.r);
                float node_9982 = 3.141592654;
                float node_3896 = (5.0*node_2967*node_9982);
                float node_6390 = 3.141592654;
                float node_6083 = sin((i.uv0.r*node_6390));
                float node_7827 = sin((i.uv0.g*node_6390));
                float node_7034 = ((1.0 - node_6083)*1.0);
                float4 node_9196 = _Time + _TimeEditor;
                float node_5900 = (mul( unity_WorldToObject, float4(objScale,0) ).xyz.rgb.rgb.r*i.uv0.r);
                float node_28 = (5.0*node_5900*node_6390);
                float node_7854 = ((sin(((_Freq2*node_9196.g)+node_28+(_Shape2*node_5900*node_6390)))*sin((node_9196.b+node_28))*node_6083)+(i.uv0.g*3.0+-1.0));
                float node_5059 = saturate((node_7854*1.052632+-0.05263158));
                float node_3132 = ((((1.0 - saturate((((sin(((_Freq1*node_5733.g)+node_3896+(node_2967*node_9982*_Shape1)))*sin((node_5733.b+node_3896))*node_6083)+(i.uv0.g*5.0+-1.0))*1.428571+-0.4285715)))*node_6083*node_7827)-node_7034)+((saturate((node_7854*1.818182+0.0))-saturate((node_7854*1.818182+-0.8181818)))-node_7034)+(node_6083*node_7827*node_5059));
                float3 emissive = (_Color.rgb*(1.5*node_3132));
                float3 finalColor = emissive;
                return fixed4(finalColor,saturate(node_3132));
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
            #pragma multi_compile _ PIXELSNAP_ON
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
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
                #ifdef PIXELSNAP_ON
                    o.pos = UnityPixelSnap(o.pos);
                #endif
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
