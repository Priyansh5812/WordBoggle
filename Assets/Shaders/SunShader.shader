Shader "Custom/SunShader"
{
    Properties
    {   
        _GlowIntensity ("Glow" ,Range(0 , 1)) = 0
        _AlphaClipAmount("Alpha Clip" , Range(0 , 1)) = 0.1
        _SunGlow ("SunGlow" ,Range(0 , 1)) = 0.1
        _ColorA ("Color A", Color) = (1,1,1,1)
        _MainTex ("Main Texture", 2D) = "white" {}   // Required for UI
        _OverlayTex ("Overlay", 2D) = "white" {}
        
    }

    SubShader
    {
        Tags 
        { 
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _OverlayTex;
            float4 _OverlayTex_ST;

            float4 _ColorA;
            float _GlowIntensity;
            float _SunGlow;
            float _AlphaClipAmount;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvOverlay : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // UI-correct UV mapping
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uvOverlay = TRANSFORM_TEX(v.uv, _OverlayTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 baseCol = tex2D(_MainTex, i.uv);

                if(baseCol.a <= _AlphaClipAmount)
                    baseCol.rgba = 0;

                float4 overlayCol = tex2D(_OverlayTex , i.uv) * _ColorA;
                overlayCol.a *= _GlowIntensity;

                float4 finalColor;
                if(baseCol.a < 0.1)
                    finalColor = overlayCol;
                else
                    finalColor = baseCol + float4(1,1,1,0) * _GlowIntensity * _SunGlow;

                return finalColor;
            }
            ENDCG
        }
    }
}