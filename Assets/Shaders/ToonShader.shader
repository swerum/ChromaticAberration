Shader "Sarah/ToonShader"
{
    Properties
    {
        _Albedo ("Albedo", Color) = (1,1,1,1)
        _MinAlbedo ("Minimum Albedo", Range(0,1)) = 0.4
        _NumSteps ("Number Of Steps", Range(1,10)) = 4
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
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                // float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD0;
            };

            float4 _Albedo;
            float _NumSteps;
            float _MinAlbedo;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = fixed4(1.0, 1.0, 1.0, 1.0);
                float cosineAngle = dot(normalize(i.worldNormal), normalize(_WorldSpaceLightPos0.xyz));
                cosineAngle = floor(cosineAngle * (_NumSteps+1)) / _NumSteps;
                cosineAngle = _MinAlbedo + cosineAngle*(1-_MinAlbedo);

                cosineAngle = max(cosineAngle, 0.0);
                return _Albedo * cosineAngle;
            }
            ENDCG
        }
    }
}
