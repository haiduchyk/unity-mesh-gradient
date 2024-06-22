namespace Pandora.MeshGradient
{
    using UnityEngine;
    using UnityEngine.UI;

    [ExecuteAlways]
    public class ShaderDebugging : MonoBehaviour
    {
        public int amount = 6;
        public Material material;
        private ComputeBuffer buffer;
        private Matrix4x4[] element;
        private string label;
        private Graphic render;
        private static readonly int Buffer = Shader.PropertyToID("buffer");

        private void Awake()
        {
            Load();
        }

        private void Load()
        {
            buffer = new ComputeBuffer(1, 16 * 4 * amount, ComputeBufferType.Default);
            element = new Matrix4x4[amount];
            label = string.Empty;
            render = GetComponent<Graphic>();
            material = render.material;
        }

        private void Update()
        {
            if (buffer == null)
            {
                Load();
            }

            Graphics.ClearRandomWriteTargets();
            material.SetPass(0);
            material.SetBuffer(Buffer, buffer);
            Graphics.SetRandomWriteTarget(1, buffer, false);
            buffer.GetData(element);
            label = (element != null && render.isActiveAndEnabled)
                ? GetResult()
                : string.Empty;

            string GetResult()
            {
                var res = "";
                for (var i = 0; i < amount; i++)
                {
                    res += element[i].ToString("F3") + "\n";
                }

                return res;
            }
        }

        private void OnGUI()
        {
            var style = new GUIStyle
            {
                fontSize = 32,
                normal = new GUIStyleState {textColor = Color.red}
            };
            GUI.Label(new Rect(50, 50, 800, 200), label, style);
        }

        private void OnDestroy()
        {
            buffer?.Dispose();
        }
    }
}