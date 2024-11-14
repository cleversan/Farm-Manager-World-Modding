Shader "Farm Manager World/Building/Building ORM"
{
	Properties
	{
		_MainTex("Albedo (RGB), Alpha (A)", 2D) = "white" {}

		_Color("Color", Color) = (0,0,0,0)

		[Normal]
		_BumpMap("Normal (RGB)", 2D) = "bump" {}

		[NoScaleOffset]
		_DetailAlbedoMap("Detail UV2", 2D) = "grey" {}

		_ORM("Occlussion Roughnes Metallic", 2D) = "black" {}

		_SnowColor("Color of snow", Color) = (1.0,1.0,1.0,1.0)
		_SnowMultip("Snow multip", Range(0,1)) = 1
		_Smoothness("Smoothness", Range(0,1)) = 1
		_ColorBase("Color Base", Color) = (1,1,1,1)
		_WorldY("Clip Below World Y", float) = 4
		_Metalic("Metalic multiplier", Range(0,1)) = 1
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
		ENDCG


		CGPROGRAM
		#pragma target 3.0
		#pragma surface surf Standard addshadow  
		#pragma multi_compile_instancing

		sampler2D _MainTex;
		sampler2D _BumpMap;

		sampler2D _DetailAlbedoMap;
		sampler2D _ORM;
		half4 _Color;


		half _Snow;
		half _SnowMultip;
		half4 _SnowColor;
		half4 _MainColor;
		half _SnowDepth;
		half _Smoothness;
		half _WorldY;
		half _Metalic;
		half _Roughnes;

		struct Input 
		{
			half2 uv_MainTex;
			half2 uv2_DetailAlbedoMap;
			half3 worldNormal;
			half3 worldPos;
			INTERNAL_DATA
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			half4 albedo = tex2D(_MainTex, IN.uv_MainTex) * half4(_Color.rgb,1);
			half3 color = albedo.rgb * tex2D(_DetailAlbedoMap, IN.uv2_DetailAlbedoMap).rgb * unity_ColorSpaceDouble.rgb;

			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));

			half angle = dot(WorldNormalVector(IN, o.Normal), half3(0, 1, 0));

			o.Albedo = lerp(color, half3(1, 1, 1), clamp(angle * _Snow * _SnowMultip * step(1.2 - _Snow, angle), 0, 1));

			half4 orm = tex2D(_ORM, IN.uv_MainTex);
			o.Alpha = albedo.a;

			o.Metallic = orm.b * _Metalic;
			o.Smoothness = (1 - orm.g) * _Roughnes;
			o.Occlusion = orm.r;
		}
		ENDCG
	}
	FallBack "Standard (Specular setup)"
}
