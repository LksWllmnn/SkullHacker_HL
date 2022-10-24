Shader "Unlit/ChromaKeyUnlit"
{
	Properties
	{
		[HideInEditor] [NoScaleOffset] _YPlane("Y plane", 2D) = "black" {}
		[HideInEditor][NoScaleOffset] _UPlane("U plane", 2D) = "gray" {}
		[HideInEditor][NoScaleOffset] _VPlane("V plane", 2D) = "gray" {}


		_Border("BorderSetting",Range(0,1000)) = 100
		_KeyColor("Key Color", Color) = (0,1,0)
		_Near("Near", Range(0, 2)) = 0.01
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100
			Cull off
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
				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _Border;
				fixed4 _KeyColor;
				half _Near;
				fixed2 bound(fixed2 st, float i)
				{
					fixed2 p = floor(st) + i;
					return p;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					UNITY_TRANSFER_FOG(o,o.vertex);

					o.uv.x = 1 - v.uv.x;
					return o;
				}

				sampler2D _YPlane;
				sampler2D _UPlane;
				sampler2D _VPlane;

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

				fixed4 frag(v2f i) : SV_Target
				{
					half3 yuv;
					yuv.x = tex2D(_YPlane, i.uv);
					yuv.y = tex2D(_UPlane, i.uv);
					yuv.z = tex2D(_VPlane, i.uv);

					half3 rgb = yuv2rgb(yuv);

					fixed4 col = fixed4(rgb.x, rgb.y, rgb.z, 0.5);
					if (col[1] == 1 && col[0] == 0 && col[2] == 0) {
						col = fixed4(rgb.x, rgb.y, rgb.z, 0);
					}
					
					
					// apply fog
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
			}
		}
}