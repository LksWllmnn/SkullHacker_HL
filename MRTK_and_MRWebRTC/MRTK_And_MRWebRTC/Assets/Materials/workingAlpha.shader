Shader "Unlit/workingAlpha"
{
    Properties{
   _Color("Main Color", Color) = (1,1,1,1)
   _MainTex("Base (RGB) Trans (A)", 2D) = "white" {}
    }
        SubShader{
             Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
             LOD 200
             Cull off

        CGPROGRAM
        #pragma surface surf Lambert alpha
        sampler2D _MainTex;
        float4 _Color;
        struct Input {
             float2 uv_MainTex;
        };
        void surf(Input IN, inout SurfaceOutput o) {

            float2 cv1 = IN.uv_MainTex;
            float2 cv2 = IN.uv_MainTex;

            cv1.x *= 0.5;
            cv2.x *= 0.5;
            cv2.x += 0.5;

            half4 c = tex2D(_MainTex, cv1) * _Color;
            o.Albedo = c.rgb;
            o.Alpha = c.a * tex2D(_MainTex, cv2).r;
        }
        ENDCG
    }
        Fallback "Transparent/VertexLit"
}
