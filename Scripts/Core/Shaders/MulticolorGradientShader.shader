Shader "Custom/MulticolorGradientShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Lighter ("Lighter", Range(0,0.3)) = 0
        _OrderZOffset ("Order ZOffset", Range(-10, 10)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite On
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // uncomment to debug shader
            // #pragma target 5.0
            // RWStructuredBuffer<float4x4> buffer : register(u1);

            #include "UnityCG.cginc"

            sampler2D _MainTex;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : COLOR;
                // uncomment to debug shader
                // uint vertexIndex : SV_VertexID;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            int _ColsInControlPoints;
            int _RowsInControlPoints;
            int _PointCount;

            float4x4 _ControlPoints[36];
            float4 _Rect;
            float4 _ImageOrigin;
            float _Lighter;
            float _OrderZOffset;

            // uncomment to debug shader
            // int _VertexIndex;
            // int _CurrentVertexIndex;

            static const float4x4 M_h = float4x4(
                float4(2, -2, 1, 1),
                float4(-3, 3, -2, -1),
                float4(0, 0, 1, 0),
                float4(1, 0, 0, 0)
            );

            static const float4x4 M_hT = float4x4(
                float4(2, -3, 0, 1),
                float4(-2, 3, 0, 0),
                float4(1, -2, 1, 0),
                float4(1, -1, 0, 0)
            );


            float4 ColorPoint(float4 U, float4 V, float4x4 R, float4x4 G, float4x4 B, float4x4 A)
            {
                float r = dot(V, mul(mul(mul(M_h, R), M_hT), U));
                float g = dot(V, mul(mul(mul(M_h, G), M_hT), U));
                float b = dot(V, mul(mul(mul(M_h, B), M_hT), U));
                float a = dot(V, mul(mul(mul(M_h, A), M_hT), U));

                return float4(r, g, b, a);
            }


            float2 SurfacePoint(float4 U, float4 V, float4x4 X, float4x4 Y)
            {
                float4 xResult = mul(mul(mul(M_h, X), M_hT), U);
                float x = dot(V, xResult);

                float4 yResult = mul(mul(mul(M_h, Y), M_hT), U);
                float y = dot(V, yResult);

                return float2(x, y);
            }

            float4 GetTransformedPoint(float oldX, float oldY, float4 U, float4 V, float4x4 p00, float4x4 p10,
                                       float4x4 p01, float4x4 p11)
            {
                float4x4 X = float4x4(
                    float4(p00[0].x, p01[0].x, p00[2].x, p01[2].x), // l(p00), l(p01), v(p00), v(p01)
                    float4(p10[0].x, p11[0].x, p10[2].x, p11[2].x), // l(p10), l(p11), v(p10), v(p11)
                    float4(p00[1].x, p01[1].x, 0, 0), // u(p00), u(p01), 0, 0
                    float4(p10[1].x, p11[1].x, 0, 0) // u(p10), u(p11), 0, 0
                );
                float4x4 Y = float4x4(
                    float4(p00[0].y, p01[0].y, p00[2].y, p01[2].y), // l(p00), l(p01), v(p00), v(p01)
                    float4(p10[0].y, p11[0].y, p10[2].y, p11[2].y), // l(p10), l(p11), v(p10), v(p11)
                    float4(p00[1].y, p01[1].y, 0, 0), // u(p00), u(p01), 0, 0
                    float4(p10[1].y, p11[1].y, 0, 0) // u(p10), u(p11), 0, 0
                );
                float2 transformedPoint = (SurfacePoint(U, V, X, Y) + float2(1, 1)) / 2;
                float z = saturate(length(transformedPoint - float2(oldX, oldY)));

                return float4(
                    transformedPoint.x * _Rect.z + _Rect.x + _ImageOrigin.x,
                    transformedPoint.y * _Rect.w + _Rect.y + _ImageOrigin.y,
                    z + _OrderZOffset,
                    1.0
                );
            }

            float4 CalculateColor(float4 U, float4 V, float4x4 p00, float4x4 p10, float4x4 p01, float4x4 p11)
            {
                float4 colorP00 = p00[3];
                float4 colorP10 = p10[3];
                float4 colorP01 = p01[3];
                float4 colorP11 = p11[3];

                float4x4 R = float4x4(
                    float4(colorP00.x, colorP01.x, 0, 0), // r(colorP00), r(colorP01), 0, 0
                    float4(colorP10.x, colorP11.x, 0, 0), // r(colorP10), r(colorP11), 0, 0
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0)
                );;
                float4x4 G = float4x4(
                    float4(colorP00.y, colorP01.y, 0, 0), // g(colorP00), g(colorP01), 0, 0
                    float4(colorP10.y, colorP11.y, 0, 0), // g(colorP10), g(colorP11), 0, 0
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0)
                );
                float4x4 B = float4x4(
                    float4(colorP00.z, colorP01.z, 0, 0), // b(colorP00), b(colorP01), 0, 0
                    float4(colorP10.z, colorP11.z, 0, 0), // b(colorP10), b(colorP11), 0, 0
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0)
                );;
                float4x4 A = float4x4(
                    float4(colorP00.w, colorP01.w, 0, 0), // a(colorP00), a(colorP01), 0, 0
                    float4(colorP10.w, colorP11.w, 0, 0), // a(colorP10), a(colorP11), 0, 0
                    float4(0, 0, 0, 0),
                    float4(0, 0, 0, 0));

                return ColorPoint(U, V, R, G, B, A);
            }


            v2f vert(appdata_t a)
            {
                v2f o;
                o.uv = a.texcoord;

                // uncomment to debug shader
                // _CurrentVertexIndex = a.vertexIndex;

                float oldX = (a.vertex.x - _ImageOrigin.x - _Rect.x) / _Rect.z;
                float oldY = (a.vertex.y - _ImageOrigin.y - _Rect.y) / _Rect.w;
                float u = oldX * (_ColsInControlPoints - 1);
                float v = oldY * (_RowsInControlPoints - 1);

                int x = (int)floor(u);
                int y = (int)floor(v);

                u -= x;
                v -= y;

                int controlPointIndexX = x;
                int controlPointIndexY = y;

                if (controlPointIndexX == _ColsInControlPoints - 1)
                {
                    u = 1;
                    controlPointIndexX = _ColsInControlPoints - 2;
                }

                if (controlPointIndexY == _RowsInControlPoints - 1)
                {
                    v = 1;
                    controlPointIndexY = _RowsInControlPoints - 2;
                }

                int p00Index = controlPointIndexX + controlPointIndexY * _ColsInControlPoints;
                int p10Index = controlPointIndexX + (controlPointIndexY + 1) * _ColsInControlPoints;
                int p01Index = (controlPointIndexX + 1) + controlPointIndexY * _ColsInControlPoints;
                int p11Index = (controlPointIndexX + 1) + (controlPointIndexY + 1) * _ColsInControlPoints;

                float4x4 p00 = _ControlPoints[p00Index];
                float4x4 p10 = _ControlPoints[p10Index];
                float4x4 p01 = _ControlPoints[p01Index];
                float4x4 p11 = _ControlPoints[p11Index];

                float4 U = float4(u * u * u, u * u, u, 1);
                float4 V = float4(v * v * v, v * v, v, 1);

                a.vertex = GetTransformedPoint(oldX, oldY, U, V, p00, p10, p01, p11);
                o.pos = UnityObjectToClipPos(a.vertex);

                o.color = CalculateColor(U, V, p00, p10, p01, p11);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 texColor = tex2D(_MainTex, i.uv);
                return texColor * i.color * (1 + _Lighter);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}