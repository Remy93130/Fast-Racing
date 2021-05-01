using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class Track : MonoBehaviour
{
    #region attributs

    /// <summary>
    /// Le smesh qui represente la route
    /// </summary>
    public Mesh2D mesh2D;

    /// <summary>
    /// Savoir si on connecte le dernier segment au premier
    /// </summary>
    public bool loop;

    /// <summary>
    /// Nombre de triangle par segment
    /// Obviously il faut qu il soit au minimum a 2
    /// (Apres on peut mettre moins j'ai bloque par contre si c est negatif surprise (:)
    /// </summary>
    [Range(2, 10)]
    public int trianglesPerSegment = 2;

    #endregion

    #region methodes

    /// <summary>
    /// Lorsqu on lance le jeu on creer le circuit
    /// </summary>
    void Awake() => UpdateSegments();

#if UNITY_EDITOR

    /// <summary>
    /// On met a jour le mesh lors d une nouvelle frame
    /// UNIQUEMENT EN MODE EDITEUR
    /// Comme ca on peux modifier la route en mode jeu et elle se reset quand on stop le mode jeu
    /// Du coup laisser ca en release c est pas ouf niveau perf
    /// </summary>
    void Update() => UpdateSegments();

#endif

    /// <summary>
    /// Boucle sur tout les segments de route pour mettre a jour leur mesh
    /// </summary>
    public void UpdateSegments()
    {
        TrackSegment[] allSegments = GetComponentsInChildren<TrackSegment>();
        TrackSegment[] segmentsWithMesh = allSegments.Where(segment => segment.HasValidNextPoint).ToArray();
        TrackSegment[] segmentsWithoutMesh = allSegments.Where(segment => segment.HasValidNextPoint == false).ToArray();

        // On calcul la longeur total du circuit pour pouvoir avoir des coordonnees normalisees
        // Sa nous donne un tableau de float d intervalle [0, 1] tel que 0 = debut et 1 = fin
        float[] lengths = segmentsWithMesh.Select(x => x.GetBezierRepresentation(Space.Self).GetArcLength()).ToArray();
        float totalRoadLength = lengths.Sum();

        float startDist = 0f;
        for (int i = 0; i < segmentsWithMesh.Length; ++i)
        {
            float endDist = startDist + lengths[i];
            Vector2 uv = new Vector2(
                startDist / totalRoadLength, // Pourcentage debut circuit
                endDist / totalRoadLength    // Pourcentage fin circuit
            );
            segmentsWithMesh[i].UpdateMesh(uv);
            startDist = endDist;
        }

        // Segment sans smesh, on les supprime
        for (int i = 0; i < segmentsWithoutMesh.Length; ++i)
        {
            segmentsWithoutMesh[i].UpdateMesh(Vector2.zero);
        }
    }

    public Vector3 getFirstSegment()
    {
        TrackSegment[] allSegments = GetComponentsInChildren<TrackSegment>();
        return allSegments[0].transform.position;

    }

    #endregion
}
