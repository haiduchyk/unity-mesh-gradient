#if UNITY_EDITOR
namespace Pandora.MeshGradient
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [CustomEditor(typeof(MeshGradientStaticEffect))]
    public class MeshGradientStaticEffectEditor : Editor
    {
        private MeshGradientStaticEffect meshGradientStaticEffect;
        private RectTransform rectTransform;
        private int selectedControlPointIndex = -1;

        private void OnEnable()
        {
            meshGradientStaticEffect = (MeshGradientStaticEffect) target;
            rectTransform = meshGradientStaticEffect.GetComponent<RectTransform>();
        }

        private void OnSceneGUI()
        {
            if (meshGradientStaticEffect.controlPoints == null || meshGradientStaticEffect.controlPoints.Length == 0)
                return;

            for (var i = 0; i < meshGradientStaticEffect.controlPoints.Length; i++)
            {
                var meshControlPoint = meshGradientStaticEffect.controlPoints[i];

                // Calculate the local and world position of the control point
                var localPos = new Vector3(meshControlPoint.location.x * rectTransform.rect.width,
                    meshControlPoint.location.y * rectTransform.rect.height, 0);
                var worldPos = rectTransform.TransformPoint(localPos / 2);

                // Draw handle for location
                Handles.color = meshControlPoint.color;
                var newWorldPos = Handles.PositionHandle(worldPos, Quaternion.identity);

                // Convert back to local position and update control point location
                if (newWorldPos != worldPos)
                {
                    Undo.RecordObject(meshGradientStaticEffect, "Move Control Point");
                    var newLocalPos = rectTransform.InverseTransformPoint(newWorldPos) * 2;
                    meshControlPoint.location = new Vector2(newLocalPos.x / rectTransform.rect.width,
                        newLocalPos.y / rectTransform.rect.height);
                    EditorUtility.SetDirty(meshGradientStaticEffect);
                    selectedControlPointIndex = i;
                    meshGradientStaticEffect.GetComponent<Graphic>().SetVerticesDirty();
                }

                // Display label with control point index
                Handles.Label(worldPos, $"P{i}");
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Setup Control Points"))
            {
                meshGradientStaticEffect.Setup();
                meshGradientStaticEffect.GetComponent<Graphic>().SetVerticesDirty();
            }

            if (selectedControlPointIndex >= 0 &&
                selectedControlPointIndex < meshGradientStaticEffect.controlPoints.Length)
            {
                var selectedMeshControlPoint =
                    meshGradientStaticEffect.controlPoints[selectedControlPointIndex];
                selectedMeshControlPoint.color =
                    EditorGUILayout.ColorField("Selected Control Point Color", selectedMeshControlPoint.color);
                EditorUtility.SetDirty(meshGradientStaticEffect);
                meshGradientStaticEffect.GetComponent<Graphic>().SetVerticesDirty();
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(meshGradientStaticEffect);
                meshGradientStaticEffect.GetComponent<Graphic>().SetVerticesDirty();
            }
        }
    }
}

#endif