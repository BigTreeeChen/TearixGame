Shader "Inception-Fx/ScreenColorCorrectionAdd" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_RRate ("_RRate", Float) = 1
		_GRate ("_GRate", Float) = 1
		_BRate ("_BRate", Float) = 1
	}

	// Shader code pasted into all further CGPROGRAM blocks
	CGINCLUDE

	#pragma fragmentoption ARB_precision_hint_fastest
	
	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : POSITION;
		half2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	fixed _RRate;
	fixed _GRate;
	fixed _BRate;

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		return o;
	} 

	fixed4 frag(v2f i) : COLOR 
	{
		fixed4 color = tex2D(_MainTex, i.uv); 
		return fixed4(color.r  +  _RRate, color.g + _GRate, color.b + _BRate, color.a);		
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma  gles d3d9 gles3 metal
      ENDCG
  }
}

Fallback off
	
} // shader