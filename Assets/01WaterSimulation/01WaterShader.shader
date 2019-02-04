Shader "Unlit/01WaterShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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

			int _NumWaves;
			float4 _WaveData[20];
			float4 _WaveDirections[20];

			float DirectionDotProduct (appdata v, int i)
			{
				float2 pnt = v.vertex.xz;
				float2 dir = _WaveDirections[i].xz;
				float waveShape = _WaveDirections[i].w;
				float2 d = normalize(waveShape * pnt + (1 - 2 * waveShape) * dir);

				return dot(pnt, d);
			}

			float CalculateWaveHeight (appdata v, int i)
			{
				float A = _WaveData[i].x;
				float w = _WaveData[i].y;
				float phi = _WaveData[i].z;
				float I = _WaveData[i].w;

				return I * A * sin(DirectionDotProduct(v, i) * w + phi * _Time);
			}
			
			v2f vert (appdata v)
			{
				v2f o;

				for (int i = 0; i < _NumWaves; i ++)
					v.vertex.y += CalculateWaveHeight(v, i);
				
				o.vertex = UnityObjectToClipPos(v.vertex);
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
