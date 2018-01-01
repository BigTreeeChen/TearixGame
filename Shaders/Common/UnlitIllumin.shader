Shader "Inception-Common/UnlitIllumin" {
		Properties {
	 	_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IlluminAlpha ("Illumin (R)", 2D) = "white" {}
		_IllumStrength("Illum Strength", float) = 1
		_IlluminColor("Illumin Color", Color) = (1,1,1,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Cull Back
		Lighting Off

			Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma  gles d3d9 gles3 metal
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;
			fixed4 _MainTex_ST;
			sampler2D _IlluminAlpha;
			
			fixed _IllumStrength;
			fixed4 _IlluminColor;
		
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed2 uv = i.texcoord *_MainTex_ST.xy + _MainTex_ST.zw;
				return tex2D(_MainTex, uv) * _Color *(1 + tex2D(_IlluminAlpha, uv).r * _IlluminColor * _IllumStrength);
			}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}
