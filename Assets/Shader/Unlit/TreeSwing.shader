//树的简单摆动，通过顶点控制.
Shader "ZZL/Env/Tree Swing Simple"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_WaveSpeed("Wave Speed",float)=1
		_WaveX("Wave X",Range(0,1))=0.1
		_WaveZ("Wave Z",Range(0,1))=0
		_HeightChange("Height Change",Range(0,0.05))=0.01
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
//			float4 _MainTex_ST;
			half _WaveSpeed;
			fixed _WaveX;
			fixed _WaveZ;
			fixed _HeightChange;
			
			
			float4x4 rotate(float3 r, float4 d) // r=rotations axes
			{
				float cx, cy, cz, sx, sy, sz;
				sincos(r.x, sx, cx);
				sincos(r.y, sy, cy);
				sincos(r.z, sz, cz);	
				return float4x4( cy*cz, -sz, sy, d.x,
						sz, cx*cz, -sx, d.y,
						-sy, sx, cx*cy, d.z,
						0, 0, 0, d.w );		
			}
 
			v2f vert (appdata v)
			{
				v2f o;
				
				half pan = sin(_Time.y*_WaveSpeed)*v.vertex.y;
				v.vertex.x += pan*_WaveX;
				v.vertex.z += pan*_WaveZ;
				v.vertex.y -= abs(pan)*_HeightChange;

				float4x4 rot_mat = rotate(float3(pan*_WaveX,pan*_WaveZ,0),float4(0,0,0,1));
				v.vertex = mul(rot_mat,v.vertex);
				
				o.vertex = mul(UNITY_MATRIX_MVP,v.vertex);
//				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = v.uv;
				
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
