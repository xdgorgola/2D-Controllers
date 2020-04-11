using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BoundBoxColl
{
    // deberia hacer una clase de bounding box que actue como boxA y boxB pero you know
    public class BoundingBox
    {
        public static OverlapInfo OverlapBoxes(Bounds boxA, Bounds boxB)
        {
            OverlapInfo results = new OverlapInfo();
            results.overlaps = false;
            results.x = -1;
            results.y = -1;
            if (CheckYOverlap(boxA, boxB) && CheckXOverlap(boxA, boxB))
            {
                results.overlaps = true;
                results.x = XOverlap(boxA, boxB);
                results.y = YOverlap(boxA, boxB);
            }
            return results;
        } 

        public static bool CheckYOverlap(Bounds boxA, Bounds boxB)
        {
            Debug.Log("A YOverlaps B: " + ((boxA.max.y >= boxB.min.y && boxA.min.y <= boxB.max.y) || (boxB.max.y >= boxA.min.y && boxB.min.y <= boxA.max.y)));
            return !(boxA.min.y > boxB.max.y || boxA.max.y < boxB.min.y);
        }

        public static bool CheckXOverlap(Bounds boxA, Bounds boxB)
        {
            Debug.Log("A XOverlaps B: " + ((boxA.max.y >= boxB.min.y && boxA.min.y <= boxB.max.y) || (boxB.max.y >= boxA.min.y && boxB.min.y <= boxA.max.y)));
            return !(boxA.min.x > boxB.max.x || boxA.max.x < boxB.min.x);
        }

        public static float YOverlap(Bounds boxA, Bounds boxB)
        {
            if (boxA.max.y >= boxB.max.y)
            {
                if (boxB.min.y >= boxA.min.y) return boxB.size.y;
                else return boxB.max.y - boxA.min.y;
            }
            else
            {
                if (boxA.min.y >= boxB.min.y) return boxA.size.y;
                else return boxA.max.y - boxB.min.y;
            }
        }

        public static float XOverlap(Bounds boxA, Bounds boxB)
        {
            if (boxA.max.x >= boxB.max.x)
            {
                if (boxB.min.x >= boxA.min.x) return boxB.size.x;
                else return boxB.max.x - boxA.min.x;
            }
            else
            {
                if (boxA.min.x >= boxB.min.x) return boxA.size.x;
                else return boxA.max.x - boxB.min.x;
            }
        }
    }

    public struct OverlapInfo
    {
        public bool overlaps;
        public float x;
        public float y;
    }
}