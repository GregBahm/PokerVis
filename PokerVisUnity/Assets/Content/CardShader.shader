Shader "Unlit/CardShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_CardIndex("_CardIndex", Float) = 0
			_SuitIndex("_SuitIndex", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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

            sampler2D _MainTex;
			float _CardIndex;
			float _SuitIndex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				i.uv += float2(_CardIndex, _SuitIndex);
				i.uv *= float2(1.0 / 13, 1.0 / 4);
                fixed4 col = tex2D(_MainTex, i.uv);
				clip(col.a - .5);
                return col;
            }
            ENDCG
        }
    }
}
