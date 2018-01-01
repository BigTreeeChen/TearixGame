#warning Upgrade NOTE: unity_Scale shader variable was removed; replaced 'unity_Scale.w' with '1.0'

Shader "Inception-Scene/Water" { 
Properties {
	_WaveScale ("Wave scale", float) = 0.063
	_FresnelPow ("Fresnel Pow", float) = 1
	_BumpMap ("Normalmap ", 2D) = "bump" {}
	WaveSpeed ("Wave speed (map1 x,y; map2 x,y)", Vector) = (19,9,-16,-7)
	_ReflectiveColor ("Reflective color (RGB) fresnel (A) ", 2D) = "" {}
	_ReflectiveColorCube ("Reflective color cube (RGB) fresnel (A)", Cube) = "" { TexGen CubeReflect }
	_HorizonColor ("Simple water horizon color", COLOR)  = ( .172, .463, .435, 1)
	_MainTex ("Fallback texture", 2D) = "" {}
}


// -----------------------------------------------------------
// Fragment program cards


Subshader { 
	Tags { "WaterMode"="Refractive" "RenderType"="Opaque" }
	LOD 250
	Blend SrcAlpha OneMinusSrcAlpha
	Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma  gles d3d9 gles3 metal
#pragma fragmentoption ARB_precision_hint_fastest 

#include "UnityCG.cginc"

uniform fixed4 _WaveScale4;
uniform fixed4 _WaveOffset;
fixed _FresnelPow;


struct appdata {
	float4 vertex : POSITION;
	float3 normal : NORMAL;
	fixed4 color : COLOR;
};

struct v2f {
	float4 pos : SV_POSITION;

	half2 bumpuv0 : TEXCOORD0;
	half2 bumpuv1 : TEXCOORD1;
	half3 viewDir : TEXCOORD2;
	fixed4 color : COLOR;
};

v2f vert(appdata v)
{
	v2f o;
	o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
	
	// scroll bump waves
	fixed4 temp;
	temp.xyzw = v.vertex.xzxz * _WaveScale4 / 1.0 + _WaveOffset;
	o.bumpuv0 = temp.xy;
	o.bumpuv1 = temp.wz;
	o.color = v.color;
	// object space view direction (will normalize per pixel)
	o.viewDir.xzy = ObjSpaceViewDir(v.vertex);
		
	return o;
}

sampler2D _ReflectiveColor;
uniform fixed4 _HorizonColor;

sampler2D _BumpMap;

fixed4 frag( v2f i ) : SV_Target
{
	i.viewDir = normalize(i.viewDir);
	
	// combine two scrolling bumpmaps into one
	fixed3 bump1 = UnpackNormal(tex2D( _BumpMap, i.bumpuv0 )).rgb;
	fixed3 bump2 = UnpackNormal(tex2D( _BumpMap, i.bumpuv1 )).rgb;
	fixed3 bump = (bump1 + bump2) * 0.5;
	
	// fresnel factor
	fixed fresnelFac = pow(dot( i.viewDir, bump ), _FresnelPow);	

	// final color is between refracted and reflected based on fresnel	
	fixed4 color;	

	fixed4 water = tex2D( _ReflectiveColor, float2(fresnelFac,fresnelFac) );
	color.rgb = lerp( water.rgb, _HorizonColor.rgb, water.a );
	color.a = _HorizonColor.a * i.color.a;
	return color;
}
ENDCG

	}
}

// -----------------------------------------------------------
//  Old cards

// three texture, cubemaps
Subshader {
	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0.5)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture * primary
		}
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix2]
			combine texture * primary + previous
		}
		SetTexture [_ReflectiveColorCube] {
			combine texture +- previous, primary
			Matrix [_Reflection]
		}
	}
}

// dual texture, cubemaps
Subshader {
	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0.5)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture
		}
		SetTexture [_ReflectiveColorCube] {
			combine texture +- previous, primary
			Matrix [_Reflection]
		}
	}
}

// single texture
Subshader {
	Tags { "WaterMode"="Simple" "RenderType"="Opaque" }
	Pass {
		Color (0.5,0.5,0.5,0)
		SetTexture [_MainTex] {
			Matrix [_WaveMatrix]
			combine texture, primary
		}
	}
}


}
