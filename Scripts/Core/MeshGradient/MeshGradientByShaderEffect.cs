namespace Pandora.MeshGradient
{
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteAlways]
    public class MeshGradientByShaderEffect : MonoBehaviour
    {
        [SerializeField, Range(2, 6)]
        public int rowsInControlPoints = 3;

        [SerializeField, Range(2, 6)]
        public int colsInControlPoints = 4;

        [SerializeField]
        public MeshControlPoint[] controlPoints;

        [SerializeField, HideInInspector]
        private Matrix4x4[] controlPointsForShader;

        [SerializeField]
        private Material material;

        [SerializeField]
        private RectTransform rectTransform;

        private Graphic graphic;

        private static readonly int ControlPointsID = Shader.PropertyToID("_ControlPoints");
        private static readonly int ColsInControlPointsID = Shader.PropertyToID("_ColsInControlPoints");
        private static readonly int RowsInControlPointsID = Shader.PropertyToID("_RowsInControlPoints");
        private static readonly int PointCountID = Shader.PropertyToID("_PointCount");
        private static readonly int RectID = Shader.PropertyToID("_Rect");
        private static readonly int ImageOriginID = Shader.PropertyToID("_ImageOrigin");

        public bool RequireUpdatingShaderProperties { get; set; }

        public void OnValidate()
        {
            graphic ??= GetComponent<Graphic>();
            rectTransform ??= GetComponent<RectTransform>();
            material = graphic.material;

            RequireUpdatingShaderProperties = true;
        }

        [ContextMenu("Setup Control Points")]
        public void SetupControlPoints()
        {
            this.RecordObject("SetupControlPoints");

            controlPoints = new MeshControlPoint[colsInControlPoints * rowsInControlPoints];
            InitializeControlPoints(colsInControlPoints, rowsInControlPoints);

            controlPointsForShader = new Matrix4x4[colsInControlPoints * rowsInControlPoints];

            RequireUpdatingShaderProperties = true;

            this.SetDirty();
        }

        [ContextMenu("Reset Positions")]
        public void ResetPositions()
        {
            this.RecordObject("ResetPositions");

            for (var y = 0; y < rowsInControlPoints; y++)
            {
                for (var x = 0; x < colsInControlPoints; x++)
                {
                    var meshControlPoint = controlPoints[x + y * colsInControlPoints];

                    meshControlPoint.location = new Vector2(
                        Mathf.Lerp(-1, 1f, x / (float) (colsInControlPoints - 1)),
                        Mathf.Lerp(-1, 1f, y / (float) (rowsInControlPoints - 1))
                    );
                }
            }

            RequireUpdatingShaderProperties = true;
            this.SetDirty();
        }

        [ContextMenu("Reset Tangents")]
        public void ResetTangents()
        {
            this.RecordObject("ResetTangents");

            for (var y = 0; y < rowsInControlPoints; y++)
            {
                for (var x = 0; x < colsInControlPoints; x++)
                {
                    var meshControlPoint = controlPoints[x + y * colsInControlPoints];

                    meshControlPoint.vTangent = new Vector2(2f / (colsInControlPoints - 1), 0);
                    meshControlPoint.uTangent = new Vector2(0, 2f / (rowsInControlPoints - 1));
                }
            }

            RequireUpdatingShaderProperties = true;
            this.SetDirty();
        }

        private void InitializeControlPoints(int width, int height)
        {
            var colors = RandomColorGenerator.GenerateColorPalette(colsInControlPoints);
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
                        color = colors[x]
                    };

                    controlPoints[x + y * colsInControlPoints] = meshControlPoint;
                }
            }
        }

        [ContextMenu("Reset Colors")]
        public void ResetColors()
        {
            this.RecordObject("ResetColors");

            var colors = RandomColorGenerator.GenerateColorPalette(colsInControlPoints);
            for (var y = 0; y < rowsInControlPoints; y++)
            {
                for (var x = 0; x < colsInControlPoints; x++)
                {
                    var meshControlPoint = controlPoints[x + y * colsInControlPoints];
                    meshControlPoint.color = colors[x];
                    controlPoints[x + y * colsInControlPoints] = meshControlPoint;
                }
            }

            RequireUpdatingShaderProperties = true;
            this.SetDirty();
        }

        private void Update()
        {
            // OnValidate bug, material.SetMatrixArray does not work from OnValidate??
            // TODO uncomment to not recalculate each frame
            // if (RequireUpdatingShaderProperties)
            {
                RequireUpdatingShaderProperties = false;
                UpdateShaderProperties();
            }
        }

        private void UpdateShaderProperties()
        {
            UpdateControlPointsForShader();
            SetScaledRect();
            SetImageOrigin();
            SetControlPoints();
        }

        private void UpdateControlPointsForShader()
        {
            for (var i = 0; i < controlPoints.Length; i++)
            {
                var cp = controlPoints[i];
                controlPointsForShader[i] = new Matrix4x4(
                    new Vector4(cp.location.x, cp.uTangent.x, cp.vTangent.x, cp.color.r),
                    new Vector4(cp.location.y, cp.uTangent.y, cp.vTangent.y, cp.color.g),
                    new Vector4(0, 0, 0, cp.color.b),
                    new Vector4(0, 0, 0, cp.color.a)
                );
            }
        }

        private void SetScaledRect()
        {
            var scaleFactor = CanvasScaleFactorConstants.ScaleFactor;
            var lossyScale = transform.lossyScale / scaleFactor;
            var rect = rectTransform.rect;
            var scaledRect = new Vector4(rect.xMin * lossyScale.x, rect.yMin * lossyScale.y, rect.width * lossyScale.x,
                rect.height * lossyScale.y);
            material.SetVector(RectID, scaledRect);
        }

        private void SetImageOrigin()
        {
            var scaleFactor = CanvasScaleFactorConstants.ScaleFactor;
            var position = rectTransform.position;
            var imageOrigin = new Vector4(position.x - Screen.width / 2f, position.y - Screen.height / 2f) /
                              scaleFactor;
            material.SetVector(ImageOriginID, imageOrigin);
        }

        private void SetControlPoints()
        {
            material.SetInt(ColsInControlPointsID, colsInControlPoints);
            material.SetInt(RowsInControlPointsID, rowsInControlPoints);
            material.SetInt(PointCountID, colsInControlPoints * rowsInControlPoints);
            material.SetMatrixArray(ControlPointsID, controlPointsForShader);
        }
    }
}