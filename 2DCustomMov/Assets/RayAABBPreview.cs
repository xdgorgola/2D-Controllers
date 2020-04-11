using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PRay;

public class RayAABBPreview : MonoBehaviour
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
        Gizmos.DrawRay(rayOrigin, rayDir * 100);
        Gizmos.DrawRay(rayOrigin, rayDir * -100);

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

        Vector2 point1 = rayOrigin + rayDir * (txmin > tymin ? txmin : tymin);
        Vector2 point2 = rayOrigin + rayDir * (txmax < tymax ? txmax : tymax);

        RayBoxResult result = new RayBoxResult();
        if (LinesIntersection.RayBoxIntersection(rayOrigin, rayDir, boxA, ref result))
        {
            if (point1 == result.point1 && point2 == result.point2) Debug.Log("Bien!");
            else Debug.Log("MAL");
        }

        if (txmin >= tymax || tymin >= txmax) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawCube(point1, Vector3.one * 0.1f);
        Gizmos.DrawCube(point2, Vector3.one * 0.1f);

    }
}
