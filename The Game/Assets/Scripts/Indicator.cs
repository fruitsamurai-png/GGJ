using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    GameObject target;
    [SerializeField]
    RectTransform canvas;
    [SerializeField]
    RectTransform indictor;
    [SerializeField]
    GameObject indictorWorld;
    bool IntersectSegmentPlane(Vector3 a, Vector3 b, Plane p, out Vector3 q)
    {
        // Compute the t value for the directed line ab intersecting the plane
        Vector3 ab = b - a;
        q = Vector3.zero;
        float t = (p.distance - Vector3.Dot(p.normal, a)) / Vector3.Dot(p.normal, ab);
        // If t in [0..1] compute and return intersection point
        if (t >= 0.0f && t <= 1.0f)
        {
            q = a + t * ab;
            return true;
        }
        return false;
    }
    void WorldToRectWorldPos(Vector3 worldPos, RectTransform rect, Camera camera)
    {

        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(camera, worldPos);
        Vector2 localPos;
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPos, camera, out localPos);
        RectTransformUtility.ScreenPointToWorldPointInRectangle(rect, screenPos, camera, out worldPos);
        //rect.anchoredPosition = localPos;
        rect.position = worldPos;
    }
    private Vector2 WorldToCanvasPosition(RectTransform canvas, Camera camera, Vector3 position)
    {
        //Vector position (percentage from 0 to 1) considering camera size.
        //For example (0,0) is lower left, middle is (0.5,0.5)
        Vector2 temp = camera.WorldToViewportPoint(position);

       
        temp.x *= canvas.sizeDelta.x;
        temp.y *= canvas.sizeDelta.y;

        temp.x -= canvas.sizeDelta.x * canvas.pivot.x;
        temp.y -= canvas.sizeDelta.y * canvas.pivot.y;

        return temp;
    }
    private void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main); 



        foreach(Plane p in planes)
        {
            Vector3 pt;
            if (IntersectSegmentPlane(transform.position,target.transform.position ,p,out pt))
            {
                indictorWorld.transform.position = pt;

                //indictor.anchoredPosition = WorldToCanvasPosition(canvas,Camera.main,pt);
                //indictor.transform.position = new Vector3(pt.x, transform.position.y,pt.z);
                //break;
            }

        }
    }
}
