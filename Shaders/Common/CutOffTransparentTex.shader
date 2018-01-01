Shader "Inception-Common/CutOffTransparentTex" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
	_Trans ("Trans (R)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	blend SrcAlpha OneMinusSrcAlpha
	
CGPROGRAM
#pragma surface surf Lambert noforwardadd exclude_path:prepass 
#pragma  gles d3d9 gles3 metal

sampler2D _MainTex;
sampler2D _Trans;

fixed4 _Color;
fixed _Cutoff;

struct Input {
	half2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed trans = tex2D(_Trans, IN.uv_MainTex).r;

	o.Albedo = c.rgb;
	o.Alpha = trans * _Color.a;
	if(o.Alpha < _Cutoff)
	{
		o.Alpha = 0;
	}
}
ENDCG
}

Fallback "Transparent/Cutout/VertexLit"
}
