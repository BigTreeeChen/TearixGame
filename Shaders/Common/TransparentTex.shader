Shader "Inception-Common/TransparentTex" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_TransTex ("Trans (R)", 2D) = "white" {}
}

SubShader {
	Tags {"Queue"="AlphaTest-1" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200

CGPROGRAM
#pragma surface surf Lambert noforwardadd alpha exclude_path:prepass 
#pragma  gles d3d9 metal

sampler2D _MainTex;
sampler2D _TransTex;
fixed4 _Color;

struct Input {
	half2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	fixed4 trans = tex2D(_TransTex, IN.uv_MainTex);

	o.Albedo = c.rgb;
	o.Alpha = trans.r * _Color.a;
}
ENDCG
}

Fallback "Transparent/VertexLit"
}
