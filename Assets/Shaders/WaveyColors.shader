Shader "Sarah/WaveyColors"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _PrimaryColor ("Color", Color) = (1,1,0,1)
        _SecondaryColor ("Color", Color) = (1,0,0,1)
        _NumXWaves ("NumXWaves", Range(1, 30)) = 8
        _NumYWaves ("NumYWaves", Range(1, 30)) = 8
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

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

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            fixed4 _PrimaryColor;
            fixed4 _SecondaryColor;
            int _NumXWaves;
            int _NumYWaves;

            fixed4 frag (v2f i) : SV_Target
            {
                //create wave pattern with two colors
                half value = i.uv.x + 0.1*sin(i.uv.y*6.28*_NumYWaves);
                half sinedValue = 0.5+sin(value*_NumXWaves*6.28)/1.8;
                half4 result = _PrimaryColor * sinedValue + _SecondaryColor * (1 - sinedValue);
                //set correct alpha
                fixed4 textureValue = tex2D(_MainTex, i.uv);
                result.a = textureValue.a;
                return result;
            }
            ENDCG
        }
    }
}
