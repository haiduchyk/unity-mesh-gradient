namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Graphic))]
    public class SplitTrianglesImage : BaseMeshEffect
    {
        [Header("Don't use, it's unoptimized")]
        [SerializeField, Range(0, 10)]
        private int divisions = 4;

        [SerializeField, Range(1, 5f)]
        private float divisionAreaParameter = 3;

        [SerializeField]
        private bool needAreaCheck;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            var vertices = new List<UIVertex>();
            vh.GetUIVertexStream(vertices);

            // SetInitialVertexColors(vertices);
            SplitTriangles(vertices);

            vh.Clear();
            vh.AddUIVertexTriangleStream(vertices);
        }

        // private void SetInitialVertexColors(List<UIVertex> vertices)
        // {
        //     for (var i = 0; i < vertices.Count; i++)
        //     {
        //         var vertex = vertices[i];
        //         vertex.color = GetInterpolatedColor(vertex.uv0.x, vertex.uv0.y);
        //         vertices[i] = vertex;
        //     }
        // }

        // private Color GetInterpolatedColor(float x, float y)
        // {
        //     Color topLeft = Color.red;
        //     Color topRight = Color.yellow;
        //     Color bottomLeft = Color.blue;
        //     Color bottomRight = Color.green;
        //
        //     Color topColor = Color.Lerp(topLeft, topRight, x);
        //     Color bottomColor = Color.Lerp(bottomLeft, bottomRight, x);
        //
        //     return Color.Lerp(bottomColor, topColor, y);
        // }

        private void SplitTriangles(List<UIVertex> vertices)
        {
            var newVertices = new List<UIVertex>();
            for (var j = 0; j < divisions; j++)
            {
                var maxArea = 0f;
                if (needAreaCheck)
                {
                    maxArea = CalculateMaxArea(vertices);
                }

                var vertexCount = vertices.Count;
                for (var i = 0; i < vertexCount; i += 3)
                {
                    var v0 = vertices[i];
                    var v1 = vertices[i + 1];
                    var v2 = vertices[i + 2];


                    var triangleArea = 0f;
                    if (needAreaCheck)
                    {
                        triangleArea = CalculateTriangleArea(v0.position, v1.position, v2.position);
                    }

                    if (!needAreaCheck || triangleArea * divisionAreaParameter >= maxArea)
                    {
                        var midVertex01 = InterpolateVertex(v0, v1, 0.5f);
                        var midVertex12 = InterpolateVertex(v1, v2, 0.5f);
                        var midVertex20 = InterpolateVertex(v2, v0, 0.5f);

                        AddTriangle(newVertices, v0, midVertex01, midVertex20);
                        AddTriangle(newVertices, midVertex01, v1, midVertex12);
                        AddTriangle(newVertices, midVertex01, midVertex12, midVertex20);
                        AddTriangle(newVertices, midVertex20, midVertex12, v2);
                    }
                    else
                    {
                        newVertices.Add(v0);
                        newVertices.Add(v1);
                        newVertices.Add(v2);
                    }
                }

                vertices.Clear();
                vertices.AddRange(newVertices);
                newVertices.Clear();
            }
        }

        private void AddTriangle(List<UIVertex> vertices, UIVertex v0, UIVertex v1, UIVertex v2)
        {
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
        }

        private float CalculateTriangleArea(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            return Mathf.Abs(Vector3.Cross(v1 - v0, v2 - v0).magnitude) / 2f;
        }

        private float CalculateMaxArea(List<UIVertex> vertices)
        {
            var maxArea = float.MinValue;

            var vertexCount = vertices.Count;

            for (var i = 0; i < vertexCount; i += 3)
            {
                var triangleArea =
                    CalculateTriangleArea(vertices[i].position, vertices[i + 1].position, vertices[i + 2].position);

                if (triangleArea > maxArea)
                {
                    maxArea = triangleArea;
                }
            }

            return maxArea;
        }


        private UIVertex InterpolateVertex(UIVertex v0, UIVertex v1, float t)
        {
            var vertex = new UIVertex
            {
                position = Vector3.Lerp(v0.position, v1.position, t),
                uv0 = Vector4.Lerp(v0.uv0, v1.uv0, t),
                color = Color.Lerp(v0.color, v1.color, t)
            };

            return vertex;
        }
    }
}