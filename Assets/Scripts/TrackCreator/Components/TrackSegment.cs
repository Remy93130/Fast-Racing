using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TrackSegment : MonoBehaviour
{
    #region attributs

    /// <summary>
    /// Longueur de la tangente
    /// Ne pas mettre trop sinon la texture est cheloue
    /// </summary>
    [Range(.5f, 20)]
    public float tangentLength = 3;

    // Attributs pour la creation du mesh

    readonly List<Vector3> vertices = new List<Vector3>();
    readonly List<int> triangles = new List<int>();
    readonly List<Vector3> normals = new List<Vector3>();
    readonly List<Vector3> uv0 = new List<Vector3>();
    readonly List<Vector3> uv1 = new List<Vector3>();

    private Mesh _mesh;

    private Mesh Mesh
    {
        get
        {
            if (null == GetComponent<MeshFilter>().sharedMesh)
            {
                _mesh = new Mesh();
                GetComponent<MeshFilter>().sharedMesh = _mesh;
            }
            else
            {
                _mesh = GetComponent<MeshFilter>().sharedMesh;
            }
            return _mesh;
        }
    }

    #endregion

    #region methodes

    /// <summary>
    /// Verifie si on a un segment apres celui ci
    /// </summary>
    public bool HasValidNextPoint => null != TryGetNextSegment();

    /// <summary>
    /// Petit getter sympa qui evite de trop repeter cette grosse ligne
    /// </summary>
    public Track RoadChain => transform.parent?.GetComponent<Track>();

    /// <summary>
    /// Verifie si on doit mettre a jour le mesh
    /// </summary>
    /// <param name="uv"></param>
    public void UpdateMesh(Vector2 uv)
    {

        // On genere le mesh seulement s il y a un segment apres celui ci
        if (HasValidNextPoint)
        {
            GenerateMesh(uv);
        }
        else if (Mesh != null)
        {
            DestroyImmediate(Mesh);
        }

    }

    /// <summary>
    /// Smesh goes brrr
    /// </summary>
    /// <param name="uv"></param>
    private void GenerateMesh(Vector2 uv)
    {
        // On vide nos listes
        Mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        uv0.Clear();
        uv1.Clear();

        CubicBezierCurve bezier = GetBezierRepresentation(Space.Self);

        float curveArcLegth = bezier.GetArcLength();
        float tiling = GetTextureAspectRatio();
        int edgeCount = Mathf.Max(2, Mathf.RoundToInt(curveArcLegth * RoadChain.trianglesPerSegment)); // 2 triangles minimum sinon kaboom

        // Generation vertices normales & map texture
        for (int i = 0; i < edgeCount; ++i)
        {
            float t = i / (edgeCount - 1f);
            OrientedPoint op = bezier.GetOrientedPoint(t);

            float uv0V = t * tiling;
            float uv1U = Mathf.Lerp(uv.x, uv.y, t);

            for (int j = 0; j < RoadChain.mesh2D.VertexCount; ++j)
            {
                vertices.Add(op.LocalToWorldPos(RoadChain.mesh2D.vertices[j].point));
                normals.Add(op.LocalToWorldVec(RoadChain.mesh2D.vertices[j].normal));
                uv0.Add(new Vector2(RoadChain.mesh2D.vertices[j].u, uv0V));
                uv1.Add(new Vector2(uv1U, 0));
            }
        }

        // Generation des triangles
        for (int i = 0; i < edgeCount - 1; ++i)
        {
            int ifv = i * RoadChain.mesh2D.VertexCount;
            int ipofv = (i + 1) * RoadChain.mesh2D.VertexCount;

            for (int j = 0; j < RoadChain.mesh2D.LineCount; j += 2)
            {
                int t = RoadChain.mesh2D.triangles[j];
                int tpo = RoadChain.mesh2D.triangles[j + 1];
                triangles.Add(ifv + t);
                triangles.Add(ipofv + t);
                triangles.Add(ipofv + tpo);
                triangles.Add(ifv + t);
                triangles.Add(ipofv + tpo);
                triangles.Add(ifv + tpo);
            }
        }

        Mesh.SetVertices(vertices);
        Mesh.SetTriangles(triangles, 0);
        Mesh.SetNormals(normals);
        Mesh.SetUVs(0, uv0);
        Mesh.SetUVs(1, uv1);
    }

    /// <summary>
    /// Recupere l aspect ratio de la texture
    /// </summary>
    /// <returns></returns>
    private float GetTextureAspectRatio()
    {
        Texture texture = GetComponent<MeshRenderer>().sharedMaterial.Ref()?.mainTexture;
        return (null != texture) ? texture.AspectRatio() : 1f;
    }

    /// <summary>
    /// Recupere un des 4 point de control de la courbe de bezier
    /// </summary>
    /// <param name="i"></param>
    /// <param name="space"></param>
    /// <returns></returns>
    public Vector3 GetControlPoint(int i, Space space)
    {
        if (i < 2)
        {
            if (0 == i)
            {
                return (space == Space.Self) ? Vector3.zero : transform.TransformPoint(Vector3.zero);
            }
            if (1 == i)
            {
                return (space == Space.Self) ? Vector3.forward * tangentLength : transform.TransformPoint(Vector3.forward * tangentLength);
            }
        }
        else
        {
            TrackSegment next = TryGetNextSegment();
            Transform nextTf = next.transform;
            if (2 == i)
            {
                return (space == Space.World) ? nextTf.TransformPoint(Vector3.back * next.tangentLength) : transform.InverseTransformPoint(nextTf.TransformPoint(Vector3.back * next.tangentLength));
            }

            if (3 == i)
            {
                return (space == Space.World) ? nextTf.position : transform.InverseTransformPoint(nextTf.position);
            }
        }
        return default;
    }

    /// <summary>
    /// Renvoi le prochain segment s il y en a un
    /// Permet aussi de relier le circuit si on fait une boucle
    /// </summary>
    /// <returns></returns>
    private TrackSegment TryGetNextSegment()
    {
        if (null == transform.parent.Ref()?.GetComponent<Track>())
        {
            return null;
        }

        int currentIndex = transform.GetSiblingIndex();
        bool isLast = currentIndex == transform.parent.childCount - 1;
        if (isLast && RoadChain.loop)
        {
            return transform.parent.GetChild(0).GetComponent<TrackSegment>();
        }
        else if (!isLast)
        {
            return transform.parent.GetChild(currentIndex + 1).GetComponent<TrackSegment>();
        }

        return null;
    }

    /// <summary>
    /// Recupere la representation de la courbe de bezier de se segment
    /// </summary>
    /// <param name="space"></param>
    /// <returns></returns>
    public CubicBezierCurve GetBezierRepresentation(Space space)
    {
        return new CubicBezierCurve(
            GetUpVector(0, space),
            GetUpVector(3, space),
            GetControlPoint(0, space),
            GetControlPoint(1, space),
            GetControlPoint(2, space),
            GetControlPoint(3, space)
        );
    }

    /// <summary>
    /// Recupere le premier ou dernier control point
    /// </summary>
    /// <param name="i"></param>
    /// <param name="space"></param>
    /// <returns></returns>
    private Vector3 GetUpVector(int i, Space space)
    {
        if (i == 0)
        {
            return space == Space.Self ? Vector3.up : transform.up;
        }
        else if (i == 3)
        {
            return (space == Space.World) ? TryGetNextSegment().transform.up : transform.InverseTransformVector(TryGetNextSegment().transform.up);
        }
        return default;
    }

    #endregion
}
