// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Inception-Scene/PlantTransparent" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_TransTex ("Trans (R)", 2D) = "white" {}
	_Dist ("_Dist", float) = 0.04
	_Speed ("_Speed", float) = 0.2
	_SpeedOffset ("_SpeedOffset", float) = 0.2
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 350

CGPROGRAM
#pragma surface surf Lambert noforwardadd alpha vertex:vert  
#pragma  gles d3d9 gles3 metal

sampler2D _MainTex;
sampler2D _TransTex;
fixed4 _Color;

struct Input {
	half2 uv_MainTex;
};

fixed _Dist;
fixed _Speed;
fixed _SpeedOffset;

struct appdata_t {
	float4 vertex : POSITION;
	fixed4 color : COLOR;
	float2 texcoord : TEXCOORD0;
	float2 texcoord1 : TEXCOORD1;
	float3 normal : NORMAL;
};

void vert (inout appdata_t v)
{
	float4 vertex = mul(v.vertex, unity_ObjectToWorld);
	float a = vertex.x * vertex.z;
	float fa = fmod(a, 2);
 	v.vertex.xyz += float3(sin(a),0,cos(a)) * _Dist * sin(_Time.w * (_Speed + _SpeedOffset*(1-fa))+a) * v.color.a;
}

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
