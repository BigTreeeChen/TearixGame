Shader "Inception-Fx/ParicleBlendTexSheet" {
Properties {
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_MainTex ("Particle Texture", 2D) = "white" {}
	_LoopTime ("LoopTime", Float) = 1
	_TileU ("TileU", Float) = 1
	_TileV ("TileV", Float) = 1
}

Category {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha OneMinusSrcAlpha
	//ColorMask RGB
	Cull Off Lighting Off ZWrite Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma  gles d3d9 gles3 metal
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			fixed _LoopTime;
			fixed _TileU;
			fixed _TileV;

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);

				//_LoopTime = 2
				//TileU = 4
				//TileV = 4

				fixed secU = 1/_TileU;//0.25
				fixed secV = 1/_TileV;//0.25

				fixed secUTime = _LoopTime * secU * secV;//0.5
				fixed secVTime = _LoopTime * secV;//0.5

				fixed fracTimeU = fmod(_Time.y, _LoopTime * secV);//2.6 - 0.6
				fixed fracTimeV = fmod(_Time.y, _LoopTime);//2.6 - 0.6

				fixed secIdxU = floor(fracTimeU/secUTime);//1.2
				fixed secIdxV = _TileV - 1 - floor(fracTimeV/secVTime);//1.2

				o.texcoord.x = o.texcoord.x/_TileU + secU * secIdxU;//v
				o.texcoord.y = o.texcoord.y/_TileV + secV * secIdxV;//v
				return o;
			}

			sampler2D_float _CameraDepthTexture;
			
			fixed4 frag (v2f i) : SV_Target
			{
				return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord);
			}
			ENDCG 
		}
	}	
}
}

