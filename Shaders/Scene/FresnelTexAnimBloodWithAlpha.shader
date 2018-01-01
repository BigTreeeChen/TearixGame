Shader "Inception-Scene/FresnelTexAnimBloodWithAlpha" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_OutputColor("outColor", Color) = (1,1,1,1)
		_RimColor ("Rim Color", Color) = (1,1,1,1)
     	_RimPower ("Rim Power", Range(1.0,8.0)) = 3.0

		_Distrub ("distrub parameter", Range(0,1)) = 0.3
		_MainTex ("Main (RGB) Alpha (A)", 2D) = "white"
		_NoiseTex ("Noise (RGB) Alpha (A)", 2D) = "white"
		_MaskTex("mask (RGB) Alpha (A)", 2D) = "white"
		_Speed("speed", Vector) = (0.1, 0.1, 0.1, 0.1)
		_PixSpeed("pixspeed", Vector) = (0.1, 0.1, 0.1, 0.1)

		_Range("Range", Float) = 0.1
		_Period("Period", Float) = 1
		_Section("Section", Float) = 10
		_Offset("Offset", Float) = 0
	}
	SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	LOD 400
	Blend SrcAlpha One
	
	CGPROGRAM
	
	#pragma surface surf Lambert vertex:vert noforwardadd nolightmap exclude_path:prepass 
	#pragma  gles d3d9 metal

	sampler2D _MaskTex;
	sampler2D _NoiseTex;
	sampler2D _MainTex;

	fixed2 _Speed;
	fixed2 _PixSpeed;
	fixed4 _OutputColor;
	fixed4 _Color;
	fixed4 _RimColor;
	fixed  _RimPower;
	fixed  _Distrub;

	fixed _Range;
	fixed _Period;
	fixed _Section;
	fixed _Offset;

	struct Input {
		half2 uv_MainTex;
		float3 viewDir;
	};

	struct appdata_t {
		float4 vertex : POSITION;
		fixed4 color : COLOR;
		float2 texcoord : TEXCOORD0;
		float3 normal : NORMAL;
	};

	void vert (inout appdata_t v)
	{
		fixed4 Sin = _Range*sin(_Time.y*_Period + v.texcoord.x*_Section + _Offset);
		v.vertex.xyz += Sin * float3( v.normal.x, v.normal.y, v.normal.z)* v.color.a;
	}

	void surf (Input IN, inout SurfaceOutput o) {
		fixed time = fmod(_Time.y, 100)* 0.01;
		fixed2 fTranslation= time * _Speed.xy;
		fixed4 vec = tex2D (_NoiseTex, IN.uv_MainTex + fTranslation);
		fixed4 out1 = float4(vec.x, vec.x, vec.x, 1.0);
		out1 *= _Distrub;
		out1.xy += time * _PixSpeed.xy;
		fixed4 c1 = tex2D(_NoiseTex, IN.uv_MainTex + out1.xy) * _OutputColor;
		
		fixed4 MainColor = tex2D(_MainTex, IN.uv_MainTex);
		fixed4 MaskColor = tex2D(_MaskTex, IN.uv_MainTex);
		fixed4 c = MainColor * _Color + c1 * MaskColor;
		
		
		o.Albedo = c.rgb;
		o.Alpha = MainColor.a * MaskColor.a;
		fixed rim = 1.0 - dot (normalize(IN.viewDir), o.Normal);
		o.Emission = _RimColor.rgb * pow (rim, _RimPower);
	}
	ENDCG
	}
	FallBack "Diffuse"
}
