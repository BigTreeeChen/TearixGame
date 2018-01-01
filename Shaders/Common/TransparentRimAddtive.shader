Shader "Inception-Fx/TransparentRimAddtive" {
	Properties{
	_RimColor("Rim Color", Color) = (0.5, 0.5, 0.5, 0.5)
	_RimPower("Rim Power", Range(0.0, 5.0)) = 2.5
	_AlphaPower("Alpha Rim Power", Range(0.0, 8.0)) = 4.0
	_AllPower("All Power", Range(0.0, 10.0)) = 1.0
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

	SubShader{
		Tags{ "Queue" = "Geometry+200" "RenderType" = "Opaque"  "IgnoreProjector" = "True" }
		Pass{
			ZWrite On
			ColorMask 0
		}

			Blend SrcAlpha One
			ZTest LEqual
			ZWrite On

			CGPROGRAM
#pragma surface surf Lambert noforwardadd exclude_path:prepass 
#pragma  gles d3d9 gles3 metal
			struct Input
			{
				float3 viewDir;
                float4 color : COLOR;
				float2 uv_MainTex;
				INTERNAL_DATA
			};

			fixed4 _RimColor;
			fixed _RimPower;
			fixed _AlphaPower;
			fixed _AllPower;
			sampler2D _MainTex;

			void surf(Input IN, inout SurfaceOutput o) {
				fixed3 tex = tex2D(_MainTex, IN.uv_MainTex + float2(_Time.x, _Time.x));
				half rim = saturate(dot(normalize(IN.viewDir), o.Normal));
				o.Emission = _RimColor.rgb * pow(1.0 - rim, _RimPower)*_AllPower +tex * pow(rim - 0.3, _RimPower) * _AllPower;
				o.Alpha = IN.color.a;
			}
			ENDCG
		
	}
	Fallback "VertexLit"
}