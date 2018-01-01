Shader "Inception-Fx/AdditiveTurbulenceProgress" {
Properties {
	_MainTex ("Base(RGBA)", 2D) = "white" {}
	_Color ("Main Color", Color) = (1,1,1,1)
	_BlendTex1 ("BlendTex1(RGBA)", 2D) = "white" {}
	_ColorBlendTex1 ("BlendTex1 Color", Color) = (1,1,1,1)
	_BlendTex2 ("BlendTex2(RGBA)", 2D) = "white" {}
	_ColorBlendTex2 ("BlendTex2Color", Color) = (1,1,1,1)
	_BlendTexSpeed1 ("_BlendTexSpeed1", float) = 0
    _BlendTexSpeed2 ("_BlendTexSpeed2", float) =  0
    _AddLightBase ("AddLightBase", float) = 1
    _AddLightBlend ("AddLightBlend", float) = 1
    _Progress("Progress", float) = 1
}
SubShader { 
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 250
	cull off
	ZWrite Off
	blend SrcAlpha One
	Fog { Color (0,0,0,0) }
	
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma  gles d3d9 gles3 metal  d3d9
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		sampler2D _BlendTex1;
		sampler2D _BlendTex2;

		fixed4 _Color;
		fixed4 _ColorBlendTex1;
		fixed4 _ColorBlendTex2;
		fixed _BlendTexSpeed1;
		fixed _BlendTexSpeed2;
		fixed _AddLightBase;
		fixed _AddLightBlend;
		fixed _Progress;

		float4 _MainTex_ST;
		float4 _BlendTex1_ST;
		float4 _BlendTex2_ST;
		
		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			half2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			half2 texcoord : TEXCOORD0;
			half2 blenduv1 : TEXCOORD1;
			half2 blenduv2 : TEXCOORD2;
		};

		
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			o.blenduv1= TRANSFORM_TEX(v.texcoord,_BlendTex1)+_Time.x * fixed2(0, _BlendTexSpeed1);
			o.blenduv2=	 TRANSFORM_TEX(v.texcoord,_BlendTex2)+_Time.x* fixed2(_BlendTexSpeed2, _BlendTexSpeed2);
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 blendc =(tex2D(_BlendTex1, i.blenduv1) * _ColorBlendTex1 + tex2D(_BlendTex2, i.blenduv2) * _ColorBlendTex2)*_AddLightBlend  ;
			fixed4 base = i.color * tex2D(_MainTex, i.texcoord) * _Color *_AddLightBase ;
			fixed4 c =  base*blendc + base;
			if(i.texcoord.x > _Progress)
			{
				c.a = 0;
			}
			return c ;
		}
		ENDCG
	}
}

FallBack "Unlit/Texture"
}
