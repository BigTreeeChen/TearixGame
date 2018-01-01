// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "Inception-Scene/UnlitLightmapMask" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_MaskTex ("Mask (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	// Non-lightmapped
	Pass {
		Tags { "LightMode" = "Vertex" }
		Lighting Off
		SetTexture [_MainTex] { combine texture } 
		SetTexture [_MaskTex] {
			combine texture * previous, texture * previous
		}
	}

	// Lightmapped, encoded as dLDR
	Pass {
		Tags { "LightMode" = "VertexLM" }

		Lighting Off
		BindChannels {
			Bind "Vertex", vertex
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
			Bind "texcoord", texcoord2 // mask uses 1st uv
		}
		
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			combine texture
		}
		SetTexture [_MainTex] {
			combine texture * previous DOUBLE, texture
		}
		SetTexture [_MaskTex] {
			combine texture * previous, texture * previous
		}
	}

	// Lightmapped, encoded as RGBM
	Pass {
		Tags { "LightMode" = "VertexLMRGBM" }
		
		Lighting Off
		BindChannels {
			Bind "Vertex", vertex
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
			Bind "texcoord", texcoord2 // mask uses 1st uv
		}
		
		SetTexture [unity_Lightmap] {
			matrix [unity_LightmapMatrix]
			combine texture * texture alpha DOUBLE
		}
		SetTexture [_MainTex] {
			combine texture * previous QUAD, texture
		}
		SetTexture [_MaskTex] {
			combine texture * previous, texture * previous
		}
	}	
	

}
}



