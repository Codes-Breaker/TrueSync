Shader "Hidden/XFur Studio 2/Designer/Filler"
{
    Properties
    {
        _MainTex("Input Image", 2D) = "white" {}
        _UVLayout("_UVLayout", 2D) = "white" {}
    }
        SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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


            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            sampler2D _MainTex;
            sampler2D _UVLayout;
            float4 _MainTex_TexelSize;


            float4 Sample(float2 uv, float lod)
            {
                return tex2Dlod(_MainTex, float4(uv, 0, lod));
            }

            float SampleUV(float2 uv, float lod)
            {
                return tex2Dlod(_UVLayout, float4(uv, 0, lod)).r;
            }

            float4 SimpleDilate(float2 uv)
            {
                float2 offsets[8] = { float2(-1,0), float2(1,0), float2(0,1), float2(0,-1), float2(-1,1), float2(1,1), float2(1,-1), float2(-1,-1) };
                float minDist = 10000000;
                float4 samp = Sample(uv, 0);
                float4 currentMinSample = samp;

                if (samp.x == 0 && samp.y == 0 && samp.z == 0) {
                    int i = 0;
                    while (i < 32)
                    {
                        i++;
                        int j = 0;
                        while (j < 8)
                        {
                            float2 offsetUV = uv + offsets[j] * _MainTex_TexelSize.x * i;
                            float4 offsetSample = Sample(offsetUV, 0);
                            float offsetLayout = SampleUV(offsetUV, 0);

                            if (offsetLayout != 0)
                            {
                                float dist = length(uv - offsetUV);
                                if (dist <= minDist)
                                {
                                    minDist = dist;
                                    float2 extrapolatedUV = offsetUV + offsets[j] * _MainTex_TexelSize.x * i * 0.25;
                                    float4 direction = Sample(extrapolatedUV, 0);
                                    float dirLayout = SampleUV(extrapolatedUV, 0);

                                    if (dirLayout != 0)
                                    {
                                        float4 delta = offsetSample - direction;
                                        currentMinSample = offsetSample + delta * 4;
                                    }
                                    else
                                    {
                                        currentMinSample = offsetSample;
                                    }
                                }
                            }
                            j++;
                        }
                    }
                }

                return currentMinSample;
            }


            float4 frag(v2f i) : SV_Target
            {
                float4 dilate = SimpleDilate(i.uv);
                return float4(dilate);
            }

            ENDCG
        }
    }
}
