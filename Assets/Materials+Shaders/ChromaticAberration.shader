Shader "Sarah/ChromaticAberration"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
        // [ShowAsVector2] _BlueOffset("_BlueOffset", Vector) = ( 0, 0)
    }
    CGINCLUDE
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

    v2f vert (appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }
    ENDCG

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            sampler2D _MainTex;
            float _xBlueOffset;
            float _yBlueOffset;
            float _xRedOffset;
            float _yRedOffset;

            fixed4 frag (v2f i) : COLOR {
				float2 uvR = float2(i.uv.x + _xRedOffset, i.uv.y + _yRedOffset);
				float2 uvG = float2(i.uv.x, i.uv.y);
				float2 uvB = float2(i.uv.x + _xBlueOffset, i.uv.y +_yBlueOffset);
				fixed4 colR = tex2D(_MainTex, uvR);
				fixed4 colG = tex2D(_MainTex, uvG);
				fixed4 colB = tex2D(_MainTex, uvB);
				return fixed4(colR.r, colG.g, colB.b, 0.8f);
			}
            ENDCG
        }
    }
}
