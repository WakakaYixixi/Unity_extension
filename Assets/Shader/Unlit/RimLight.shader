Shader "ZZL/Unlit/Rim Light" {
   
    Properties {
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _RimWidth ("Rim Width", Range(0.0, 1)) = 0.7
    }
   
    SubShader {
       
        Pass {
           
            CGPROGRAM
               
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"
                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                    float2 texcoord : TEXCOORD0;
                };
               
                struct v2f {
                    float4 pos : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 color : COLOR;
                };
               
                uniform float4 _MainTex_ST;
                uniform float4 _RimColor;
                uniform fixed _RimWidth;
               
                v2f vert (appdata_base v) {
                    v2f o;
                    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
                   
                    float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
                    float dotProduct = 1 - dot(v.normal, viewDir);
                    o.color = smoothstep(1 - _RimWidth, 1.0, dotProduct);
                   
                    o.color *= _RimColor;
                   
                    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                   
                    return o;
                }
               
                uniform sampler2D _MainTex;
               
                float4 frag(v2f i) : COLOR {
                    float4 col = tex2D(_MainTex, i.uv);
                    col.rgb += i.color;
                    return col;
                }
               
            ENDCG
        }
    }
}