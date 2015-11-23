//树的简单摆动，通过顶点控制.
Shader "ZZL/Env/Tree Swing Simple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveSpeed("Wave Speed",float)=1
		_WaveX("Wave X",Range(0,2))=0.1
		_WaveZ("Wave Z",Range(0,2))=0
		_HeightChange("eightChange",Range(0,0.05))=0.02
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _WaveSpeed;
			fixed _WaveX;
			fixed _WaveZ;
			fixed _HeightChange;
			
			v2f vert (appdata v)
			{
				v2f o;
				
				fixed dx = sin(_Time.y*_WaveSpeed)*v.vertex.y;
				v.vertex.x += dx*_WaveX;
				v.vertex.z += dx*_WaveZ;
				v.vertex.y -= sin(abs(dx*_HeightChange));
				
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return col;
			}
			ENDCG
		}
	}
}
