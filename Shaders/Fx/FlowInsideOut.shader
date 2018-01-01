Shader "Inception-Fx/FlowInsideOut" {
	Properties {
		_MainTex ("Base Texture (RGB)", 2D) = "white" {}
        _MainColor ("Base Color", Color) = (1, 1, 1, 1)
		_FlowTex ("Flow Texture (RGB)", 2D) = "black" {}
        _FlowColorAtCenter ("Flow Color At Center", Color) = (1, 1, 1, 1)
        _FlowColorAtEdge ("Flow Color At Edge", Color) = (1, 1, 1, 1)
		_Period ("Period (Seconds)", float) = 1
		_FlowWidth ("Flow Width", Range(0, 1)) = 0.1
        _FlowHighlight ("Flow Highlight", float) = 1
        [Toggle] _InvertDirection ("Invert Direction?", float) = 0
        _AllAlpha ("All Alpha", Range(0, 1)) = 1
	}
	SubShader {
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Cull Off
		Lighting Off
		ZWrite Off
        Fog { Mode Off }
		Blend One One
		
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma  gles d3d9 gles3 metal
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
            fixed4 _MainColor;
			sampler2D _FlowTex;
            fixed4 _FlowColorAtCenter;
            fixed4 _FlowColorAtEdge;
			float _Period;
			float _FlowWidth;
            float _FlowHighlight;
            float _InvertDirection;
            float _AllAlpha;
		
			struct appdata_t {
				float4 vertex : POSITION;
				half2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : POSITION;
				half2 mainTex : TEXCOORD0;
                half2 flowUV : TEXCOORD1;
			};
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.mainTex = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.flowUV = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 baseColor = tex2D(_MainTex, i.mainTex) * _MainColor;
				float2 center = float2(0.5, 0.5);
				float r = distance(i.flowUV, center);
                float radiusMax = 0.5;  // 从(0.5, 0.5)到(0.5, 1)的距离
                float timeProgress = fmod(_Time.y, _Period) / _Period;
                float flowProgress = _InvertDirection * (1 - timeProgress) + (1 - _InvertDirection) * timeProgress;
				float flowRadiusMax = flowProgress * (radiusMax + _FlowWidth);
				float flowRadiusMin = flowRadiusMax - _FlowWidth;
				float isInFlow = step(flowRadiusMin, r) - step(flowRadiusMax, r);
				float2 flowTexUV = float2((r - flowRadiusMin) / (flowRadiusMax - flowRadiusMin), 0);
                fixed4 flowColor = _FlowColorAtCenter + flowProgress * (_FlowColorAtEdge - _FlowColorAtCenter);
				fixed4 finalColor = baseColor + isInFlow * _FlowHighlight * tex2D(_FlowTex, flowTexUV) * flowColor * baseColor;
                finalColor *= _AllAlpha;
                return finalColor;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
