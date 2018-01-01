Shader "Inception-Scene/T4MFourTexturesCube" {
Properties {
	_Splat0 ("Layer 1", 2D) = "white" {}
	_Splat1 ("Layer 2", 2D) = "white" {}
	_Splat2 ("Layer 3", 2D) = "white" {}
	_SplatCube ("Layer 4", Cube) = "_Skybox" {TexGen CubeReflect}
	_Control ("Control (RGBA)", 2D) = "white" {}
	_CubeColor ("Cube Color", Color) = (1,1,1,0.5)
	_CubeStrength("Cube Strength", float) = 1
	_CubeSize("Cubemap Size", float) = 1
}
                
SubShader {
	Tags {
   "SplatCount" = "4"
   "RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf Lambert noforwardadd approxview halfasview exclude_path:prepass 
#pragma  gles d3d9 gles3 metal
#pragma target 4.0
struct Input {
	half2 uv_Control : TEXCOORD0;
	half2 uv_Splat0 : TEXCOORD1;
	half2 uv_Splat1 : TEXCOORD2;
	half2 uv_Splat2 : TEXCOORD3;
	float3 worldRefl;
};
 
sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2;
samplerCUBE _SplatCube;
fixed4 _CubeColor;
fixed _CubeStrength;
fixed _CubeSize;

 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
	fixed3 lay1 = tex2D (_Splat0, IN.uv_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.uv_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.uv_Splat2);
	fixed3 reflcol = texCUBE (_SplatCube, float3(IN.worldRefl.x, IN.worldRefl.y * _CubeSize, IN.worldRefl.z)) * _CubeColor.rgb  * _CubeStrength;
	o.Alpha = 0.0;
	o.Albedo.rgb = ((lay1 * ( 1-  _CubeColor.a ) + reflcol *  _CubeColor.a ) * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b);
}
ENDCG 
}

// Fallback to Diffuse
Fallback "Diffuse"
}
