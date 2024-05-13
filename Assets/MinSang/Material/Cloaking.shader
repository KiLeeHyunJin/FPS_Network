Shader "Custom/Cloaking"
{
    Properties
    {
        _MainTex("Albeo texture", 2D) = "White" {}
        _NormalMap("Normal Map", 2D) = "bump" {}

        _Opacity("Opacity", Range(0,1)) = 0.1
        _DeformIntesity("Deform by Normal Intensity", Range(0, 3)) = 1

        _RimPow("Rim Power", int) = 3
        _RimColor("Rim Color", Color) = (0,1,1,1)
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Opaque" }
        // LOD 200
        zwrite off

        GrabPass{}

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf CloakingLight noambient novertexlights noforwardadd

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _GrabTexture;

        sampler2D _MainTex;
        sampler2D _NormalMap;

        float _DeformIntensity;
        float _Opacity;
        float _RimPow;
        float3 _RimColor;

        struct Input
        {
            float4 screenPos;
            float2 uv_MainTex;
            float2 uv_NormalMap;
            float3 viewDir;
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

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_NormalMap));

            float4 color = tex2D(_MainTex, IN.uv_MainTex);

            float2 uv_screen = IN.screenPos.xy / IN.screenPos.w;

            fixed3 mappingScreenColor = tex2D(_GrabTexture, uv_screen);

            float rimBrightness = 1 - saturate(dot(IN.viewDir, o.Normal));
            rimBrightness = pow(rimBrightness, _RimPow);
            
            o.Emission = mappingScreenColor * (1 - _Opacity) + _RimColor * rimBrightness;
            o.Albedo = color.rgb;
        }

        fixed4 LightingCloakingLight(SurfaceOutput s, float3 lightDir, float atten){
            return fixed4(s.Albedo * _Opacity * _LightColor0, 1);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
