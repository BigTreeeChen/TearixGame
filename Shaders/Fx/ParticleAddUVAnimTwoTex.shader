//透明的add叠加，两张贴图分别进行uv动画

Shader "Inception-Fx/ParticleAddUVAnimTwoTex" {
	Properties {
		 _Color ("Main Color", Color) = (1,1,1,1)
		 _MainTex ("Main Tex (RGB)", 2D) = "black" {}
		 _SpeedU ("Base SpeedU", float) = 0
		 _SpeedV ("Base SpeedV", float) = -0.05
		 
		 _Color2 ("2nd Color", Color) = (1,1,1,1)
		 _Tex2 ("2nd Tex (RGB)", 2D) = "black" {}
		 _SpeedU2 ("2nd SpeedU", float) = 0
		 _SpeedV2 ("2nd SpeedV", float) = -0.05
		 _Add2("2nd Add",float)=1
		 
		 _AllAlpha("all alpha",float)=1
		 
		
	}
	
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 300
		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		BLEND SrcAlpha One 
		Pass {
			CGPROGRAM
			//MODE_USE_ALPHA_1: the second alpha will be inflected by the first texture`s alpha
		    #pragma multi_compile MODE_NORMAL MODE_USE_ALPHA_1
			#pragma vertex vert
			#pragma fragment frag
			#pragma  gles d3d9 gles3 metal
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			fixed _SpeedU;
			fixed _SpeedV;
			fixed4 _Color2;
			sampler2D _Tex2;
			fixed _SpeedU2;
			fixed _SpeedV2;
			fixed _Add2;
			fixed _AllAlpha;
			
			
			
			float4 _MainTex_ST;
			float4 _Tex2_ST;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
				float3 normal: NORMAL;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 animUv1 : TEXCOORD0;
				half2 animUv2 : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.animUv1=TRANSFORM_TEX(v.texcoord,_MainTex)+_Time*fixed2(_SpeedU,_SpeedV);
				o.animUv2=TRANSFORM_TEX(v.texcoord,_Tex2)+_Time*fixed2(_SpeedU2,_SpeedV2);
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 mainTex=tex2D(_MainTex, i.animUv1)*_Color*i.color;
				fixed4 secondeTex=tex2D(_Tex2, i.animUv2)*_Color2*i.color;
				#if MODE_USE_ALPHA_1
				secondeTex.a *= mainTex.r;
				#endif
				fixed4 total=mainTex*mainTex.a+secondeTex*secondeTex.a*_Add2;
				total.a *= _AllAlpha;
				return total;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
	CustomEditor  "ParticleAddUv2AnimEditor"
}
