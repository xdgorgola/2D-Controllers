using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PRay
{

    public class PRay
    {
        public Vector2 origin;
        public Vector2 dir;

        public PRay(Vector2 origin, Vector2 dir)
        {
            this.origin = origin;
            if (dir.sqrMagnitude == 0)
            {
                throw new System.ArgumentException("Ray cannot have direction (0,0)");
            }
            this.dir = dir;
        }
    }

    public struct RayBoxResult
    {
        public Vector2 point1;
        public Vector2 point2;

        public RayBoxResult(Vector2 point1, Vector2 point2)
        {
            this.point1 = point1;
            this.point2 = point2;
        }
    }

    public struct LineBoxResult
    {
        public List<Vector2> points;

        public int ContactNumber()
        {
            if (points != null) return points.Count;
            return -1;
        }
    }


    public class LinesIntersection
    {
        public static bool RayBoxIntersection(PRay ray, Bounds box, ref RayBoxResult result)
        {
            return RayBoxIntersection(ray.origin, ray.dir, box, ref result);
        }

        public static bool RayBoxIntersection(Vector2 origin, Vector2 dir, Bounds box, ref RayBoxResult result)
        {
            //dir = dir.normalized;

            // Paralelo horizontalmente y mas arriba o abajo que el AABB
            if (Vector2.Dot(dir, Vector2.up) == 0 && (origin.y <= box.min.y || origin.y >= box.max.y)) return false;
            // Paralelo verticalmente y mas derecha o izquierda que el AABB
            if (Vector2.Dot(dir, Vector2.right) == 0 && (origin.x <= box.min.x || origin.x >= box.max.x)) return false;


            // Recta vertical en x = box.min.x NearPlaneX
            // Recta vertical en x = box.max.x FarPlaneX
            float txmin;
            float txmax;
            // Recta horizontal en y = box.min.y NearPlaneY
            // Recta horizontal en y = box.max.y FarPlaneY
            float tymin;
            float tymax;

            float dx = 1f / dir.x;
            float dy = 1f / dir.y;

            // Pendiente de los infinitos aca? Aunque igual da jejeje
            txmin = (box.min.x - origin.x) * dx;
            txmax = (box.max.x - origin.x) * dx;
            tymin = (box.min.y - origin.y) * dy;
            tymax = (box.max.y - origin.y) * dy;

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

            if (txmin >= tymax || tymin >= txmax) return false;


            result.point1 = origin + dir * (txmin > tymin ? txmin : tymin);
            result.point2 = origin + dir * (txmax < tymax ? txmax : tymax);
            return true;
        }

        public static bool LineBoxIntersection(Vector2 origin, Vector2 to, Bounds box, ref LineBoxResult result)
        {
            Vector2 dir = (to - origin);
            return LineBoxIntersection(origin, dir, dir.magnitude, box, ref result);
        }

        public static bool LineBoxIntersection(Vector2 origin, Vector2 dir, float length, Bounds box, ref LineBoxResult result)
        {
            // Totalmente similar a interseccion rayo caja
            // Solo que tenemos que asegurarnos que las t no se pasen del rango [0,1]
            // ya que esto significaria que irias mas alla del segmento.
            dir = dir.normalized * length;

            if (Vector2.Dot(dir, Vector2.up) == 0 && (origin.y <= box.min.y || origin.y >= box.max.y)) return false;
            else if (Vector2.Dot(dir, Vector2.right) == 0 && (origin.x <= box.min.x || origin.x >= box.max.x)) return false;

            // Calculamos las intersecciones con los Near-Far Planes
            float dx = 1f / dir.x;
            float dy = 1f / dir.y;

            float txmin = (box.min.x - origin.x) * dx;
            float txmax = (box.max.x - origin.x) * dx;
            float tymin = (box.min.y - origin.y) * dy;
            float tymax = (box.max.y - origin.y) * dy;

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
            if (txmin >= tymax || tymin >= txmax) return false;

            float nearTime = (txmin > tymin ? txmin : tymin);
            float farTime = (txmax < tymax ? txmax : tymax);

            // La direccion si puede hacer interseccion pero de izquierda a derecha:
            // 1) Apuntamos al cuadrado pero no llegamos
            // 2) No apuntamos al cuadrado, sino en direccion contraria
            if (nearTime >= 1 || farTime <= 0) return false;

            result.points = new List<Vector2>();

            Vector2 point1 = Vector2.positiveInfinity;
            Vector2 point2 = Vector2.positiveInfinity;

            if (nearTime > 0) point1 = origin + dir * nearTime;
            if (farTime < 1) point2 = origin + dir * farTime;

            if (!float.IsInfinity(point1.x)) result.points.Add(point1);
            if (!float.IsInfinity(point2.x)) result.points.Add(point2);

            return true;
        }
    }
}
