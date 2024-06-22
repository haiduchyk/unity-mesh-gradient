namespace Pandora.MeshGradient
{
    using System.Collections.Generic;
    using System.Reflection;

    public static class ListSizeExtensions
    {
        public static void SetSizeWithReflection<T>(this List<T> list, int size)
        {
            typeof(List<T>).GetField("_size", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(list, size);
        }
    }
}