using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[CustomEditor(typeof(TrackSegment))]
public class TrackSegmentInspector : Editor
{
    void OnSceneGUI()
    {

        // Creeation des ID
        int arrowIDForward = GUIUtility.GetControlID("Arrow Forward".GetHashCode(), FocusType.Passive);
        int arrowIDBack = GUIUtility.GetControlID("Arrow Back".GetHashCode(), FocusType.Passive);

        // Recupere les variables utiles
        TrackSegment track = target as TrackSegment;
        Vector3 origin = track.GetControlPoint(0, Space.World);
        Vector3 tangentForward = track.GetControlPoint(1, Space.World);
        Vector3 tangentBack = origin * 2 - tangentForward;
        Vector3 tangentDir = track.transform.forward;

        // Creation du plane
        Vector3 camUp = SceneView.lastActiveSceneView.camera.transform.up;
        Vector3 pNormal = Vector3.Cross(tangentDir, camUp).normalized;
        Plane draggingPlane = new Plane(pNormal, origin);
        float newDistance = 0;

        // Affichage des handles pour la tangente
        bool changedForward = DrawTangentHandle(arrowIDForward, tangentForward, origin, tangentDir, draggingPlane, ref newDistance);
        bool changedBack = DrawTangentHandle(arrowIDBack, tangentBack, origin, -tangentDir, draggingPlane, ref newDistance);

        // Si un des handle est modifie on met a jour la distance
        if (changedForward || changedBack)
        {
            Undo.RecordObject(target, "adjust bezier tangent");
            track.tangentLength = newDistance;
            track.RoadChain?.UpdateSegments();
        }
    }

    private bool DrawTangentHandle(int id, Vector3 handlePos, Vector3 origin, Vector3 direction, Plane draggingPlane, ref float newDistance)
    {
        bool isChanged = false;
        float size = HandleUtility.GetHandleSize(handlePos);
        float handleRadius = size * 0.25f;
        float cursorDistancePx = HandleUtility.DistanceToCircle(handlePos, handleRadius * 0.5f);

        Event e = Event.current;
        bool leftClick = e.button == 0;
        bool isDragging = GUIUtility.hotControl == id && leftClick;
        bool isHover = HandleUtility.nearestControl == id;

        switch (e.type)
        {
            case EventType.Layout:
                HandleUtility.AddControl(id, cursorDistancePx);
                break;
            case EventType.MouseDown:
                if (isHover && leftClick)
                {
                    GUIUtility.hotControl = id;
                    GUIUtility.keyboardControl = id;
                    e.Use();
                }
                break;
            case EventType.MouseDrag:
                if (isDragging)
                {
                    Ray r = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                    if (draggingPlane.Raycast(r, out float dist))
                    {
                        Vector3 intersectionPt = r.GetPoint(dist);
                        // On bloque a .5 car il faut pas que se soit 0 ou negatif
                        float projectedDistance = Vector3.Dot(intersectionPt - origin, direction).AtLeast(.5f);
                        newDistance = projectedDistance;
                        isChanged = true;
                    }
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                if (isDragging)
                {
                    GUIUtility.hotControl = 0;
                    e.Use();
                }
                break;
            case EventType.Repaint:
                Color color = GetHandleColor(isHover, isDragging);
                using (new TemporaryHandleColor(color))
                {
                    Handles.DrawAAPolyLine(origin, handlePos);
                    Quaternion rot = Quaternion.LookRotation(direction);
                    Handles.SphereHandleCap(id, handlePos, rot, handleRadius, EventType.Repaint);
                }
                break;
        }

        return isChanged;
    }

    private Color GetHandleColor(bool hovering, bool dragging)
    {
        if (dragging)
            return Color.yellow;
        else if (hovering)
            return Color.green;
        return Handles.zAxisColor;
    }

    class TemporaryHandleColor : IDisposable
    {
        static readonly Stack<Color> colorStack = new Stack<Color>();
        public TemporaryHandleColor(Color color)
        {
            colorStack.Push(Handles.color);
            Handles.color = color;
        }
        public void Dispose() => Handles.color = colorStack.Pop();
    }
}
