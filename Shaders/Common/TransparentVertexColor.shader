Shader "Inception-Common/TransparentVertexColor" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}

	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		LOD 200
		Cull Back
		BLEND SrcAlpha OneMinusSrcAlpha 

	CGPROGRAM
	#pragma surface surf Lambert noforwardadd exclude_path:prepass 
	#pragma  gles d3d9 metal


	sampler2D _MainTex;
	fixed4 _Color;

	struct Input {
		half2 uv_MainTex;
		float4 color:COLOR;
	};

	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb;
		o.Alpha = c.a*IN.color.a;
	}
	ENDCG
	}
	FallBack "Unlit/Texture"
}
