Shader "Custom/VisionMask2D"
{
    Properties
    {
        _Color ("Mask Color", Color) = (0,0,0,1)
        _Center ("Center", Vector) = (0,0,0,0)
        _Radius ("Radius", Float) = 150
        _Softness ("Softness", Float) = 30
    }

    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float4 _Center;   // screen position (pixel)
            float _Radius;
            float _Softness;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
{
    float2 screenPos = (i.screenPos.xy / i.screenPos.w) * _ScreenParams.xy;
    float dist = distance(screenPos, _Center.xy);

    // 🔴 alpha = 1 ด้านนอก / 0 ด้านใน
    float alpha = smoothstep(_Radius - _Softness, _Radius, dist);
    alpha *= 0.7;
    return fixed4(_Color.rgb,_Color.a * alpha);
}
            ENDCG
        }
    }
}
