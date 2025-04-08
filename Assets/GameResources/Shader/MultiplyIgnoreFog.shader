Shader "Custom/Particles/MultiplyNoFog"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" {}
        _Color ("Tint Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100

        // Multiplicative blending mode
        Blend DstColor OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR; // Particle system color
            };

            struct v2f
            {
                float2 texcoord : TEXCOORD0;
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color; // Pass particle color to the fragment shader
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the main texture
                fixed4 texColor = tex2D(_MainTex, i.texcoord);

                // Apply both the particle system color and the tint color
                fixed4 result = texColor * _Color * i.color;

                // Premultiply alpha for proper transparency handling
                result.rgb *= result.a;

                return result;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
