Shader "Unlit/ScoringDisplayShader"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct ScoreDisplayData
			{
				uint PlayerWins;
				uint OpponentWins;
			};

			StructuredBuffer<ScoreDisplayData> _ScoreData;

            struct v2f
            {
				float index : TEXCOORD1;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
            };

			uint _BoxCount;
			float4x4 _Transform;
			float _BoxMargin;
			float3 _PositionFixer;

			v2f vert(appdata_full v, uint instanceId : SV_InstanceID)
			{
				float index = (float)instanceId / _BoxCount;
				float4 offsetVert = v.vertex;
				float span = (float)1 / _BoxCount;
				float offset = (float)instanceId - (_BoxCount / 2);
				offset *= span;
				offsetVert.x *= span;
				offsetVert.x += offset;
				float4 transformedVert = mul(_Transform, offsetVert);
				transformedVert.xyz -= _PositionFixer;
				ScoreDisplayData scoreData = _ScoreData[instanceId];

                v2f o;
				o.uv = v.texcoord;
                o.vertex = UnityObjectToClipPos(transformedVert);
				o.index = index;
				o.normal = v.normal;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				return float4(i.normal, 1);
            }
            ENDCG
        }
    }
}
