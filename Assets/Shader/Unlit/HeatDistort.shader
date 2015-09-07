//热流扰动的效果.
Shader "ZZL/Unlit/Heat Distort" {
	Properties {
		_Color("Main Color",Color)=(1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OffsetTex ("Shui (RGB)", 2D) = "white" {}
		_Speed("_Speed",float)=1
		_Range("_Range",float)=20
	}

	SubShader {
		Tags{"RenderType"="Transparent"}
		ZTest Always Cull Off ZWrite Off
		Blend SrcAlpha One
		Fog { Mode off }
		
		Pass {
					
			CGPROGRAM
			#pragma exclude_renderers d3d11 xbox360
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _OffsetTex;
			float _Speed;
			float _Range;
			fixed4 _Color;

			struct v2f {
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
				float2 offsetUV;
			};

			v2f vert( appdata_img v )
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord;
				o.offsetUV = float2(v.texcoord.x,v.texcoord.y-_Time*_Speed);
				return o;
			}

			float4 frag (v2f i) : COLOR{
				float4 offsetTex = tex2D(_OffsetTex, i.offsetUV);
				float2 offsetUV = i.uv + offsetTex.xy/_Range;
				return tex2D(_MainTex, offsetUV)*_Color;
			}
			ENDCG

		}
	}

}
