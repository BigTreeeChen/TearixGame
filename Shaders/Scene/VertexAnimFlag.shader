Shader "Inception-Scene/VertexAnimFlag" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Main (RGB)", 2D) = "white"
		_AlphaTex ("Alpha (A)", 2D) = "white"
		_Range("Range", Float) = 0.1
		_Period("Period", Float) = 1
		_Section("Section", Float) = 10
		_Offset("Offset", Float) = 0
	}
	SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	ZWRITE Off
	LOD 300

	CGPROGRAM
	#pragma surface surf Lambert vertex:vert  noforwardadd  exclude_path:prepass 
	#pragma  gles d3d9 gles3 metal

	sampler2D _MainTex;
	sampler2D _AlphaTex;


	fixed4 _Color;
	fixed _Range;
	fixed _Period;
	fixed _Section;
	fixed _Offset;

	struct Input {
		half2 uv_MainTex;
	};

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float2 texcoord1 : TEXCOORD1;
		float3 normal : NORMAL;
	};

	void vert (inout appdata_t v)
	{
		fixed4 Sin = _Range*sin(_Time.y*_Period + v.texcoord.x*_Section + _Offset);
		v.vertex.xyz += Sin * float3( v.normal.x, v.normal.y, v.normal.z)* v.color.a;
	}

	void surf (Input IN, inout SurfaceOutput o) {
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex) *  _Color;
		o.Alpha =  tex2D(_AlphaTex, IN.uv_MainTex).r *  _Color.a;
	}
	ENDCG
	}
	FallBack "Diffuse"
}
