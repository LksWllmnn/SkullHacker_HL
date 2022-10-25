Shader "Unlit/testAlpha"
{
    Properties
    {
        [HideInEditor] [NoScaleOffset] _YPlane("Y plane", 2D) = "black" {}
        [HideInEditor][NoScaleOffset] _UPlane("U plane", 2D) = "gray" {}
        [HideInEditor][NoScaleOffset] _VPlane("V plane", 2D) = "gray" {}
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull front
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _YPlane;
            float4 _YPlane_ST;
            sampler2D _UPlane;
            float4 _UPlane_ST;
            sampler2D _VPlane;
            float4 _VPlane_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _YPlane);
                o.uv = TRANSFORM_TEX(v.uv, _UPlane);
                o.uv = TRANSFORM_TEX(v.uv, _VPlane);
                UNITY_TRANSFER_FOG(o,o.vertex);
                //o.uv.y = 1 - v.uv.y;
                o.uv.x = 1 - v.uv.x;
                return o;
            }

            half3 yuv2rgb(half3 yuv)
            {
                // The YUV to RBA conversion, please refer to: http://en.wikipedia.org/wiki/YUV
                // Y'UV420p (I420) to RGB888 conversion section.
                half y_value = yuv[0];
                half u_value = yuv[1];
                half v_value = yuv[2];
                half r = y_value + 1.370705 * (v_value - 0.5);
                half g = y_value - 0.698001 * (v_value - 0.5) - (0.337633 * (u_value - 0.5));
                half b = y_value + 1.732446 * (u_value - 0.5);
                return half3(r, g, b);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half3 yuv;
                yuv.x = tex2D(_YPlane, i.uv);
                yuv.y = tex2D(_UPlane, i.uv);
                yuv.z = tex2D(_VPlane, i.uv);
                
                // sample the texture
                //fixed4 col = tex2D(yuv2rgb(yuv), i.uv);
                fixed3 rgb = yuv2rgb(yuv);
                fixed4 col = fixed4(rgb[0], rgb[1], rgb[2], 1);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                if (col[1] >= 0.89 && col[0] <= 0.1 && col[2] <= 0.1) {
                    return fixed4(col[0], col[1], col[2], 0);
                }
                else {
                    return fixed4(col[0], col[1], col[2], 1);
                }
                
                
                
            }
            ENDCG
        }
    }
        Fallback "Transparent/VertexLit"
}
