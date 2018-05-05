// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/PartHideShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	//-------------------add----------------------  
	_Switch1("Switch1", Int) = 0
		_MinX1("MinX1", Float) = -0.1
		_MaxX1("MaxX1", Float) = 0.1
		_MinY1("MinY1", Float) = -0.1
		_MaxY1("MaxY1", Float) = 0.1
		_Switch2("Switch2", Int) = 0
		_MinX2("MinX2", Float) = -0.3
		_MaxX2("MaxX2", Float) = -0.2
		_MinY2("MinY2", Float) = -0.3
		_MaxY2("MaxY2", Float) = -0.2
		//-------------------add----------------------  
	}
		SubShader
	{
		//-------------------add----------------------  
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
		//-------------------add----------------------  
		LOD 100
		//-------------------add----------------------  
		Blend SrcAlpha OneMinusSrcAlpha
		//-------------------add----------------------  
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

	//-------------------add----------------------  
	float _MinX1;
	float _MaxX1;
	float _MinY1;
	float _MaxY1;
	fixed _Switch1;
	fixed _ResultAlpha1;
	float _MinX2;
	float _MaxX2;
	float _MinY2;
	float _MaxY2;
	fixed _Switch2;
	fixed _ResultAlpha2;
	//-------------------add----------------------  

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		UNITY_TRANSFER_FOG(o,o.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		// sample the texture  
		//fixed4 col = tex2D(_MainTex, i.uv);  

		// apply fog  
		UNITY_APPLY_FOG(i.fogCoord, col);

	//-------------------add----------------------  
	fixed4 col = tex2D(_MainTex, i.uv);

	_ResultAlpha1 = 2.0f * col.a;
	_ResultAlpha1 *= (i.uv.x >= _MinX1);
	_ResultAlpha1 *= (i.uv.x <= _MaxX1);
	_ResultAlpha1 *= (i.uv.y >= _MinY1);
	_ResultAlpha1 *= (i.uv.y <= _MaxY1);
	_ResultAlpha1 = (1 - _ResultAlpha1) * _Switch1;

	_ResultAlpha2 = 2.0f * col.a;
	_ResultAlpha2 *= (i.uv.x >= _MinX2);
	_ResultAlpha2 *= (i.uv.x <= _MaxX2);
	_ResultAlpha2 *= (i.uv.y >= _MinY2);
	_ResultAlpha2 *= (i.uv.y <= _MaxY2);
	_ResultAlpha2 = (1 - _ResultAlpha2) * _Switch2;

	col.a = _ResultAlpha1 + _ResultAlpha2;
	//-------------------add----------------------  
	return col;
	}
		ENDCG
	}
	}
}