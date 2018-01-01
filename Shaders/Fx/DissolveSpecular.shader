Shader "Inception-Fx/DissolveSpecular" {
Properties {
	_MainColor ("Main Color", Color) = (1,1,1,1)
	_Amount ("Amount", Range (0, 1)) = 0.5
	_StartAmount("StartAmount", float) = 0.1
	_Illuminate ("Illuminate", Range (0, 1)) = 0.5
	_Tile("Tile", float) = 1
	_DissColor ("DissColor", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_DissolveSrc ("DissolveSrc", 2D) = "white" {}

}
SubShader { 
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 250
	ZWrite Off
	blend SrcAlpha OneMinusSrcAlpha

    // 因为这个半透shader可能会被用在Cube这种模型上，为了模型背面和正面的深度能正常显示，需要用两个Pass。
    // 不能用ZWrite On，否则可能会导致整个背面都看不见。
	
	Pass
	{
        Cull Front
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma  gles d3d9 gles3 metal
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		sampler2D _DissolveSrc;

        fixed4 _MainColor;
		fixed4 _DissColor;
		fixed _Amount;
		static fixed3 Color = float3(1,1,1);
		fixed _Illuminate;
		fixed _Tile;
		fixed _StartAmount;
		
		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			half2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			half2 texcoord : TEXCOORD0;
		};

		float4 _MainTex_ST;
		v2f vert (appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 tex = tex2D(_MainTex, i.texcoord);
			fixed4 c = tex * _MainColor;
			fixed ClipTex = tex2D (_DissolveSrc, i.texcoord/_Tile).r ;
			fixed ClipAmount = ClipTex - _Amount;
			fixed Clip = 0;
			if (ClipAmount <0)
			{
				c.a = 0;		
			}
			 else
			 {
				c.a = 1;
				if (ClipAmount < _StartAmount)
				{
					c.rgb  = (c.rgb *((_DissColor.x+_DissColor.y+_DissColor.z))* _DissColor.rgb*((_DissColor.x+_DissColor.y+_DissColor.z)))/(1 - _Illuminate);
				}
			 }

			c.a *= _MainColor.a;
			return c;
		}
		ENDCG
	}

	Pass
	{
        Cull Back
		
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma  gles d3d9 gles3 metal
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		sampler2D _DissolveSrc;

        fixed4 _MainColor;
		fixed4 _DissColor;
		fixed _Amount;
		static fixed3 Color = float3(1,1,1);
		fixed _Illuminate;
		fixed _Tile;
		fixed _StartAmount;
		
		struct appdata_t {
			float4 vertex : POSITION;
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
			o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			return o;
		}

		fixed4 frag (v2f i) : COLOR
		{
			fixed4 tex = tex2D(_MainTex, i.texcoord);
			fixed4 c = tex * _MainColor;
			fixed ClipTex = tex2D (_DissolveSrc, i.texcoord/_Tile).r ;
			fixed ClipAmount = ClipTex - _Amount;
			fixed Clip = 0;
			if (ClipAmount <0)
			{
				c.a = 0;		
			}
			 else
			 {
				c.a = 1;
				if (ClipAmount < _StartAmount)
				{
					c.rgb  = (c.rgb *((_DissColor.x+_DissColor.y+_DissColor.z))* _DissColor.rgb*((_DissColor.x+_DissColor.y+_DissColor.z)))/(1 - _Illuminate);
				}
			 }

			c.a *= _MainColor.a;
			return c;
		}
		ENDCG
	}
}

FallBack "Unlit/Texture"
}
