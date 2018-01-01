Shader "Inception-Scene/TransparentXYZ" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_XDist ("X Dist", float) = 0
	_YDist ("Y Dist", float) = 0
	_ZDist ("Z Dist", float) = 0
	_Speed ("Speed", float) = 1
}
SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 200

CGPROGRAM
#pragma surface surf Lambert vertex:vert noforwardadd exclude_path:prepass 
#pragma  gles d3d9 gles3 metal

sampler2D _MainTex;
fixed4 _Color;

float _XDist;
float _YDist;
float _ZDist;
float _Speed;

struct Input {
	float2 uv_MainTex;
};

void vert (inout appdata_full v)
{	
	v.vertex.xyz += float3(_XDist, _YDist, _ZDist) * sin(_Speed * _Time.y) * v.color.a;
}

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
	o.Albedo = c.rgb;
	o.Alpha = c.a;
}
ENDCG
}

Fallback "VertexLit"
}
