Shader "Inception-Fx/ParticleAddIllumin" {
	Properties {
		 _Color ("Main Color", Color) = (1,1,1,1)
		 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		 _UVInlumminTex ("UV (R) Illumin (G)", 2D) = "black" {}
		  _IlluminlColor ("Illumin Color", Color) = (1,1,1,1)
		  _FlowAdd("FlowAdd", Range(0,20)) = 2
		  _AllAlpha("AllAlpha",Range(0,1))=1
		   
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		Lighting Off
		ZWrite Off
		BLEND SrcAlpha OneMinusSrcAlpha 
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma  gles d3d9 gles3 metal
			 #include "UnityCG.cginc"

			fixed4 _Color;
			sampler2D _MainTex;

			sampler2D _UVInlumminTex;
			fixed4 _IlluminlColor;
			fixed _FlowAdd;
			fixed _AllAlpha;

			
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
				fixed4 diffuseTex=tex2D(_MainTex, i.texcoord);
				fixed illum=tex2D(_UVInlumminTex, i.texcoord).g;
		
				fixed4 c = diffuseTex * _Color*_FlowAdd;
				c+=_IlluminlColor*diffuseTex*illum;// add detail to transparent area add illumination
				c.a=c.a*_AllAlpha;
				return c;
			}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}
