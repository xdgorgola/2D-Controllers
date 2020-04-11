using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoundBoxColl;
using PRay;

public class BoxCollVelVisualizer : MonoBehaviour
{
    [Range(0f, 1f)]
    public float t = 0f;
    public bool preview = false;

    public Vector2 AVelocity = Vector2.zero;

    public Vector2 ACenter = Vector2.zero;
    public Vector2 ASize = Vector2.one;

    public Vector2 BCenter = -Vector2.one;
    public Vector2 BSize = Vector2.right * 4 + Vector2.one;

    private Bounds boxA;
    private Bounds boxB;
    private Bounds boxAMoved;

    private void Start()
    {
        boxA = new Bounds(ACenter, ASize);
        boxAMoved = new Bounds(ACenter + AVelocity, ASize);
        boxB = new Bounds(BCenter, BSize);
    }

    private void Update()
    {
        boxA.center = ACenter;
        boxA.size = ASize;

        boxB.center = BCenter;
        boxB.size = BSize;

        boxAMoved.center = ACenter + AVelocity;
        boxAMoved.size = boxA.size;

        RayBoxResult xd = new PRay.RayBoxResult();
        LinesIntersection.RayBoxIntersection(ACenter, AVelocity, new Bounds(Vector2.one * 1.5f, Vector3.one), ref xd);
    }

