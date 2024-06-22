#if UNITY_EDITOR
namespace Pandora.MeshGradient
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(MeshGradientByShaderEffect))]
    public class MeshGradientByShaderEffectEditor : Editor
    {
        private MeshGradientByShaderEffect meshGradientByShaderEffect;
        private RectTransform rectTransform;
        private int selectedControlPointIndex = -1;
        private const float HandleSizeCoef = 1.5f;
        private const float OffsetCoef = 15f;

        private void OnEnable()
        {
            meshGradientByShaderEffect = (MeshGradientByShaderEffect) target;
            rectTransform = meshGradientByShaderEffect.GetComponent<RectTransform>();
        }

        private void OnSceneGUI()
        {
            if (meshGradientByShaderEffect.controlPoints == null ||
                meshGradientByShaderEffect.controlPoints.Length == 0)
                return;

            for (var i = 0; i < meshGradientByShaderEffect.controlPoints.Length; i++)
            {
                var meshControlPoint = meshGradientByShaderEffect.controlPoints[i];

                var localPos = new Vector3(meshControlPoint.location.x * rectTransform.rect.width,
                    meshControlPoint.location.y * rectTransform.rect.height, 0);
                var worldPos = rectTransform.TransformPoint(localPos / 2);

                Handles.color = meshControlPoint.color;

                var handleSize = HandleUtility.GetHandleSize(worldPos);
                var newWorldPos = Handles.FreeMoveHandle(worldPos, handleSize * HandleSizeCoef, Vector3.zero,
                    Handles.ArrowHandleCap);

                if (newWorldPos != worldPos)
                {
                    Undo.RecordObject(meshGradientByShaderEffect, "Move Control Point");
                    var newLocalPos = rectTransform.InverseTransformPoint(newWorldPos) * 2;
                    meshControlPoint.location = new Vector2(newLocalPos.x / rectTransform.rect.width,
                        newLocalPos.y / rectTransform.rect.height);

                    meshGradientByShaderEffect.OnValidate();
                    EditorUtility.SetDirty(meshGradientByShaderEffect);
                    selectedControlPointIndex = i;
                    meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
                }

                DrawTangentHandles(meshControlPoint, worldPos, rectTransform);

                var style = new GUIStyle
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = {textColor = Color.black}
                };
                Handles.Label(worldPos, $"{i}", style);
            }
        }

        private void DrawTangentHandles(MeshControlPoint meshControlPoint, Vector3 worldPos,
            RectTransform rectTransform)
        {
            var blendedBlackColor = BlendColorWithBlack(meshControlPoint.color, 0.7f);
            var blendedBlackColor2 = BlendColorWithBlack(meshControlPoint.color, 0.9f);

            var blendedWhiteColor = BlendColorWithWhite(meshControlPoint.color, 0.3f);
            var blendedWhiteColor2 = BlendColorWithWhite(meshControlPoint.color, 0.5f);

            DrawVTangentHandle(meshControlPoint, worldPos, rectTransform, blendedWhiteColor);
            DrawUTangentHandle(meshControlPoint, worldPos, rectTransform, blendedWhiteColor2);
            DrawVTangentYHandle(meshControlPoint, worldPos, rectTransform, blendedBlackColor);
            DrawUTangentXHandle(meshControlPoint, worldPos, rectTransform, blendedBlackColor2);
        }

        private Color BlendColorWithBlack(Color originalColor, float t)
        {
            return Color.Lerp(originalColor, Color.black, t);
        }

        private Color BlendColorWithWhite(Color originalColor, float t)
        {
            return Color.Lerp(originalColor, Color.white, t);
        }

        private void DrawVTangentHandle(MeshControlPoint meshControlPoint, Vector3 worldPos,
            RectTransform rectTransform,
            Color blendedColor)
        {
            var vTangentWorldPos =
                worldPos + new Vector3(meshControlPoint.vTangent.x * rectTransform.rect.width / OffsetCoef, 0, 0);
            Handles.color = blendedColor;
            var newVTangentWorldPos = Handles.FreeMoveHandle(vTangentWorldPos,
                HandleUtility.GetHandleSize(vTangentWorldPos) / 2 * HandleSizeCoef, Vector3.zero,
                Handles.ArrowHandleCap);
            if (newVTangentWorldPos.x != vTangentWorldPos.x)
            {
                Undo.RecordObject(meshGradientByShaderEffect, "Move vTangent Handle");
                var newVTangentLocalX = (newVTangentWorldPos.x - worldPos.x) * OffsetCoef / rectTransform.rect.width;
                meshControlPoint.vTangent = new Vector2(newVTangentLocalX, meshControlPoint.vTangent.y);

                meshGradientByShaderEffect.OnValidate();
                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }

        private void DrawUTangentHandle(MeshControlPoint meshControlPoint, Vector3 worldPos,
            RectTransform rectTransform,
            Color blendedColor)
        {
            var uTangentWorldPos = worldPos;
            uTangentWorldPos.y += (meshControlPoint.uTangent.y * (meshGradientByShaderEffect.rowsInControlPoints /
                                                                  (float) meshGradientByShaderEffect
                                                                      .colsInControlPoints)) *
                                  rectTransform.rect.width /
                                  OffsetCoef;
            Handles.color = blendedColor;
            var newUTangentWorldPos = Handles.FreeMoveHandle(uTangentWorldPos,
                HandleUtility.GetHandleSize(uTangentWorldPos) / 2 * HandleSizeCoef, Vector3.zero,
                Handles.ArrowHandleCap);
            if (newUTangentWorldPos.y != uTangentWorldPos.y)
            {
                Undo.RecordObject(meshGradientByShaderEffect, "Move uTangent Handle");
                var newUTangentLocalY = (newUTangentWorldPos.y - worldPos.y) * OffsetCoef / rectTransform.rect.width;
                meshControlPoint.uTangent = new Vector2(meshControlPoint.uTangent.x, newUTangentLocalY);

                meshGradientByShaderEffect.OnValidate();
                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }

        private void DrawVTangentYHandle(MeshControlPoint meshControlPoint, Vector3 worldPos,
            RectTransform rectTransform,
            Color blendedColor)
        {
            var vTangentYWorldPos = worldPos;
            vTangentYWorldPos.y += meshControlPoint.vTangent.y * rectTransform.rect.width / OffsetCoef;

            var offsetY = -1f * rectTransform.rect.width / OffsetCoef;
            vTangentYWorldPos.y += offsetY;

            Handles.color = blendedColor;
            var newVTangentYWorldPos = Handles.FreeMoveHandle(vTangentYWorldPos,
                HandleUtility.GetHandleSize(vTangentYWorldPos) / 2 * HandleSizeCoef, Vector3.zero,
                Handles.ArrowHandleCap);
            if (newVTangentYWorldPos.y != vTangentYWorldPos.y)
            {
                Undo.RecordObject(meshGradientByShaderEffect, "Move vTangent Y Handle");
                var newVTangentLocalY = (newVTangentYWorldPos.y - (worldPos.y + offsetY)) * OffsetCoef /
                                        rectTransform.rect.width;
                meshControlPoint.vTangent = new Vector2(meshControlPoint.vTangent.x, newVTangentLocalY);

                meshGradientByShaderEffect.OnValidate();
                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }

        private void DrawUTangentXHandle(MeshControlPoint meshControlPoint, Vector3 worldPos,
            RectTransform rectTransform,
            Color blendedColor)
        {
            var uTangentXWorldPos = worldPos;
            uTangentXWorldPos.x += meshControlPoint.uTangent.x * rectTransform.rect.width / OffsetCoef;

            var offsetX = -1f * rectTransform.rect.width / OffsetCoef;
            uTangentXWorldPos.x += offsetX;

            Handles.color = blendedColor;
            var newUTangentXWorldPos = Handles.FreeMoveHandle(uTangentXWorldPos,
                HandleUtility.GetHandleSize(uTangentXWorldPos) / 2 * HandleSizeCoef, Vector3.zero,
                Handles.ArrowHandleCap);
            if (newUTangentXWorldPos.x != uTangentXWorldPos.x)
            {
                Undo.RecordObject(meshGradientByShaderEffect, "Move uTangent X Handle");
                var newUTangentLocalX = (newUTangentXWorldPos.x - (worldPos.x + offsetX)) * OffsetCoef /
                                        rectTransform.rect.width;
                meshControlPoint.uTangent = new Vector2(newUTangentLocalX, meshControlPoint.uTangent.y);

                meshGradientByShaderEffect.OnValidate();
                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (selectedControlPointIndex >= 0 &&
                selectedControlPointIndex < meshGradientByShaderEffect.controlPoints.Length)
            {
                var selectedMeshControlPoint =
                    meshGradientByShaderEffect.controlPoints[selectedControlPointIndex];
                selectedMeshControlPoint.color =
                    EditorGUILayout.ColorField("Selected Control Point Color", selectedMeshControlPoint.color);
                selectedMeshControlPoint.vTangent =
                    EditorGUILayout.Vector2Field("vTangent", selectedMeshControlPoint.vTangent);
                selectedMeshControlPoint.uTangent =
                    EditorGUILayout.Vector2Field("uTangent", selectedMeshControlPoint.uTangent);

                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(meshGradientByShaderEffect);
                meshGradientByShaderEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }
    }
}
#endif