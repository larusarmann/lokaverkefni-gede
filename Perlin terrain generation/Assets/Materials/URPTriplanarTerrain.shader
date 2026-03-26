Shader "Custom/URPTriplanarTerrain"
{
    Properties
    {
        [Header(Textures)]
        _SandTex("Sand Texture", 2D) = "white" {}
        _GrassTex("Grass Texture", 2D) = "white" {}
        _SnowTex("Snow Texture", 2D) = "white" {}
        
        [Header(Height Settings)]
        _SandHeight("Sand Max Height", Float) = 7.0
        _GrassHeight("Grass Max Height", Float) = 25.0
        _BlendDistance("Blend Smoothness", Float) = 4.0
        
        [Header(Scaling)]
        _TextureScale("Texture Scale", Float) = 0.05
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        
        // --- PASS 1: Main Lighting and Color ---
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Enable URP Shadows
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float3 positionWS   : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float4 shadowCoord  : TEXCOORD2;
            };

            TEXTURE2D(_SandTex);  SAMPLER(sampler_SandTex);
            TEXTURE2D(_GrassTex); SAMPLER(sampler_GrassTex);
            TEXTURE2D(_SnowTex);  SAMPLER(sampler_SnowTex);

            CBUFFER_START(UnityPerMaterial)
                float _SandHeight;
                float _GrassHeight;
                float _BlendDistance;
                float _TextureScale;
            CBUFFER_END

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionHCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                
                // Get coordinates for shadows
                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.positionOS.xyz);
                output.shadowCoord = GetShadowCoord(vertexInput);
                
                return output;
            }

            float InverseLerp(float a, float b, float value)
            {
                return saturate((value - a) / (b - a));
            }

            // The Triplanar Magic
            float4 Triplanar(TEXTURE2D_PARAM(tex, samp), float3 positionWS, float3 blendAxes)
            {
                float3 scaledPos = positionWS * _TextureScale;
                
                // Project from X, Y, and Z sides
                float4 xProj = SAMPLE_TEXTURE2D(tex, samp, scaledPos.zy);
                float4 yProj = SAMPLE_TEXTURE2D(tex, samp, scaledPos.xz);
                float4 zProj = SAMPLE_TEXTURE2D(tex, samp, scaledPos.xy);
                
                // Blend based on the normal angle
                return xProj * blendAxes.x + yProj * blendAxes.y + zProj * blendAxes.z;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float3 normalWS = normalize(input.normalWS);
                
                // Calculate which direction the face is pointing
                float3 blendAxes = abs(normalWS);
                blendAxes /= (blendAxes.x + blendAxes.y + blendAxes.z);

                // Sample our 3 textures
                float4 sandCol = Triplanar(TEXTURE2D_ARGS(_SandTex, sampler_SandTex), input.positionWS, blendAxes);
                float4 grassCol = Triplanar(TEXTURE2D_ARGS(_GrassTex, sampler_GrassTex), input.positionWS, blendAxes);
                float4 snowCol = Triplanar(TEXTURE2D_ARGS(_SnowTex, sampler_SnowTex), input.positionWS, blendAxes);

                // Blend them purely based on world Y height
                float h = input.positionWS.y;
                float sandGrassBlend = InverseLerp(_SandHeight - _BlendDistance, _SandHeight + _BlendDistance, h);
                float grassSnowBlend = InverseLerp(_GrassHeight - _BlendDistance, _GrassHeight + _BlendDistance, h);

                float4 albedo = lerp(sandCol, grassCol, sandGrassBlend);
                albedo = lerp(albedo, snowCol, grassSnowBlend);

                // Get URP Main Light and Shadows
                Light mainLight = GetMainLight(input.shadowCoord);
                half NdotL = saturate(dot(normalWS, mainLight.direction));
                half3 lighting = mainLight.color * (NdotL * mainLight.shadowAttenuation);
                
                // Add Ambient sky lighting so shadows aren't pitch black
                half3 ambient = SampleSH(normalWS);

                half3 finalColor = albedo.rgb * (lighting + ambient);

                return half4(finalColor, 1.0);
            }
            ENDHLSL
        }
        
        // --- PASS 2: Shadow Caster (Unity 6 Compliant) ---
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode"="ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // Required for point/spot light shadows in URP
            #pragma multi_compile_vertex _ _CASTING_PUNCTUAL_LIGHT_SHADOW

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"

            // DECLARE THE LIGHT VARIABLES HERE SO UNITY CAN PASS THEM IN
            float4 _LightDirection;
            float4 _LightPosition;

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                
                // Calculate World Position and Normal
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);

                // Unity 6 requires manual shadow bias calculation
                #if _CASTING_PUNCTUAL_LIGHT_SHADOW
                    float3 lightDirectionWS = normalize(_LightPosition.xyz - positionWS);
                #else
                    float3 lightDirectionWS = _LightDirection.xyz;
                #endif

                output.positionHCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));
                
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                return 0; // Shadows only care about geometry, not color
            }
            ENDHLSL
        }
    }
}