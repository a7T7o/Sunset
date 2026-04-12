Shader "Custom/NightVisionOverlay"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BaseColor ("Base Multiply Color", Color) = (1,1,1,1)
        _OuterColor ("Outer Multiply Color", Color) = (1,1,1,1)
        _VisionCenter ("Vision Center", Vector) = (0,0,0,0)
        _VisionParams ("Vision Params", Vector) = (100,100,0.2,0)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend DstColor Zero

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA

            #include "UnityCG.cginc"

            #define MAX_NIGHT_LIGHTS 8

            sampler2D _MainTex;
            sampler2D _AlphaTex;
            float _EnableExternalAlpha;

            fixed4 _BaseColor;
            fixed4 _OuterColor;
            float4 _VisionCenter;
            float4 _VisionParams;
            float4 _LightPositions[MAX_NIGHT_LIGHTS];
            float4 _LightColors[MAX_NIGHT_LIGHTS];
            float4 _LightData[MAX_NIGHT_LIGHTS];
            float _LightCount;

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float SmoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 worldPos : TEXCOORD1;
            };

            fixed4 SampleSpriteTexture(float2 uv)
            {
                fixed4 color = tex2D(_MainTex, uv);
                #if ETC1_EXTERNAL_ALPHA
                fixed4 alpha = tex2D(_AlphaTex, uv);
                color.a = lerp(color.a, alpha.r, _EnableExternalAlpha);
                #endif
                return color;
            }

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                float4 world = mul(unity_ObjectToWorld, IN.vertex);
                OUT.worldPos = world.xy;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 texColor = SampleSpriteTexture(IN.texcoord) * IN.color;

                float2 radii = max(_VisionParams.xy, float2(0.001, 0.001));
                float feather = max(_VisionParams.z, 0.001);
                float visionStrength = saturate(_VisionParams.w);

                float2 visionDelta = IN.worldPos - _VisionCenter.xy;
                float normalizedDistance = length(float2(visionDelta.x / radii.x, visionDelta.y / radii.y));
                float innerVisionEdge = max(0.18, 0.82 - feather * 0.42);
                float outerVisionEdge = 1.0 + feather * 0.45;
                float peripheralEdge = outerVisionEdge + 0.55 + feather * 0.35;
                float focusCore = 1.0 - smoothstep(innerVisionEdge, outerVisionEdge, normalizedDistance);
                float focusPeriphery = 1.0 - smoothstep(outerVisionEdge, peripheralEdge, normalizedDistance);
                float focusMask = saturate(focusCore * 0.84 + focusPeriphery * 0.16);
                focusMask = lerp(1.0, focusMask, visionStrength);

                fixed3 finalColor = lerp(_OuterColor.rgb, _BaseColor.rgb, focusMask);

                int lightCount = (int)_LightCount;
                [unroll]
                for (int i = 0; i < MAX_NIGHT_LIGHTS; i++)
                {
                    if (i >= lightCount)
                    {
                        break;
                    }

                    float2 lightDelta = IN.worldPos - _LightPositions[i].xy;
                    float radius = max(_LightData[i].x, 0.001);
                    float lightFeather = saturate(_LightData[i].y);
                    float intensity = saturate(_LightData[i].z);
                    float coreRatio = saturate(_LightData[i].w);

                    float seed = Hash21(_LightPositions[i].xy * 0.173 + float2(i * 1.13, i * 2.71));
                    float flicker =
                        0.9 +
                        0.1 * (
                            sin(_Time.y * (1.45 + seed * 1.35) + seed * 6.2831) * 0.55 +
                            sin(_Time.y * (2.35 + seed * 1.95) + seed * 11.21) * 0.45);

                    float2 flutterOffset = float2(
                        sin(_Time.y * (1.6 + seed * 1.4) + seed * 9.17),
                        cos(_Time.y * (1.12 + seed * 1.08) + seed * 5.13))
                        * radius * 0.035 * intensity;

                    float2 shapedDelta = lightDelta + flutterOffset;
                    float localNoise = SmoothNoise(
                        shapedDelta * (0.55 / max(radius, 0.001)) +
                        float2(seed * 7.31, _Time.y * (0.42 + seed * 0.23)));
                    float angle = atan2(shapedDelta.y, shapedDelta.x);
                    float angleNoise = sin(angle * 3.0 + _Time.y * (0.9 + seed) + seed * 6.2831);
                    float irregularScale = max(0.82, lerp(0.92, 1.08, localNoise) + angleNoise * 0.04 * intensity);

                    float distance01 = length(shapedDelta) / max(radius * irregularScale, 0.001);
                    float bloomDistance = distance01 / (1.08 + lightFeather * 0.9);
                    float mainDistance = distance01 / lerp(0.96, 0.72, lightFeather);

                    float outerBloom = exp(-bloomDistance * bloomDistance * 1.8);
                    float mainGlow = exp(-mainDistance * mainDistance * 2.9);

                    float coreRadius = max(radius * lerp(0.22, 0.42, coreRatio), 0.001);
                    float coreDistance01 = length(shapedDelta) / coreRadius;
                    float coreGlow = exp(-coreDistance01 * coreDistance01 * 3.8);

                    float emberNoise = SmoothNoise(
                        shapedDelta * (3.6 / max(radius, 0.001)) +
                        float2(seed * 13.1, _Time.y * (2.8 + seed)));
                    float emberMask = smoothstep(0.78, 0.96, emberNoise) * exp(-coreDistance01 * coreDistance01 * 10.5);

                    float haloMask = saturate((outerBloom * 0.55 + mainGlow * 0.78) * intensity * flicker);
                    float coreMask = saturate((coreGlow * 0.9 + emberMask * 0.5) * intensity);

                    fixed3 haloTint = lerp(_LightColors[i].rgb, saturate(_LightColors[i].rgb * 1.04 + 0.02), 0.25);
                    fixed3 emberTint = saturate(_LightColors[i].rgb * 1.08 + 0.035);
                    fixed3 haloColor = lerp(finalColor, haloTint, haloMask * 0.6);
                    fixed3 coreColor = lerp(haloColor, emberTint, coreMask * 0.62);
                    finalColor = coreColor;
                }

                return fixed4(finalColor * texColor.rgb, 1.0);
            }
            ENDCG
        }
    }
}
