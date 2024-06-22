namespace Pandora.MeshGradient
{
    using UnityEditor;
    using UnityEngine;

    public static class UnityObjectExtensions
    {
        public static void SetDirty(this Object target)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(target);
#endif
        }

        public static void RecordObject(this Object target, string name)
        {
#if UNITY_EDITOR
            Undo.RecordObject(target, name);
#endif
        }
    }
}