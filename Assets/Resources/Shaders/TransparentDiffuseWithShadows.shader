Shader "Farm Manager World/Building/Fade With Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BumpMap ("Normalmap", 2D) = "bump" {}
        _BumpIntensity("NormalMap Intensity", Float) = 1
        _Cutoff2("Alpha Cutoff", Range(0,1)) = 0
        _Color("Tint", Color) = (1,1,1,1)
        _SnowMultip2("Snow multip", Range(0,1)) = 0
        
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        blend One OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Lambert vertex:vert nofog alphatest:_Cutoff2 addshadow
 
        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
        fixed _BumpIntensity;
        float _SnowMultip2;
        float _Snow;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            fixed4 color;
        };
       
        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = v.color * _Color;
        }
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            _BumpIntensity = 1 / _BumpIntensity;
            o.Normal.z = o.Normal.z * _BumpIntensity;
            o.Normal = normalize((half3)o.Normal);
            o.Albedo = lerp(c.rgb * c.a, fixed3(1, 1, 1) * c.a, _Snow * _SnowMultip2);

            o.Alpha = c.a;
        }
        ENDCG
    }
 
Fallback "Transparent/Cutout/VertexLit"
}
 