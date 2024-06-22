namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using Random = UnityEngine.Random;

    [RequireComponent(typeof(Graphic))]
    public class MeshGradientStaticEffect : BaseMeshEffect
    {
        [Header("Don't use this script, it's unoptimized and for debugging")]
        private const int rowsInControlPoints = 3;

        private const int colsInControlPoints = 4;
        private static Matrix4x4 M_h;
        private static Matrix4x4 M_hT;
        public MeshControlPoint[] controlPoints;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            Prepare();

            var vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);

            var rectTransform = GetComponent<RectTransform>();

            NormalizeAndTransformVertices(vertices, rectTransform.rect);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }

        [ContextMenu("Setup")]
        public void Setup()
        {
            controlPoints = new MeshControlPoint[colsInControlPoints * rowsInControlPoints];
            InitializeControlPoints(colsInControlPoints, rowsInControlPoints);
        }

        void Prepare()
        {
            InitializeMatrices();
        }

        void InitializeControlPoints(int width, int height)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var meshControlPoint = new MeshControlPoint
                    {
                        location = new Vector2(
                            Mathf.Lerp(-1, 1f, x / (float) (width - 1)),
                            Mathf.Lerp(-1, 1f, y / (float) (height - 1))
                        ),
                        vTangent = new Vector2(2f / (width - 1), 0),
                        uTangent = new Vector2(0, 2f / (height - 1)),
                        color = Random.ColorHSV()
                    };

                    controlPoints[x + y * colsInControlPoints] = meshControlPoint;
                }
            }
        }

        private void InitializeMatrices()
        {
            M_h = new Matrix4x4();
            M_h.SetRow(0, new Vector4(2, -2, 1, 1));
            M_h.SetRow(1, new Vector4(-3, 3, -2, -1));
            M_h.SetRow(2, new Vector4(0, 0, 1, 0));
            M_h.SetRow(3, new Vector4(1, 0, 0, 0));

            M_hT = M_h.transpose;
        }

        private void NormalizeAndTransformVertices(List<UIVertex> vertices, Rect rect)
        {
            var newVertices = new List<UIVertex>(vertices);

            for (var i = 0; i < vertices.Count; i++)
            {
                var vertex = vertices[i];

                var position = vertex.position;
                var u = (position.x - rect.xMin) / rect.width * (colsInControlPoints - 1);
                var v = (position.y - rect.yMin) / rect.height * (rowsInControlPoints - 1);

                var x = Mathf.FloorToInt(u);
                var y = Mathf.FloorToInt(v);

                u -= x;
                v -= y;

                var controlPointIndexX = x;
                var controlPointIndexY = y;
                if (controlPointIndexX == colsInControlPoints - 1)
                {
                    u = 1;
                    controlPointIndexX = colsInControlPoints - 2;
                }

                if (controlPointIndexY == rowsInControlPoints - 1)
                {
                    v = 1;
                    controlPointIndexY = rowsInControlPoints - 2;
                }

                var p00 = controlPoints[controlPointIndexX + controlPointIndexY * colsInControlPoints];
                var p10 = controlPoints[controlPointIndexX + (controlPointIndexY + 1) * colsInControlPoints];
                var p01 = controlPoints[(controlPointIndexX + 1) + controlPointIndexY * colsInControlPoints];
                var p11 = controlPoints[(controlPointIndexX + 1) + (controlPointIndexY + 1) * colsInControlPoints];

                var X = MeshCoefficients(p00, p01, p10, p11, 0);
                var Y = MeshCoefficients(p00, p01, p10, p11, 1);

                var transformedPoint = (SurfacePoint(u, v, X, Y) + Vector2.one) / 2f;

                vertex.position = new Vector3(
                    transformedPoint.x * rect.width + rect.xMin,
                    transformedPoint.y * rect.height + rect.yMin,
                    transformedPoint.y * 100
                );

                var R = ColorCoefficients(p00, p01, p10, p11, 'r');
                var G = ColorCoefficients(p00, p01, p10, p11, 'g');
                var B = ColorCoefficients(p00, p01, p10, p11, 'b');
                var A = ColorCoefficients(p00, p01, p10, p11, 'a');

                var interpolatedColor = ColorPoint(u, v, R, G, B, A);
                vertex.color = new Color(interpolatedColor.x, interpolatedColor.y, interpolatedColor.z,
                    interpolatedColor.w);

                newVertices[i] = vertex;
            }

            vertices.Clear();
            vertices.AddRange(newVertices);
        }

        private Matrix4x4 MeshCoefficients(MeshControlPoint p00, MeshControlPoint p01, MeshControlPoint p10,
            MeshControlPoint p11, int axis)
        {
            float l(MeshControlPoint p) => p.location[axis];
            float u(MeshControlPoint p) => p.uTangent[axis];
            float v(MeshControlPoint p) => p.vTangent[axis];

            var result = new Matrix4x4();
            result.SetRow(0, new Vector4(l(p00), l(p01), v(p00), v(p01)));
            result.SetRow(1, new Vector4(l(p10), l(p11), v(p10), v(p11)));
            result.SetRow(2, new Vector4(u(p00), u(p01), 0, 0));
            result.SetRow(3, new Vector4(u(p10), u(p11), 0, 0));

            return result;
        }

        private Matrix4x4 ColorCoefficients(MeshControlPoint p00, MeshControlPoint p01, MeshControlPoint p10,
            MeshControlPoint p11,
            char colorChannel)
        {
            float l(MeshControlPoint p)
            {
                switch (colorChannel)
                {
                    case 'r': return p.color.r;
                    case 'g': return p.color.g;
                    case 'b': return p.color.b;
                    case 'a': return p.color.a;
                    default: return 0f;
                }
            }

            var result = new Matrix4x4();
            result.SetRow(0, new Vector4(l(p00), l(p01), 0, 0));
            result.SetRow(1, new Vector4(l(p10), l(p11), 0, 0));
            result.SetRow(2, new Vector4(0, 0, 0, 0));
            result.SetRow(3, new Vector4(0, 0, 0, 0));

            return result;
        }

        public static Vector4 ColorPoint(float u, float v, Matrix4x4 R, Matrix4x4 G, Matrix4x4 B, Matrix4x4 A)
        {
            var U = new Vector4(u * u * u, u * u, u, 1);
            var V = new Vector4(v * v * v, v * v, v, 1);

            var r = Vector4.Dot(V, M_h * R * M_hT * U);
            var g = Vector4.Dot(V, M_h * G * M_hT * U);
            var b = Vector4.Dot(V, M_h * B * M_hT * U);
            var a = Vector4.Dot(V, M_h * A * M_hT * U);

            return new Vector4(r, g, b, a);
        }

        public static Vector2 SurfacePoint(float u, float v, Matrix4x4 X, Matrix4x4 Y)
        {
            var U = new Vector4(u * u * u, u * u, u, 1);
            var V = new Vector4(v * v * v, v * v, v, 1);

            var xResult = M_h * X * M_hT * U;
            var x = Vector4.Dot(V, xResult);

            var yResult = M_h * Y * M_hT * U;
            var y = Vector4.Dot(V, yResult);

            return new Vector2(x, y);
        }
    }
}