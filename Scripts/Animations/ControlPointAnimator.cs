namespace Pandora.MeshGradient
{
    using DG.Tweening;
    using UnityEngine;

    // made by ai, requires dotween
    [RequireComponent(typeof(MeshGradientByShaderEffect))]
    public class ControlPointAnimator : MonoBehaviour
    {
        private MeshControlPoint[] controlPoints;
        private int rows;
        private int cols;

        [SerializeField]
        private float positionAnimationDuration = 1f;

        [SerializeField]
        private float colorAnimationDuration = 1f;

        [SerializeField]
        private float moveRange = 0.5f;

        [SerializeField]
        private float hueRotationRange = 0.1f;

        [SerializeField]
        private float pauseDuration = 0.5f;

        [SerializeField]
        private float durationDeviation = 1.15f;

        [SerializeField]
        private Ease tweenEase = Ease.InOutQuad;

        private void Start()
        {
            var meshGradientByShaderEffect = GetComponent<MeshGradientByShaderEffect>();

            controlPoints = meshGradientByShaderEffect.controlPoints;
            rows = meshGradientByShaderEffect.rowsInControlPoints;
            cols = meshGradientByShaderEffect.colsInControlPoints;

            AnimateControlPoints();
        }

        private void AnimateControlPoints()
        {
            for (var i = 0; i < controlPoints.Length; i++)
            {
                StartAnimatingControlPoint(controlPoints[i], i);
            }
        }

        private void StartAnimatingControlPoint(MeshControlPoint cp, int index)
        {
            var initialPosition = cp.location;
            var initialColor = cp.color;

            if (!IsCornerPoint(index))
            {
                AnimatePosition(cp, initialPosition, index);
            }

            AnimateColor(cp, initialColor, index);
        }

        private void AnimatePosition(MeshControlPoint cp, Vector2 initialPosition, int index)
        {
            var duration = positionAnimationDuration *
                           Random.Range(1f / durationDeviation, durationDeviation);
            var targetPosition = GetTargetPosition(initialPosition, index);

            DOTween.To(() => cp.location, x => cp.location = x, targetPosition, duration)
                .SetEase(tweenEase)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(pauseDuration, () => { AnimatePosition(cp, initialPosition, index); });
                });
        }

        private void AnimateColor(MeshControlPoint cp, Color initialColor, int index)
        {
            var duration = colorAnimationDuration * Random.Range(1f / durationDeviation, durationDeviation);
            var targetColor = GetTargetColor(initialColor);

            DOTween.To(() => cp.color, x => cp.color = x, targetColor, duration)
                .SetEase(tweenEase)
                .OnComplete(() =>
                {
                    DOVirtual.DelayedCall(pauseDuration, () => { AnimateColor(cp, initialColor, index); });
                });
        }

        private Vector2 GetTargetPosition(Vector2 initialPosition, int index)
        {
            Vector2 targetPosition;
            if (IsSidePoint(index))
            {
                targetPosition = GetSideTargetPosition(initialPosition, index);
            }
            else
            {
                targetPosition = initialPosition + Random.insideUnitCircle.normalized * moveRange;
                targetPosition = ClampPosition(targetPosition);
            }

            return targetPosition;
        }

        private Color GetTargetColor(Color initialColor)
        {
            Color.RGBToHSV(initialColor, out var h, out var s, out var v);
            h = (h + Random.Range(-hueRotationRange, hueRotationRange)) % 1f;
            if (h < 0) h += 1f;
            return Color.HSVToRGB(h, s, v);
        }

        private bool IsCornerPoint(int index)
        {
            return index == 0 || index == cols - 1 || index == controlPoints.Length - cols ||
                   index == controlPoints.Length - 1;
        }

        private bool IsSidePoint(int index)
        {
            return index < cols || index % cols == 0 || (index + 1) % cols == 0 || index >= controlPoints.Length - cols;
        }

        private Vector2 GetSideTargetPosition(Vector2 initialPosition, int index)
        {
            if (index < cols) // Top row
            {
                return ClampPosition(new Vector2(initialPosition.x + Random.Range(-moveRange, moveRange),
                    initialPosition.y));
            }

            if (index % cols == 0) // Left column
            {
                return ClampPosition(new Vector2(initialPosition.x,
                    initialPosition.y + Random.Range(-moveRange, moveRange)));
            }

            if ((index + 1) % cols == 0) // Right column
            {
                return ClampPosition(new Vector2(initialPosition.x,
                    initialPosition.y + Random.Range(-moveRange, moveRange)));
            }

            if (index >= controlPoints.Length - cols) // Bottom row
            {
                return ClampPosition(new Vector2(initialPosition.x + Random.Range(-moveRange, moveRange),
                    initialPosition.y));
            }

            return initialPosition;
        }

        private Vector2 ClampPosition(Vector2 position)
        {
            return new Vector2(Mathf.Clamp(position.x, -1f, 1f), Mathf.Clamp(position.y, -1f, 1f));
        }
    }
}