using UnityEngine;

public static class ExtensionMethods
{
    /// <summary>
    /// Permet verifier qu une reference existe avec l operateur .?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <returns></returns>
    public static T Ref<T>(this T obj) where T : Object => obj ?? null;

    public static float AspectRatio(this Texture texture) => texture.width / texture.height;

    public static float AtLeast(this float v, float minVal) => Mathf.Max(v, minVal);

}
