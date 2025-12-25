Shader "Custom/CloudShadowMultiply"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Intensity ("Shadow Intensity", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent+100"
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        // Multiply blend: result = src * dst (darkens the background)
        Blend DstColor Zero

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _Color;
            float _Intensity;

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
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag (v2f IN) : SV_Target
            {
                fixed4 texC = tex2D(_MainTex, IN.texcoord);
                
                // Calculate shadow darkness based on texture alpha and intensity
                // White (1,1,1) = no shadow, darker = more shadow
                float shadowStrength = texC.a * IN.color.a * _Intensity;
                
                // Multiply blend: output color that will darken the background
                // 1.0 = no change, 0.0 = full black
                float darkness = 1.0 - shadowStrength;
                
                return float4(darkness, darkness, darkness, 1.0);
            }
            ENDCG
        }
    }
    
    Fallback "Sprites/Default"
}
