Shader "Farm Manager World/Vehicle Merged Snow ORM(Specular setup)"
{
	Properties
	{
		[NoScaleOffset]
		_Albedo("Albedo 1 (RGB), Alpha (A)", 2D) = "white" {}
		[NoScaleOffset]
		_Albedo2("Albedo 2 (RGB), Alpha (A)", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,0)

		_BlendAmount("Blend Amount", Range(0, 1)) = 0

		[NoScaleOffset]
		_ORM("Occlussion Roughnes Metallic 1", 2D) = "black" {}
		[NoScaleOffset]
		_ORM2("Occlussion Roughnes Metallic 2", 2D) = "black" {}


		[Normal]
		[NoScaleOffset]
		_Normal("Normal (RGB)", 2D) = "bump" {}
		[Normal]
		[NoScaleOffset]
		_Normal2("Normal 2 (RGB)", 2D) = "bump" {}

		[NoScaleOffset]
		_Emission("Emission (RGB)", 2D) = "black" {}
		[HDR]
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)

		_SnowColor("Color of snow", Color) = (1.0,1.0,1.0,1.0)
		_SnowMultip("Snow multip", Range(0,1)) = 1
		_Roughnes("Roughnes multiplier", Range(0,1)) = 0.3
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
			"RenderType" = "Opaque"
		}

		CGINCLUDE

		#define _GLOSSYENV 1
		#define UNITY_SETUP_BRDF_INPUT Standard
		#define UNITY_BRDF_CGX 1
		#define UNITY_GLOSS_MATCHES_MARMOSET_TOOLBAG2 1

		ENDCG

		CGPROGRAM
		#pragma target 4.0
		#pragma surface surf Standard 

		sampler2D _Albedo;
		sampler2D _Albedo2;
		sampler2D _Normal;
		sampler2D _Normal2;
		sampler2D _Emission;
		sampler2D _ORM;
		sampler2D _ORM2;
		half _Roughnes;

		fixed4 _Color;
		half _BlendAmount;
		fixed4 _EmissionColor;
		float _Snow;
		float _SnowMultip;
		float4 _SnowColor;
		float _SnowDepth;

		struct Input 
		{
			float2 uv_Albedo;
			float2 uv_MainTex;
			float2 uv2_DetailAlbedoMap;
			float3 worldNormal;
			float3 worldPos;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 albedo = lerp(tex2D(_Albedo, IN.uv_Albedo), tex2D(_Albedo2, IN.uv_Albedo), _BlendAmount)  * _Color.rgba;
			fixed4 orm = lerp(tex2D(_ORM, IN.uv_Albedo), tex2D(_ORM2, IN.uv_Albedo), _BlendAmount);
			fixed4 emission = tex2D(_Emission, IN.uv_Albedo);
			fixed3 normal = UnpackScaleNormal(lerp(tex2D(_Normal, IN.uv_Albedo), tex2D(_Normal2, IN.uv_Albedo), _BlendAmount), 1);

			float angle = dot(WorldNormalVector(IN, o.Normal), fixed3(0, 1, 0));

			o.Albedo = lerp(albedo.rgb, fixed3(1, 1, 1), clamp(angle * _Snow * _SnowMultip * step(1.2 - _Snow, angle), 0, 1));
			o.Alpha = albedo.a;
			o.Normal = normal;
			o.Emission = emission.rgb * _EmissionColor.rgb;

			o.Occlusion = orm.r;
			o.Smoothness = (1 - orm.g) * _Roughnes;
			o.Metallic = orm.b;
		}
		ENDCG
	}
	FallBack "Standard (Specular setup)"
}
