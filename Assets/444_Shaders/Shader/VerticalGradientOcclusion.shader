Shader "Custom/VerticalGradientOcclusion"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        // Gradient splits (0=bottom, 1=top)
        _Split1 ("Split 1 (Bottom→Middle)", Range(0,1)) = 0.33
        _Split2 ("Split 2 (Middle→Top)", Range(0,1)) = 0.66
        _Softness ("Transition Softness", Range(0,0.2)) = 0.05

        // Target alphas when occluded
        _BottomAlpha ("Bottom Alpha", Range(0,1)) = 0.8
        _MiddleAlpha ("Middle Alpha", Range(0,1)) = 0.5
        _TopAlpha ("Top Alpha", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        // Pre-multiplied alpha blending (match Sprites-Default behavior)
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;

            float _Split1;
            float _Split2;
            float _Softness;
            float _BottomAlpha;
            float _MiddleAlpha;
            float _TopAlpha;

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
            };

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color; // SpriteRenderer color × material tint
                return OUT;
            }

            // Smooth piecewise alpha: bottom->middle in [0, _Split1], middle->top in [_Split1, _Split2]
            float GradientAlpha(float t)
            {
                float s1 = saturate(_Split1);
                float s2 = saturate(_Split2);
                s2 = max(s2, s1 + 1e-4);
                float soft = _Softness;

                if (t <= s1)
                {
                    float k = smoothstep(0.0, s1 + soft, t);
                    return lerp(_BottomAlpha, _MiddleAlpha, k);
                }
                else if (t <= s2)
                {
                    float k = smoothstep(s1 - soft, s2 + soft, t);
                    return lerp(_MiddleAlpha, _TopAlpha, k);
                }
                else
                {
                    return _TopAlpha;
                }
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 texC = tex2D(_MainTex, IN.texcoord);

                // Base albedo (no premultiply yet)
                float3 albedo = texC.rgb * IN.color.rgb;        // keep sprite tinting
                float baseAlpha = texC.a;                        // DO NOT multiply by vertex alpha here

                // Occlusion amount comes from SpriteRenderer.color alpha (via IN.color.a)
                // a=1 → occlude=0 (no effect), a<1 → occlude>0 (apply gradient)
                float occlude = saturate(1.0 - IN.color.a);

                // Gradient target alpha along UV.y (0 bottom → 1 top)
                float gradient = GradientAlpha(IN.texcoord.y);

                // Final alpha: normal = baseAlpha; occluded = baseAlpha * gradient
                float finalAlpha = baseAlpha * lerp(1.0, gradient, occlude);

                // PREMULTIPLIED OUTPUT: multiply RGB by final alpha to avoid any full-rect tint/halo
                float3 finalRGB = albedo * finalAlpha;
                return float4(finalRGB, finalAlpha);
            }
            ENDCG
        }
    }
}
