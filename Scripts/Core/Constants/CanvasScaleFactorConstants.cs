namespace Pandora.MeshGradient
{
    using UnityEngine;
    using Screen = UnityEngine.Device.Screen;

    public static class CanvasScaleFactorConstants
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            // ScaleFactor = canvas.scaleFactor
            ScaleFactor = Screen.height / DefaultScreenHeight;

            // SizeMultiplier = 1 / (canvas.scaleFactor)
            SizeMultiplier = 1 / ScaleFactor;

            SqrSizeMultiplier = SizeMultiplier * SizeMultiplier;
        }

        public const float DefaultScreenHeight = 2688f;
        public const float DefaultScreenWidth = 1242;

        public static float ScaleFactor = Screen.height / DefaultScreenHeight;

        public static float SizeMultiplier = 1 / (Screen.height / DefaultScreenHeight);

        public static float SqrSizeMultiplier = SizeMultiplier * SizeMultiplier;
    }
}