    public bool IsCorrectSide(Bounds boxB, Bounds solBox, Vector2 collFace)
    {
        if (collFace.x == 0)
        {
            if (collFace.y == 1)
            {
                return solBox.min.y > boxB.max.y;
            }
            return solBox.max.y < boxB.min.y;
        }
        else
        {
            if (collFace.x == 1)
            {
                return solBox.min.x > boxB.max.x;
            }
            return solBox.max.x < boxB.max.x;
            
        }
    }
    private void OnDrawGizmos()
    {
        if (!enabled) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxA.center, boxA.size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(boxB.center, boxB.size);

        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(boxAMoved.center, boxAMoved.size);

        Gizmos.color = Color.white;
        Gizmos.DrawRay(boxA.center, AVelocity);

        if (preview)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Vector2.Lerp(boxA.center, boxAMoved.center, t), boxA.size);
        }

        OverlapInfo overlap = BoundingBox.OverlapBoxes(boxAMoved, boxB);

        float dirY = 0;
        float dirX = 0;

        float quadX = 0;
        float quadY = 0;
        // Determinamos en que cuadrante esta
        if (boxA.center.x < boxB.center.x) quadX = -1;
        else quadX = 1;

        if (boxA.center.y < boxB.center.y) quadY = -1;
        else quadY = 1;

        if (quadY == 1)
        {
            if (boxA.min.y < boxB.max.y) dirX = quadX;
            else
            {
                dirY = 1;
                if (quadX == 1 && boxA.min.x > boxB.max.x)
                {
                    dirX = 1;
                }
                else if (quadX == -1 && boxA.max.x < boxB.min.x)
                {
                    dirX = -1;
                }
            }
        }
        else
        {
            if (boxA.max.y > boxB.min.y) dirX = quadX;
            else
            {
                dirY = -1;
                if (quadX == 1 && boxA.min.x > boxB.max.x)
                {
                    dirX = 1;
                }
                else if (quadX == -1 && boxA.max.x < boxB.min.x)
                {
                    dirX = -1;
                }
            }
        }

        // Este codigo seria un poco mas comodo con los halfwidth (aunque igual tendria que estar pendiente de los cuadrantes)
        Debug.Log("Prev Dir (" + dirX + "," + dirY + ")");
        if (dirX == 1)
        {
            if (dirY == 1)
            {
                Vector2 orig = Vector2.right * boxA.min.x + Vector2.up * boxA.min.y;

                Vector2 vOC = ((Vector2.right * boxAMoved.min.x + Vector2.up * boxAMoved.min.y) - orig).normalized;
                Vector2 vOB = ((Vector2.right * boxB.max.x + Vector2.up * boxB.max.y) - orig).normalized;

                float roC = Mathf.Abs(vOC.y / vOC.x);
                float roB = Mathf.Abs(vOB.y / vOB.x);

                // Revisar
                if (roC > roB) dirY = 0;
                else dirX = 0;

                Debug.Log("roC: " + roC);
                Debug.Log("roB: " + roB);
            }
            else if (dirY == -1)
            {
                Vector2 orig = Vector2.right * boxA.min.x + Vector2.up * boxA.max.y;

                Vector2 vOC = ((Vector2.right * boxAMoved.min.x + Vector2.up * boxAMoved.max.y) - orig).normalized;
                Vector2 vOB = ((Vector2.right * boxB.max.x + Vector2.up * boxB.min.y) - orig).normalized;

                float roC = Mathf.Abs(vOC.y / vOC.x);
                float roB = Mathf.Abs(vOB.y / vOB.x);

                if (roC > roB) dirY = 0;
                else dirX = 0;

                Debug.Log("roC: " + roC);
                Debug.Log("roB: " + roB);
            }
        }
        else if (dirX == -1)
        {
            if (dirY == 1)
            {
                Vector2 orig = Vector2.right * boxA.max.x + Vector2.up * boxA.min.y;

                Vector2 vOC = ((Vector2.right * boxAMoved.max.x + Vector2.up * boxAMoved.min.y) - orig).normalized;
                Vector2 vOB = ((Vector2.right * boxB.min.x + Vector2.up * boxB.max.y) - orig).normalized;

                float roC = Mathf.Abs(vOC.y / vOC.x);
                float roB = Mathf.Abs(vOB.y / vOB.x);

                if (roC < roB) dirX = 0;
                else dirY = 0;

                Debug.Log("roC: " + roC);
                Debug.Log("roB: " + roB);
            }
            else if (dirY == -1)
            {
                Vector2 orig = Vector2.right * boxA.max.x + Vector2.up * boxA.max.y;

                Vector2 vOC = ((Vector2.right * boxAMoved.max.x + Vector2.up * boxAMoved.max.y) - orig).normalized;
                Vector2 vOB = ((Vector2.right * boxB.min.x + Vector2.up * boxB.min.y) - orig).normalized;

                float roC = Mathf.Abs(vOC.y / vOC.x);
                float roB = Mathf.Abs(vOB.y / vOB.x);

                if (roC < roB) dirX = 0;
                else dirY = 0;

                Debug.Log("roC: " + roC);
                Debug.Log("roB: " + roB);
            }
        }

        Debug.Log("Dir (" + dirX + "," + dirY + ")");

        if (overlap.overlaps)
        {
            //Debug.Log("Penetracion X: " + overlap.x);
            //Debug.Log("Penetracion Y: " + overlap.y);

            // Definimos de que sentido "viene la colision"
            // "" porque me estos saltando casos piches

            Vector2 response = Vector2.zero;
            //Debug.Log(dirY * (overlap.y + 0.001f));
            //Debug.Log(dirX * (overlap.x + 0.001f));
            if (overlap.y <= overlap.x) response.y = dirY * (overlap.y + 0.001f);
            else response.x = dirX * (overlap.x + 0.001f);


            //
            //Debug.Log("Resp X: " + response.x);
            //Debug.Log("Resp Y: " + response.y);
            //
            //Debug.Log(response);

            Vector2 finalPos = (Vector2)boxAMoved.center + Vector2.up * dirY * (overlap.y + 0.015f) + Vector2.right * dirX * (overlap.x + 0.015f);
            Bounds solved = new Bounds(finalPos, boxA.size);

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(solved.center, solved.size);

            Gizmos.color = Color.green;
            Gizmos.DrawSphere(solved.center, 0.025f);

            OverlapInfo finalInfo = BoundingBox.OverlapBoxes(boxB, solved);

            if (finalInfo.overlaps || !IsCorrectSide(boxB, solved, Vector2.right * dirX + Vector2.up * dirY))
            {
                Debug.Log("Resolucion conflictiva! Hacer Sweeping Test");
                Debug.Log("Conflicto overlapping: " + finalInfo.overlaps);
                Debug.Log("Conficto lado correcto: " + !IsCorrectSide(boxB, boxAMoved, Vector2.right * dirX + Vector2.up * dirY));
            }
        }
        else
        {
            if (!IsCorrectSide(boxB, boxAMoved, Vector2.right * dirX + Vector2.up * dirY))
            {
                Debug.Log("La caja no detecta la pared! Tunneling");
                return;
            }
            // if (BoundingBox.CheckXOverlap(boxAMoved, boxB) || BoundingBox.CheckYOverlap(boxAMoved, boxB))
            // {
            //     if (!IsCorrectSide(boxB, boxAMoved, Vector2.right * dirX + Vector2.up * dirY))
            //     {
            //         Debug.Log("La caja no detecta la pared! Tunneling");
            //         return;
            //     }
            // }
            // else
            // {
            //     // ver si la velocidad traspasa la pared de alguna forma
            // }
        }
    }
}
