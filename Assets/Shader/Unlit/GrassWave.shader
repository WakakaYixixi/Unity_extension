Shader "ZZL/Unlit/Grass Wave"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveDistance("Wave Distance",Range(0,1))=0.25
		_WindSpeed("Wind Speed",Range(0,2))=0.4
//		_RockPower("RockPower",float)=1
		_Cutoff("alpha CutOff",Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="Transparent"}
		
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha 

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			half _WaveDistance;
			fixed _WindSpeed;
//			float _RockPower;
			fixed _Cutoff;
			
			float4 GrassRock(float4 pos, half2 uv ,float dir)
			{
				if (uv.y > 0.99){

					float windx = 0;
					float windz = 0;
					if ( dir >0 ){
						windx = sin(_Time.y * _WindSpeed)*_WaveDistance;
					}else{
						windz =  cos(_Time.y * _WindSpeed)*_WaveDistance;
					} 
//					if (uv1.x >0) {
//						float percentage = 1- uv1.x/(_totalTime);
//						if (dir < 0) {
//							windx += sin(uv1.x * _RockSpeed) * _RockPower * percentage ;
//							windz += cos(uv1.x * _RockSpeed) * _RockPower * percentage ;
//						}else{
//							windx -= sin(uv1.x * _RockSpeed) * _RockPower * percentage ;
//							windz -= cos(uv1.x * _RockSpeed) * _RockPower * percentage ;
//						}
//					} 
					pos.x += windx;
					pos.z += windz;
				}
				return pos;
			}

			struct appdata
			{
				float4 vertex : POSITION;
				half2 uv : TEXCOORD0;
			};

			struct v2f
			{
				half2 uv;
				float4 pos : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = GrassRock(o.pos, v.uv ,1);
//				o.uv = v.uv;
				o.uv = TRANSFORM_TEX(v.uv,_MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a-_Cutoff);
				return col;
			}
			ENDCG
		}
	}
}
