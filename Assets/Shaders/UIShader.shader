Shader "Custom/MenuBG"
{
    Properties
    {  _ColorA ("Color A" ,Color) = (1,1,1,1)
       _ColorB ("Color B" ,Color) = (1,1,1,1)
       _UVScale ("UvScale" , Range(0 , 10)) = 1
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            //sampler2D _MainTex;
            float4 _ColorA;
            float4 _ColorB;
            float _UVScale;
            float4 frag (v2f i) : SV_Target
            {
                //fixed4 col = tex2D(_MainTex, i.uv);
                float t = saturate((i.uv.x + i.uv.y) * 0.5 * _UVScale);
                float4 color = lerp(_ColorA,_ColorB,t);
                return color;
            }
            ENDCG
        }
    }
}
