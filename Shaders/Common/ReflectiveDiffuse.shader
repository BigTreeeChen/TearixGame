Shader "Inception-Common/ReflectiveDiffuse" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
	_MainTex ("Base (RGB)", 2D) = "white" {} 
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
	_RefStrength ("RefStrength (R)", 2D) = "white" {}
	_CubeSize("Cubemap Size", float) = 3
}

SubShader {
	LOD 200
	Tags { "RenderType"="Opaque" }
	
CGPROGRAM
#pragma  gles d3d9 metal
#pragma surface surf Lambert noforwardadd exclude_path:prepass 

sampler2D _MainTex;
samplerCUBE _Cube;
sampler2D _RefStrength;

fixed4 _Color;
fixed4 _ReflectColor;
fixed _CubeSize;

struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
	fixed4 refStrength = tex2D(_RefStrength, IN.uv_MainTex);

	fixed4 c = tex * _Color;
	fixed4 reflcol = texCUBE (_Cube, float3(IN.worldRefl.x, IN.worldRefl.y * _CubeSize, IN.worldRefl.z));
	float3 e = reflcol.rgb * _ReflectColor.rgb * refStrength.r;

	o.Albedo = c.rgb * (float3(1,1,1) + e * 2);

	o.Alpha = reflcol.a * _ReflectColor.a;
}
ENDCG
}

FallBack "Reflective/VertexLit"
} 
