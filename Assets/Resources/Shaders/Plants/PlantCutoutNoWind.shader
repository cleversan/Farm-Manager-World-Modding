// Upgrade NOTE: replaced 'UNITY_INSTANCE_ID' with 'UNITY_VERTEX_INPUT_INSTANCE_ID'

Shader "Farm Manager World/PlantCutoutNoWind"
{
	Properties
	{
		_Grow("Grow", Range(0,1)) = 1

		_MainTex("Texture", 2D) = "white" {}
		_YoungTex("Young (RGBA)", 2D) = "white" {}
		_HarvestTex("Harvest (RGBA)", 2D) = "white" {}
		_OverripingTex("Overriping (RGBA)", 2D) = "white" {}

		_Bias1("Stage Seedling Bias", Range(0,1)) = 0.4
		_Bias2("Stage Blossom Bias", Range(0,1)) = 0.55
		_Bias3("Stage Harvest Bias", Range(0,1)) = 0.85

		_Cutoff("Base Alpha cutoff", Range(0,.9)) = .5
		_Overriping("Overriping", Range(0,1)) = 0
		_TileMultiplier("Tile multiplier", Range(0,20)) = 1
	}

	SubShader
	{
		Tags{ "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" }
		LOD 200

		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Lambert alphatest:_Cutoff addshadow vertex:vert
		#pragma multi_compile_instancing
		#pragma instancing_options procedural:setup
		#include "UnityCG.cginc"

		struct Input 
		{
			float2 uv_MainTex;
			float2 Grow;
			float4 Color;
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

		struct appdata_full2
		{
			float4 vertex    : POSITION;
			float3 normal    : NORMAL;
			float4 texcoord  : TEXCOORD0;
			float4 color     : COLOR;
			UNITY_VERTEX_INPUT_INSTANCE_ID
		};

		float _Overriping;

		sampler2D _MainTex;
		sampler2D _OverripingTex;
		sampler2D _YoungTex;
		sampler2D _HarvestTex;
		float _Bias1;
		float _Bias2;
		float _Bias3;
		float _Grow;
		float _TileMultiplier;

					
		void vert(inout appdata_full2 v, out Input o) 
		{				
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			
			o.Grow = growBuffer[unity_InstanceID];
			o.Color = colorBuffer[unity_InstanceID];
			
			#else

			o.Grow = fixed2(_Grow,1);
			o.Color =fixed4(1,1,1,1);
			
			#endif

			o.uv_MainTex = v.texcoord;
		}

		void surf(Input IN, inout SurfaceOutput o) 
		{
			float _Grow = IN.Grow.x;

			half4 color;

			float2 uv = IN.uv_MainTex;

			uv.x = uv.x + uv.x * _TileMultiplier * (1-_Grow);

			color = lerp(tex2D(_YoungTex, uv), tex2D(_MainTex, uv), clamp((_Grow - _Bias1) / (1 - _Bias1) / _Bias2, 0, 1));
			color = lerp(color, tex2D(_HarvestTex, uv), clamp((_Grow - _Bias3) / (1 - _Bias3), 0, 1));
			color = lerp(color, tex2D(_OverripingTex, uv), _Overriping);

			o.Albedo = color.rgb;
			o.Alpha = color.a;
		}

		ENDCG
	}
		
	FallBack "Diffuse"	
}