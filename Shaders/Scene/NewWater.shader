// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Inception-Scene/NewWater" {
	Properties {
		_Color("Main Color", Color) = (0, 0.15, 0.115, 1)
		_MainTex ("BaseTex ", 2D) = "white" {}
		_WaveTex("Wave Texture(normalmap)", 2D) = "bump" {}
		_CubeTex("Reflection Texture", Cube) = "_Skybox" { TexGen CubeReflect }
		_BaseTexSpeed("_BaseTexSpeed", Vector) = (0,0,0,0)
		_Fresnel("Fresnel Amount", Range(0.1, 8)) = 0.5
		_WaveSpeed("Wave Speed", Range(0.0, 0.2)) = 0.01
		_Refraction("Refraction Amount", Range(0.0, 1.0)) = 0.5
	}
	SubShader {
		Tags {"Queue"="Geometry+100" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200

		Pass {
		Tags { "LightMode" = "ForwardBase" } 
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
//			Lighting Off Fog { Mode Off }

			CGPROGRAM
			#pragma multi_compile_fwbase
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma  gles d3d9 gles3 metal
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"  
         	#include "AutoLight.cginc"

				fixed4 _Color;
				sampler2D _WaveTex;
				sampler2D _MainTex;
				fixed4 _WaveTex_ST;
				fixed4 _MainTex_ST;
				samplerCUBE _CubeTex;
				fixed4 _BaseTexSpeed;
				fixed _WaveSpeed, _Refraction,_Fresnel;

				uniform fixed _SpendTime;
				uniform fixed4 _TexOffset;

				struct a2f {
					float4 vertex : POSITION;
					//fixed4 normal : NORMAL;
					//fixed4 tangent : TANGENT;
					float4 texcoord : TEXCOORD0;
					half4 color : COLOR;
				};

				struct v2f {
					float4 Pos : SV_POSITION;
					float4 Uv : TEXCOORD0;
					fixed3 viewDir  : TEXCOORD1;
					fixed4 world_viewDir : TEXCOORD2;
				};

				v2f vert (a2f v) {
					v2f o;
					o.Pos = mul(UNITY_MATRIX_MVP, v.vertex);
					o.Uv.xy = TRANSFORM_TEX(v.texcoord, _WaveTex);
					o.Uv.zw = TRANSFORM_TEX(v.texcoord, _MainTex);

					float3 viewDir = ObjSpaceViewDir(v.vertex);
					float3 worldRefl = mul ((float3x3)unity_ObjectToWorld, -viewDir);
					//TANGENT_SPACE_ROTATION;
					o.viewDir = viewDir;// mul(rotation, viewDir);
					o.world_viewDir = float4(worldRefl, v.color.w);
					return o;
				}

				fixed4 frag(v2f i) : COLOR {
					//Fake tangent Space Normals
					float4 uv = i.Uv;
					fixed4 tex = tex2D(_MainTex, uv.wz + _TexOffset);
					fixed4 bump = tex2D(_WaveTex, uv.xy + _SpendTime) + tex2D(_WaveTex, uv.xy - _SpendTime);
					bump *=0.5;

					fixed3 normalW = normalize(UnpackNormal(bump).rgb);

					i.viewDir  = normalize(i.viewDir );

					//Reflection
					fixed3 worldRefl = reflect(i.world_viewDir, normalW);
					fixed3 reflCol = texCUBE(_CubeTex, worldRefl).rgb;

					//Fresnel term
					fixed EdotN = max(dot(i.viewDir,normalW), 0);
					fixed facing = 1.0 - EdotN;

					fixed fresnel = pow(facing, _Fresnel);

					fixed3 deepCol = (reflCol * _Refraction + _Color * (1 - _Refraction));
					fixed3 waterCol = (_Color * facing + deepCol * EdotN);
					fixed3 finalColor = fresnel*reflCol + waterCol;
					finalColor*=tex.rgb;
					return fixed4(finalColor, i.world_viewDir.w * saturate(fresnel + 0.2));
				}
			ENDCG
		}
	} 
	FallBack Off
}
