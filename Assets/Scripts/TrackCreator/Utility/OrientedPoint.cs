using UnityEngine;

/// <summary>
/// Structure pour gerer l orientation et la position d un objet
/// C est un peu un transform mais en plus light
/// </summary>
public struct OrientedPoint
{
    #region attribut

    /// <summary>
    /// La position
    /// </summary>
    public Vector3 pos;

    /// <summary>
    /// La rotation
    /// </summary>
    public Quaternion rot;

    #endregion

    #region methodes

    public OrientedPoint(Vector3 pos, Quaternion rot)
    {
        this.pos = pos;
        this.rot = rot;
    }

    /// <summary>
    /// Convertion local vers global
    /// </summary>
    /// <param name="localSpacePos"></param>
    /// <returns></returns>
    public Vector3 LocalToWorldPos(Vector3 localSpacePos)
    {
        return pos + rot * localSpacePos;
    }

    /// <summary>
    /// Convertion global vers local
    /// </summary>
    /// <param name="localSpacePos"></param>
    /// <returns></returns>
    public Vector3 LocalToWorldVec(Vector3 localSpacePos)
    {
        return rot * localSpacePos;
    }

    #endregion
}
