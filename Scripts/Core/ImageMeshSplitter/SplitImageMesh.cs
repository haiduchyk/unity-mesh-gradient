namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Pool;
    using UnityEngine.UI;

    [RequireComponent(typeof(Graphic))]
    public class SplitImageMesh : BaseMeshEffect
    {
        [SerializeField, Range(0, 100)]
        private int vertexAmountPerSize = 30;

        private List<UIVertex> newVerticesList;
        private List<int> newIndices;

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive() || vertexAmountPerSize == 0)
                return;

            var oldVertices = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(oldVertices);

            var vertexCount = (vertexAmountPerSize + 1) * (vertexAmountPerSize + 1);
            if (newVerticesList == null || newVerticesList.Count != vertexCount)
            {
                newVerticesList = new List<UIVertex>(vertexCount);
                newVerticesList.SetSizeWithReflection(vertexCount);
            }

            var indexCount = vertexAmountPerSize * vertexAmountPerSize * 6;
            if (newIndices == null || newIndices.Count != indexCount)
            {
                newIndices = new List<int>(indexCount);
                newIndices.SetSizeWithReflection(indexCount);
            }

            BuildPlaneMesh(oldVertices[4], oldVertices[0], oldVertices[1], oldVertices[3]);

            ListPool<UIVertex>.Release(oldVertices);

            vh.Clear();
            vh.AddUIVertexStream(newVerticesList, newIndices);
        }

        private void BuildPlaneMesh(UIVertex v0, UIVertex v1, UIVertex v2, UIVertex v3)
        {
            var vertexIndex = 0;
            for (var i = 0; i <= vertexAmountPerSize; i++)
            {
                var tY = (float) i / vertexAmountPerSize;
                var vLeft = InterpolateVertex(ref v0, ref v3, tY);
                var vRight = InterpolateVertex(ref v1, ref v2, tY);

                for (var j = 0; j <= vertexAmountPerSize; j++)
                {
                    var tX = (float) j / vertexAmountPerSize;
                    newVerticesList[vertexIndex++] = InterpolateVertex(ref vLeft, ref vRight, tX);
                }
            }

            var index = 0;
            var rowSize = vertexAmountPerSize + 1;
            for (var i = 0; i < vertexAmountPerSize; i++)
            {
                for (var j = 0; j < vertexAmountPerSize; j++)
                {
                    var startIndex = i * rowSize + j;
                    newIndices[index++] = startIndex;
                    newIndices[index++] = startIndex + 1;
                    newIndices[index++] = startIndex + rowSize;

                    newIndices[index++] = startIndex + 1;
                    newIndices[index++] = startIndex + rowSize + 1;
                    newIndices[index++] = startIndex + rowSize;
                }
            }
        }

        private UIVertex InterpolateVertex(ref UIVertex v0, ref UIVertex v1, float t)
        {
            return new UIVertex
            {
                position = Vector3.Lerp(v0.position, v1.position, t),
                uv0 = Vector4.Lerp(v0.uv0, v1.uv0, t),
                // normal = Vector3.Lerp(v0.position, v1.position, t),
                // color = Color32.Lerp(v0.color, v1.color, t)
            };
        }
    }
}