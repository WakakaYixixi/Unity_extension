Shader "ZZL/Unlit/MixTwoTextureAlpha" {
	Properties {
		_MainTex ("Main Texture (RGB)", 2D) = "white" {}
		_MainTex2("Texture2 (RGB)", 2D) = "white" {}
		_Mix("Texture2 Alpha",Range(0,1)) = 0
		[MaterialToggle] isReplace ("Texture2 Replace MainTex", Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "IgnoreProjector"="True" "Queue"="Transparent"}
		LOD 100
		
		Lighting off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass{
		
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile _ ISREPLACE_ON
				#include "UnityCG.cginc"
				
				struct appdata_t {
					float4 vertex : POSITION;
					fixed2 texcoord : TEXCOORD0;
					fixed2 texcoord1 : TEXCOORD1;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					fixed2 texcoord : TEXCOORD0;
					fixed2 texcoord1 : TEXCOORD1;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;
				sampler2D _MainTex2;
				float4 _MainTex2_ST;
				fixed _Mix;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _MainTex2);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					if(_Mix>0){
						fixed4 c = tex2D(_MainTex, i.texcoord);
						fixed4 tex2 = tex2D(_MainTex2, i.texcoord1);
						fixed alpha = _Mix;
						#ifndef ISREPLACE_ON
						alpha *= tex2.a;
						#endif
						c.rgb = lerp(c,tex2,alpha);
						return c;
					}
					else
					{
						return tex2D(_MainTex, i.texcoord);
					}
				}
			
			ENDCG
		}
	} 
	FallBack "Mobile/Diffuse"
}
