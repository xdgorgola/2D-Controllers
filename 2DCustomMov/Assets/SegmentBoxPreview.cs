using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PRay;

public class SegmentBoxPreview : MonoBehaviour
{
    public Vector2 rayOrigin = Vector2.zero;
    public Vector2 rayDir = Vector2.one;

    public Vector2 boxCenter = Vector2.one * 3;
    public Vector2 boxSize = Vector2.one;

    private Bounds boxA = new Bounds();

    private void Update()
    {
        boxA.center = boxCenter;
        boxA.size = boxSize;
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(boxA.min, Vector2.right * 100000);
        Gizmos.DrawRay(boxA.min, -Vector2.right * 100000);
        Gizmos.DrawRay(boxA.min, Vector2.up * 100000);
        Gizmos.DrawRay(boxA.min, -Vector2.up * 100000);

        Gizmos.DrawRay(boxA.max, Vector2.right * 100000);
        Gizmos.DrawRay(boxA.max, -Vector2.right * 100000);
        Gizmos.DrawRay(boxA.max, Vector2.up * 100000);
        Gizmos.DrawRay(boxA.max, -Vector2.up * 100000);

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(boxA.center, boxA.size);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(rayOrigin, rayDir);

        Gizmos.color = Color.black;
        Gizmos.DrawCube(rayOrigin, Vector3.one * 0.1f);

        ///rayDir.Normalize();
        //rayDir *= 100;
        float dx = 1f / rayDir.x;
        float dy = 1f / rayDir.y;

        float txmin = (boxA.min.x - rayOrigin.x) * dx;
        float txmax = (boxA.max.x - rayOrigin.x) * dx;
        float tymin = (boxA.min.y - rayOrigin.y) * dy;
        float tymax = (boxA.max.y - rayOrigin.y) * dy;

        float temp = txmin;
        if (txmin > txmax)
        {
            txmin = txmax;
            txmax = temp;
        }

        temp = tymin;
        if (tymin > tymax)
        {
            tymin = tymax;
            tymax = temp;
        }

        Debug.Log("Near plane x = " + txmin);
        Debug.Log("Far plane x = " + txmax);
        Debug.Log("Near plane y = " + tymin);
        Debug.Log("Far plane y = " + tymax);

        Vector2 nearX = rayDir * txmin;
        Vector2 nearY = rayDir * tymin;
        Vector2 farX = rayDir * txmax;
        Vector2 farY = rayDir * tymax;

        Gizmos.color = Color.red;

        Gizmos.DrawCube((Vector2)rayOrigin + nearX, Vector3.one * 0.1f);
        Gizmos.DrawCube((Vector2)rayOrigin + farX, Vector3.one * 0.1f);

        Gizmos.color = Color.green;

        Gizmos.DrawCube((Vector2)rayOrigin + nearY, Vector3.one * 0.1f);
        Gizmos.DrawCube((Vector2)rayOrigin + farY, Vector3.one * 0.1f);

        if (txmin >= tymax || tymin >= txmax) return;

        float nearTime = (txmin > tymin ? txmin : tymin);
        float farTime = (txmax < tymax ? txmax : tymax);

        Debug.Log("Nearest plane: " + nearTime);
        Debug.Log("Furthest plane: " + farTime);

        if (nearTime >= 1 || farTime <= 0)
        {
            Debug.Log("Su direccion intersecta pero no lo alcanza o lo tiene detras");
            return;
        }

        Vector2 point1 = Vector2.one * float.PositiveInfinity;
        Vector2 point2 = Vector2.one * float.PositiveInfinity;

        if (nearTime > 0)
        {
            Debug.Log("Intersecta con el near plane!");
            point1 = rayOrigin + rayDir * nearTime;
        }
        if (farTime < 1)
        {
            Debug.Log("Intersecta con el far plane!");
            point2 = rayOrigin + rayDir * farTime;
        }

        Gizmos.color = Color.yellow;
        if (!float.IsInfinity(point1.x)) Gizmos.DrawCube(point1, Vector3.one * 0.1f);
        if (!float.IsInfinity(point2.x)) Gizmos.DrawCube(point2, Vector3.one * 0.1f);

        Debug.Log("Los preview: " + point1);
        Debug.Log("Los preview: " + point2);
        LineBoxResult xd = new LineBoxResult();
        if (LinesIntersection.LineBoxIntersection(rayOrigin, rayDir, rayDir.magnitude, boxA, ref xd))
        {
            if (xd.ContactNumber() == 1)
            {
                Debug.Log("Unico resultado: " + xd.points[0]);
                if (xd.points.Contains(point1) || xd.points.Contains(point2)) Debug.Log("Bien");
                else Debug.Log("MAL PUTA");
            }
            else
            {
                Debug.Log("Resultados: " + xd.points[0]);
                Debug.Log("Resultados: " + xd.points[1]);
                if (xd.points.Contains(point1) && xd.points.Contains(point2)) Debug.Log("Bien");
                else Debug.Log("MAL PUTA");
            }
        }
    }
}
