Shader "Custom/01WaterShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#define GERSTNER

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		UNITY_INSTANCING_BUFFER_END(Props)

		int _NumWaves;
		float4 _WaveData[20];
		float4 _WaveDirections[20];
		float _WaveIntensity[20];

		float2 GetDirection (appdata_full v, int i)
		{
			// float2 pnt = v.vertex.xz;
			// float2 dir = _WaveDirections[i].xy;
			// float waveShape1 = _WaveDirections[i].z;
			// float waveShape2 = _WaveDirections[i].w;
			// float2 d = normalize(waveShape1 * pnt + waveShape2 * dir);

			return _WaveDirections[i].xy;
		}

		float DirectionDotProduct (appdata_full v, int i)
		{
			// float2 pnt = v.vertex.xz;
			// float2 dir = _WaveDirections[i].xy;
			// float waveShape1 = _WaveDirections[i].z;
			// float waveShape2 = _WaveDirections[i].w;
			// float2 d = normalize(waveShape1 * pnt + waveShape2 * dir);

			// return dot(pnt, d);
			return dot (GetDirection(v, i), v.vertex.xz);
		}

		float CalculateWaveHeight (appdata_full v, int i)
		{
			float A = _WaveData[i].x;
			float w = _WaveData[i].y;
			float phi = _WaveData[i].z;
			float I = _WaveIntensity[i];

			return I * A * sin(DirectionDotProduct(v, i) * w + phi * _Time.y);
		}

		float2 CalculateGerstner(appdata_full v, int i)
		{
			float A = _WaveData[i].x;
			float w = _WaveData[i].y;
			float phi = _WaveData[i].z;
			float q = _WaveData[i].w;
			float I = _WaveIntensity[i];
			float2 dir = GetDirection(v, i);
			float x = q * I * A * dir.x * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);
			float z = q * I * A * dir.y * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);

			return float2(x, z);
		}

		float3 CalculateNormal (appdata_full v, int i)
		{
			float A = _WaveData[i].x;
			float w = _WaveData[i].y;
			float phi = _WaveData[i].z;
			float q = _WaveData[i].w;
			float I = _WaveIntensity[i];
			float2 dir = GetDirection(v, i);
			#ifdef GERSTNER
				float x = dir.x * w * I * A * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);
				float z = dir.y * w * I * A * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);
				float y = 1 - q * w * I * A * sin(DirectionDotProduct(v, i) * w + phi * _Time.y);

				return normalize(float3(x, y, z));
			#else
				float x = dir.x * w * I * A * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);
				float z = dir.y * w * I * A * cos(DirectionDotProduct(v, i) * w + phi * _Time.y);

				return normalize(float3(-x, 1, -z));
			#endif
		}

		void vert (inout appdata_full v)
		{
			for (int i = 0; i < _NumWaves; i ++)
			{
				v.vertex.y += CalculateWaveHeight(v, i);
				#ifdef GERSTNER
					v.vertex.xz += CalculateGerstner(v, i);
				#endif
			}

			if(_NumWaves == 0)
				v.normal = float3(0, 1., 0);
			else
			{
				v.normal = float3(0, 0, 0);

				for (int j = 0; j < _NumWaves; j ++)
					v.normal += CalculateNormal(v, j);
			}
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
