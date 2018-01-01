Shader "Inception-Common/TransparentIllumin" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Illum ("Illumin (R)", 2D) = "white" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
}
SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert noforwardadd alpha exclude_path:prepass 
#pragma  gles d3d9 gles3 metal

sampler2D _MainTex;
sampler2D _Illum;
fixed4 _Color;

struct Input {
	half2 uv_MainTex;
	half2 uv_Illum;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 c = tex * _Color;
	o.Albedo = c.rgb;
	o.Emission = c.rgb * tex2D(_Illum, IN.uv_Illum).r;
	o.Alpha = c.a;
}
ENDCG
} 
FallBack "Transparent/VertexLit"
}
