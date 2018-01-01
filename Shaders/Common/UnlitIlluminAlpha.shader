Shader "Inception-Common/UnlitIlluminAlpha" {
		Properties {
	 	_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_IlluminAlpha ("Illumin (R) Alpha (G)", 2D) = "white" {}
		_IllumStrength("Illum Strength", float) = 1
		_IlluminColor("Illumin Color", Color) = (1,1,1,1)
	}
	SubShader {
			Tags{ "RenderType" = "Oqapue" "IgnoreProjector" = "True" }
		LOD 200
		Cull Back
		Blend SrcAlpha OneMinusSrcAlpha
		//ZWrite Off
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
				fixed2 illuminAlpha = tex2D(_IlluminAlpha, uv).rg;
				
				clip(illuminAlpha.y - 0.1);

				fixed4 base = tex2D(_MainTex, uv);
				
				base.a = illuminAlpha.y;
				base = base * _Color *(1 + illuminAlpha.x * _IlluminColor * _IllumStrength);
				return base;
			}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}
