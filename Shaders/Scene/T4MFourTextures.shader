Shader "Inception-Scene/T4MFourTextures" {
Properties {
	_Splat0 ("Layer 1", 2D) = "white" {}
	_Splat1 ("Layer 2", 2D) = "white" {}
	_Splat2 ("Layer 3", 2D) = "white" {}
	_Splat3 ("Layer 4", 2D) = "white" {}
	_Control ("Control (RGBA)", 2D) = "white" {}
}
                
SubShader {
	Tags {
   "SplatCount" = "4"
   "RenderType" = "Opaque"
	}
CGPROGRAM
#pragma surface surf Lambert //noforwardadd approxview halfasview exclude_path:prepass 
#pragma  gles d3d9 metal
#pragma target 4.0
struct Input {
	half2 uv_Control : TEXCOORD0;
	half2 uv_Splat0 : TEXCOORD1;
	half2 uv_Splat1 : TEXCOORD2;
	half2 uv_Splat2 : TEXCOORD3;
	half2 uv_Splat3 : TEXCOORD4;
};
 
sampler2D _Control;
sampler2D _Splat0,_Splat1,_Splat2,_Splat3;
 
void surf (Input IN, inout SurfaceOutput o) {
	fixed4 splat_control = tex2D (_Control, IN.uv_Control).rgba;
		
	fixed3 lay1 = tex2D (_Splat0, IN.uv_Splat0);
	fixed3 lay2 = tex2D (_Splat1, IN.uv_Splat1);
	fixed3 lay3 = tex2D (_Splat2, IN.uv_Splat2);
	fixed3 lay4 = tex2D (_Splat3, IN.uv_Splat3);
	o.Alpha = 0.0;
	o.Albedo.rgb = (lay1 * splat_control.r + lay2 * splat_control.g + lay3 * splat_control.b + lay4 * splat_control.a);
}
ENDCG 
}
FallBack "Unlit/Texture"
}