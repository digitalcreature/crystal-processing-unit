Shader "Unlit/LaserDrillBeam"
{
	Properties
	{
		_Color ("Color", Color) = (1, 1, 1, 1)
		_Amp ("Amplitude", Float) = 0.5
		_Freq ("Frequency", Float) = 2
	}
	SubShader
	{

		//Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		Tags { "Queue" = "Transparent" }

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#define PI 3.1415926535

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
			};

			float4 _Color;
			float _Amp;
			float _Freq;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = v.vertex + (v.normal * (((sin(((_Time.y + v.uv.y) * (2 * PI)) * _Freq) + 1) / 2) * _Amp));
				o.vertex = mul(UNITY_MATRIX_MVP, o.vertex);
				o.normal = v.normal;
				o.uv = float2(0, 0);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				return _Color;// * (i.normal * 0.5 + 0.5);
			}
			ENDCG
		}
	}
}
