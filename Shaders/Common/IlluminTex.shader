Shader "Inception-Common/IlluminTex" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_OtherColor ("Other Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) ", 2D) = "white" {}
	_SpecularIllumOther ("SpecularIllumOther (RGB)", 2D) = "black" {}
	_EmissionLM ("Emission (Lightmapper)", Float) = 0
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200
	
CGPROGRAM
#pragma surface surf Lambert noforwardadd exclude_path:prepass 
#pragma  gles d3d9 metal

sampler2D _MainTex;
sampler2D _SpecularIllumOther;
fixed4 _Color;
fixed4 _OtherColor;

struct Input {
	half2 uv_MainTex;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed3 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed3 illumOther=tex2D(_SpecularIllumOther, IN.uv_MainTex);
	fixed3 c = tex * ( illumOther.b * _Color + (1-illumOther.b) * _OtherColor );
	o.Albedo = c.rgb;
	o.Emission = c.rgb * illumOther.g;
}
ENDCG
} 
FallBack "Self-Illumin/VertexLit"
}
