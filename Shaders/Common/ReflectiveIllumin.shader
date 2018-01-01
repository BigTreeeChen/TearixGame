Shader "Inception-Common/ReflectiveIllumin" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)	
	_MainTex ("Base (RGB)", 2D) = "white" {} 
	_Cube ("Reflection Cubemap", Cube) = "_Skybox" { TexGen CubeReflect }
    _CubeSize("Cubemap Size", float) = 3	_IllumColor("Illum Color", Color) = (0,0,0,0)
	_IllumStrength("Illum Strength", float) = 1
	_CubeStrength("Cube Strength", float) = 1
}

SubShader {
	LOD 250
	Tags{ "RenderType" = "Opaque"}
	
CGPROGRAM
#pragma  gles d3d9 metal
#pragma surface surf Lambert noforwardadd exclude_path:prepass

sampler2D _MainTex;
samplerCUBE _Cube;

fixed4 _Color;
fixed _CubeSize;
fixed4 _ReflectColor;
fixed4 _IllumColor;
fixed _IllumStrength;
fixed _CubeStrength;

struct Input {
	float2 uv_MainTex;
	float3 worldRefl;
};

void surf (Input IN, inout SurfaceOutput o) {
	fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

	fixed4 c = tex * _Color;
	fixed4 reflcol = texCUBE(_Cube, float3(IN.worldRefl.x, IN.worldRefl.y * _CubeSize, IN.worldRefl.z)) * _ReflectColor;

	o.Albedo = c.rgb * (reflcol.rgb * _CubeStrength  + 1);
	o.Emission = c.rgb* _IllumColor*_IllumStrength;

	o.Alpha = reflcol.a;
}
ENDCG
}

FallBack "Unlit/Texture"
} 
