using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{

    /// <summary>
    /// Nous permet de rentrer a la main la position des points
    /// de la normal et de u
    /// </summary>
    [System.Serializable]
    public class Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float u;
    }

    public int[] triangles;

    public Vertex[] vertices;

    public int VertexCount => vertices.Length;

    public int LineCount => triangles.Length;

}
