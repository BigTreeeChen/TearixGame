Shader "Inception-Charactor/AdditiveUv" {
	Properties{
	_Color("Main Color", Color) = (0.75, 0.75, 0.75, 1)
	_Shininess("Shininess", Range(0.01, 2)) = 0.618
	_MainTex("Base (RGB)", 2D) = "white" {}
	_AddtiveTex("AddtiveTex (RGB)", 2D) = "black" {}
	_Addtive( "Addtive", Color) = (1,1,1)
	_SpeedUV("Speed U", Vector) = (1, 1, 1,1)
}
	SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 350
			Pass{
			Name "FORWARD"
			Tags{ "LightMode" = "ForwardBase" }

			CGPROGRAM
#pragma vertex vert_surf
#pragma fragment frag_surf
#pragma  gles d3d9 metal
#pragma fragmentoption ARB_precision_hint_fastest
#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "AutoLight.cginc"

			fixed4 _Color;
			half _Shininess;
			half2 _SpeedUV;
			sampler2D _MainTex;
			sampler2D _AddtiveTex;
			fixed3 _Addtive;

			// vertex-to-fragment interpolation data
			struct v2f_surf {
				float4 pos : SV_POSITION;
				half4 pack0 : TEXCOORD0;
			};

			float4 _MainTex_ST;

			// vertex shader
			v2f_surf vert_surf(appdata_full v) {
				v2f_surf o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.pack0.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.pack0.zw = o.pack0.xy + _Time.x*_SpeedUV.xy;
				return o;
			}

			fixed4 frag_surf(v2f_surf IN) : SV_Target{
				
				half2 mainUv = IN.pack0.xy;
				half2 addtiveUv = IN.pack0.zw;

				fixed3 tex = tex2D(_MainTex, mainUv);
				fixed3 c = tex *_Color;

				fixed4 out_c;
			
				fixed3 addtive = tex2D(_AddtiveTex, addtiveUv);
				out_c.rgb = c*_Shininess + addtive*_Addtive;
				out_c.a = 1;

				return out_c;
			}

			ENDCG

		}

	}
	FallBack "Unlit/Texture"
}
