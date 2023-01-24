Shader "Unlit/uVlahosDepth"
{
    Properties
    {
        [HideInEditor] [NoScaleOffset] _YPlane("Y plane", 2D) = "black" {}
        [HideInEditor][NoScaleOffset] _UPlane("U plane", 2D) = "gray" {}
        [HideInEditor][NoScaleOffset] _VPlane("V plane", 2D) = "gray" {}

        [HideInEditor][NoScaleOffset] _DepthPlane("Depth plane", 2D) = "white" {}

        _a1("a1", float) = 0
        _a2("a2", float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
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
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        struct v2f
        {
            float2 uv : TEXCOORD0;
            float2 uv2 : TEXCOORD1;
            UNITY_FOG_COORDS(2)
            float4 vertex : SV_POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

        struct fragOutput {
            fixed4 color : SV_Target;
            fixed depth : SV_Depth;
        };

        sampler2D _YPlane;
        float4 _YPlane_ST;
        sampler2D _UPlane;
        float4 _UPlane_ST;
        sampler2D _VPlane;
        float4 _VPlane_ST;
        sampler2D _DepthPlane;
        float4 _DepthPlane_ST;

        float _a1;
        float _a2;

        v2f vert(appdata v)
        {
            v2f o;

            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            o.vertex = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _YPlane);
            o.uv = TRANSFORM_TEX(v.uv, _UPlane);
            o.uv = TRANSFORM_TEX(v.uv, _VPlane);
            o.uv2 = TRANSFORM_TEX(v.uv, _DepthPlane);
            UNITY_TRANSFER_FOG(o, o.vertex);

            o.uv.y = 1 - v.uv.y;
            o.uv.x = 1 - v.uv.x;
            o.uv2.x = 1 - v.uv.x;
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

        fragOutput frag(in v2f i)
        {
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            fragOutput o;

            fixed4 depthBack = tex2D(_DepthPlane, i.uv2);

            half3 yuv;
            yuv.x = tex2D(_YPlane, i.uv);
            yuv.y = tex2D(_UPlane, i.uv);
            yuv.z = tex2D(_VPlane, i.uv);

            float alpha = 1;

            // sample the texture
            fixed3 rgb = yuv2rgb(yuv);
            fixed4 col = fixed4(rgb[0], rgb[1], rgb[2], 1);

            if (unity_StereoEyeIndex == 0) {
                alpha = 1 - _a1 * (col[1] - _a2 * col[2]);
                o.color = fixed4(col[0], col[1], col[2], alpha);
                o.depth = alpha * depthBack[0];
                return o;
            }
            else {
                o.color = fixed4(0, 0, 0, 0);
                o.depth = 0;
                return o;
            }

            return o;
        }
            ENDCG
        }
    }
    Fallback "Transparent/VertexLit"
}
