using UnityEngine;

[System.Serializable]
public class CubicBezierCurve
{
    #region attributs

    /// <summary>
    /// Les points de control de notre courbe de bezie
    /// </summary>
    public Vector3[] points = new Vector3[4];

    public Vector3 start;

    public Vector3 end;

    #endregion

    #region surchage d operateur

    /// <summary>
    /// Surchage d operateur pour acceder comme dans un tableau
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public Vector3 this[int i] => points[i];

    #endregion

    #region methodes

    public CubicBezierCurve(Vector3 start, Vector3 end, params Vector3[] points)
    {
        this.points = points;
        this.start = start;
        this.end = end;
    }

    public OrientedPoint GetOrientedPoint(float t) => new OrientedPoint(GetPoint(t), GetOrientation(t));

    private Quaternion GetOrientation(float t) => Quaternion.LookRotation(GetTangent(t), Vector3.Slerp(start, end, t).normalized);

    /// <summary>
    /// La magie des mathematiques
    /// Permet de split la courbe en petits morceaux lineraire
    /// <see cref="https://en.wikipedia.org/wiki/Abel%E2%80%93Ruffini_theorem"/>
    /// (La version francaise donne plus de detail)
    /// </summary>
    /// <returns></returns>
    public float GetArcLength()
    {
        int accuracy = 16;
        Vector3[] points = new Vector3[accuracy];
        for (int i = 0; i < accuracy; i++)
        {
            float t = i / (accuracy - 1);
            points[i] = GetPoint(t);
        }
        float dist = 0;
        for (int i = 0; i < accuracy - 1; i++)
        {
            Vector3 a = points[i];
            Vector3 b = points[i + 1];
            dist += Vector3.Distance(a, b);
        }
        return dist;
    }

    /// <summary>
    /// Recupere un point avec le polynome de Bernstein
    /// <see cref="https://docs.google.com/presentation/d/10XjxscVrm5LprOmG-VB2DltVyQ_QygD26N6XC2iap2A/edit#slide=id.gc41ce114c_0_330"/>
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPoint(float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        return points[0] * (omt2 * omt) + points[1] * (3 * omt2 * t) + points[2] * (3 * omt * t2) + points[3] * (t2 * t);
    }

    /// <summary>
    /// Recupere la tangente
    /// On utilise la tangente car c est plus opti niveau calcul
    /// <see cref="https://docs.google.com/presentation/d/10XjxscVrm5LprOmG-VB2DltVyQ_QygD26N6XC2iap2A/edit#slide=id.gc41ce114c_1_1"/>
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetTangent(float t)
    {
        float omt = 1f - t;
        float omt2 = omt * omt;
        float t2 = t * t;
        Vector3 tangent = points[0] * (-omt2) + points[1] * (3 * omt2 - 2 * omt) + points[2] * (-3 * t2 + 2 * t) + points[3] * (t2);
        return tangent.normalized;
    }

    #endregion
}
