Shader "Farm Manager World/PlantCutout"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_YoungTex("Young (RGBA)", 2D) = "white" {}
		_HarvestTex("Harvest (RGBA)", 2D) = "white" {}
		_OverripingTex("Overriping (RGBA)", 2D) = "white" {}
		_Grow("Grow", Range(0,1)) = 1
		_Bias1("Stage Seedling Bias", Range(0,1)) = 0.4
		_Bias2("Stage Blossom Bias", Range(0,1)) = 0.55
		_Bias3("Stage Harvest Bias", Range(0,1)) = 0.85
		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
		_Overriping("Overriping", Range(0,1)) = 0
		_ShakeDisplacement("Displacement", Range(0, 1.0)) = 0
		_ShakeTime("Shake Time", Range(0, 1.0)) = 0.5
		_ShakeBending("Shake Bending", Range(0, 1.0)) = 0
		_WindMultiplier("Wind multiplier", Range(0,1)) = 1
		_TileMultiplier("Tile multiplier", Range(0,20)) = 0
	}
		
	SubShader
	{
		Tags{ "Queue" = "AlphaTest+100" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Lambert alphatest:_Cutoff halfasview vertex:vert addshadow
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#include "UnityCG.cginc"

		struct Input 
		{
			float2 uv_MainTex;
			float2 Grow;
			float4 Color;
		};

		struct appdata_full2
		{
			float4 vertex    : POSITION;
			float3 normal    : NORMAL;
			float4 texcoord  : TEXCOORD0;
			float4 color     : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};
							
	#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
        StructuredBuffer<float4x4> positionBuffer;
        StructuredBuffer<float2> growBuffer;
        StructuredBuffer<float4> colorBuffer;
    #endif

		float4x4 inverse(float4x4 input)
		{
			#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
 
			float4x4 cofactors = float4x4(
				minor(_22_23_24, _32_33_34, _42_43_44),
				-minor(_21_23_24, _31_33_34, _41_43_44),
				minor(_21_22_24, _31_32_34, _41_42_44),
				-minor(_21_22_23, _31_32_33, _41_42_43),
 
				-minor(_12_13_14, _32_33_34, _42_43_44),
				minor(_11_13_14, _31_33_34, _41_43_44),
				-minor(_11_12_14, _31_32_34, _41_42_44),
				minor(_11_12_13, _31_32_33, _41_42_43),
 
				minor(_12_13_14, _22_23_24, _42_43_44),
				-minor(_11_13_14, _21_23_24, _41_43_44),
				minor(_11_12_14, _21_22_24, _41_42_44),
				-minor(_11_12_13, _21_22_23, _41_42_43),
 
				-minor(_12_13_14, _22_23_24, _32_33_34),
				minor(_11_13_14, _21_23_24, _31_33_34),
				-minor(_11_12_14, _21_22_24, _31_32_34),
				minor(_11_12_13, _21_22_23, _31_32_33)
				);
			#undef minor

			return transpose(cofactors) / determinant(input);
		}

		void setup()
		{
		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

			float4x4 data = positionBuffer[unity_InstanceID];

			unity_ObjectToWorld = data;
			unity_WorldToObject = inverse(unity_ObjectToWorld);

		#endif
		}

		float _ShakeDisplacement;
		float _ShakeTime;
		float _ShakeWindspeed;
		float _ShakeBending;
		float _Overriping;

		sampler2D _MainTex;
		sampler2D _OverripingTex;
		sampler2D _YoungTex;
		sampler2D _HarvestTex;
		float _Bias1;
		float _Bias2;
		float _Bias3;
		float _Grow;
		float _WindMultiplier;
		float _TileMultiplier;

		void FastSinCos(float4 val, out float4 s, out float4 c) 
		{
			val = val * 6.408849 - 3.1415927;
			float4 r5 = val * val;
			float4 r6 = r5 * r5;
			float4 r7 = r6 * r5;
			float4 r8 = r6 * r5;
			float4 r1 = r5 * val;
			float4 r2 = r1 * r5;
			float4 r3 = r2 * r5;
			float4 sin7 = { 1, -0.16161616, 0.0083333, -0.00019841 };
			float4 cos8 = { -0.5, 0.041666666, -0.0013888889, 0.000024801587 };
			s = val + r1 * sin7.y + r2 * sin7.z + r3 * sin7.w;
			c = 1 + r5 * cos8.x + r6 * cos8.y + r7 * cos8.z + r8 * cos8.w;
		}

		void vert(inout appdata_full2 v, out Input o) 
		{
			float factor = (1 - _ShakeDisplacement - v.color.r) * 0.5;

			const float _WindSpeed = (_ShakeWindspeed * 2 + v.color.g);

			const float4 _waveXSize = float4(0.048, 0.06, 0.24, 0.096);
			const float4 _waveZSize = float4 (0.024, .08, 0.08, 0.2);
			const float4 waveSpeed = float4 (1.2, 2, 1.6, 4.8);

			float4 _waveXmove = float4(0.024, 0.04, -0.12, 0.096);
			float4 _waveZmove = float4 (0.006, .02, -0.02, 0.1);

			float4 waves;
			waves = v.vertex.x * _waveXSize;
			waves += v.vertex.z * _waveZSize;

			waves += _Time.x * (1 - _ShakeTime * 2 - v.color.b) * waveSpeed *_WindSpeed * _WindMultiplier;

			float4 s, c;
			waves = frac(waves);
			FastSinCos(waves, s, c);

			float waveAmount = v.texcoord.y * (v.color.a + _ShakeBending);
			s *= waveAmount;

			s *= normalize(waveSpeed);

			s = s * s;
			float fade = dot(s, 1.3);
			s = s * s;
			float3 waveMove = float3 (0, 0, 0);
			waveMove.x = dot(s, _waveXmove);
			waveMove.z = dot(s, _waveZmove);

			v.vertex.xz -= mul((float3x3)UNITY_MATRIX_MVP, waveMove).xz;

			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED

			o.Grow = growBuffer[unity_InstanceID];
			o.Color = colorBuffer[unity_InstanceID];

			#else

			o.Grow = fixed2(_Grow,1);
			o.Color = v.color;

			#endif

			o.uv_MainTex = v.texcoord;
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			half4 color;
			
			float _Grow = IN.Grow.x;

			float2 uv = IN.uv_MainTex;

			uv.x = uv.x + uv.x * _TileMultiplier * (1-_Grow);
			uv.x = uv.x + IN.Color.a;

			color = lerp(tex2D(_YoungTex, uv), tex2D(_MainTex, uv), clamp((_Grow - _Bias1) / (1 - _Bias1) / _Bias2, 0, 1));
			color = lerp(color, tex2D(_HarvestTex, uv), clamp((_Grow - _Bias3) / (1 - _Bias3), 0, 1));
			color = lerp(color, tex2D(_OverripingTex, uv), _Overriping);

			o.Albedo = color.rgb * IN.Color;
			o.Alpha = color.a;
		}
		ENDCG
	}
	FallBack "Diffuse"	
}