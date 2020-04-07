using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCollisionSolver : MonoBehaviour
{
    private Collider2D coll;

    public LayerMask collisionMask;

    [SerializeField]
    private float maxSlopeAngle = 60f;


    [SerializeField]
    private float skinWidth = 0.1f;

    [SerializeField]
    private float horizontalRays = 5f;
    [SerializeField]
    private float verticalRays = 5f;

    private float widht;
    private float height;

    private float sepHorizontal;
    private float sepVertical;

    public Platform platform;

    public RaycastOrigins origins = new RaycastOrigins();
    public CollisionInfo collisionInfo = new CollisionInfo();
    
    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }


    private void Start()
    {
        UpdateRaysSep();    
    }


    public void UpdateRaysSep()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2);

        widht = Mathf.Abs(bounds.min.x - bounds.max.x);
        height = Mathf.Abs(bounds.min.y - bounds.max.y);
        sepHorizontal = widht / (horizontalRays - 1);
        sepVertical = height / (verticalRays - 1);
    }
    

    public void MovePlatform(Vector2 platVel)
    {
        UpdateRaycastOrigins();
        //Debug.Log(platVel);
        if (platVel.x != 0) SolveXCollisionPlat(ref platVel);
        if (platVel.y != 0) SolveYCollisionPlat(ref platVel);
        if (platVel.sqrMagnitude != 0)
        {
            //GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + velocity);
            // Quizas necesario
            transform.Translate(platVel);
            //Debug.Log(platVel);
            Physics2D.SyncTransforms();
            //Debug.Log(velocity.x + " final");
        }
    }

    private void SolveXCollisionPlat(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRays; i++)
        {
            Vector2 rayOrigin = (directionX == 1 ? origins.bottomRight : origins.bottomLeft) + Vector2.up * (sepVertical * i + velocity.y);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            //Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                Debug.Log("Hola! X");
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisionInfo.right = directionX == 1;
                collisionInfo.left = directionX == -1;
            }
        }
    }

    private void SolveYCollisionPlat(ref Vector2 velocity)
    {

        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRays; i++)
        {
            Vector2 rayOrigin = (directionY == -1 ? origins.bottomLeft : origins.topLeft) + Vector2.right * (sepHorizontal * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;
                collisionInfo.top = directionY == 1;
            }
        }
    }

    public void Move(Vector2 velocity)
    {
        UpdateRaycastOrigins();

        collisionInfo.ResetInfo();

        if (velocity.y < 0) DescendSlope(ref velocity);
        if (velocity.x != 0) SolveXCollision(ref velocity);
        if (velocity.y != 0) SolveYCollision(ref velocity);

        if (velocity.sqrMagnitude != 0)
        {
            transform.Translate(velocity);
            Physics2D.SyncTransforms();
        }
    }

    private void SolveXCollision(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRays; i++)
        {
            Vector2 rayOrigin = (directionX == 1 ? origins.bottomRight : origins.bottomLeft) + Vector2.up * (sepVertical * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);


            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {
                    float compensation = 0;
                    if (collisionInfo.oldSlopeAngle != slopeAngle)
                    {
                        compensation = hit.distance - skinWidth;
                        velocity.x -= compensation * directionX;
                    }

                    ClimbSlope(ref velocity, slopeAngle);

                    velocity.x += compensation * directionX;
                }

                if (!collisionInfo.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;


                    if (collisionInfo.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisionInfo.right = directionX == 1;
                    collisionInfo.left = directionX == -1;
                }
            }
        }
    }

    private void SolveYCollision(ref Vector2 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        if (directionY == 1 && platform != null)
        {
            collisionInfo.inPlatform = false;
            platform.inPlatform.Remove(this);
            platform = null;
        }

        bool atLeastonce = false;
        for (int i = 0; i < verticalRays; i++)
        {
            Vector2 rayOrigin = (directionY == -1 ? origins.bottomLeft : origins.topLeft) + Vector2.right * (sepHorizontal * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            if (hit)
            {
                atLeastonce = hit;
                if (hit.collider.gameObject.layer == 9)
                {
                    if (directionY == -1)
                    {
                        if (platform == null)
                        {
                            platform = hit.collider.GetComponent<Platform>();
                            platform.inPlatform.Add(this);
                            collisionInfo.bottom = true;
                            collisionInfo.inPlatform = true;
                        }
                    }
                }

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionInfo.top = directionY == 1;
                collisionInfo.bottom = directionY == -1;
            }
            if (!collisionInfo.bottom && platform != null)
            {

                platform.inPlatform.Remove(this);
                platform = null;
                collisionInfo.inPlatform = false;
            }
        }

        // Este raycast chequea si quedamos en una nueva slope
        if (collisionInfo.climbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionY == -1 ? origins.bottomLeft : origins.topLeft);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength + skinWidth, collisionMask);

            if (hit)
            {
                float ang = Vector2.Angle(hit.normal, Vector2.up);
                if (ang <= maxSlopeAngle && ang != 90f)
                {
                    float compensation = 0;

                    if (collisionInfo.oldSlopeAngle != ang)
                    {
                        compensation = hit.distance - skinWidth;
                        velocity.x += compensation * directionX;
                    }
                }
            }
        }
    }


    private void ClimbSlope(ref Vector2 velocity, float angle)
    {
        float d = Mathf.Abs(velocity.x);
        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * d;

        if (velocity.y <= y)
        {
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * d;

            velocity.y = y;
            velocity.x = x * Mathf.Sign(velocity.x);

            collisionInfo.bottom = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = angle;
        }
    }

    private void DescendSlope(ref Vector2 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLenght = velocity.y + skinWidth;
        Vector2 rayOrigin = (directionX == -1 ? origins.bottomRight : origins.bottomLeft);
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, 100, collisionMask);
        if (hit)
        {
            float angle = Vector2.Angle(hit.normal, Vector2.up);
            if (angle != 0 && angle <= maxSlopeAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {
                    if (hit.distance - skinWidth <= Mathf.Tan(angle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x) && velocity.x != 0)
                    {
                        float d = Mathf.Abs(velocity.x);
                        float x = Mathf.Cos(angle * Mathf.Deg2Rad) * d * Mathf.Sign(velocity.x);
                        float y = Mathf.Sin(angle * Mathf.Deg2Rad) * d;

                        velocity.x = x;
                        velocity.y -= y;

                        collisionInfo.slopeAngle = angle;
                        collisionInfo.bottom = true;
                        collisionInfo.descendigSlope = true;
                    }
                }
            }
        }
    }

    public void UsePlatform()
    {

    }

    void UpdateRaycastOrigins()
    {
        Bounds bounds = coll.bounds;
        bounds.Expand(skinWidth * -2);

        origins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        origins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        origins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        origins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    public struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }


    public struct CollisionInfo
    {
        public bool climbingSlope;
        public bool descendigSlope;
        public bool top;
        public bool bottom;
        public bool right;
        public bool left;
        public bool inPlatform;

        public float oldSlopeAngle;
        public float slopeAngle;

        public void ResetInfo()
        {
            climbingSlope = false;
            descendigSlope = false;
            top = false;
            bottom = false;
            left = false;
            right = false;
            inPlatform = false;

            oldSlopeAngle = slopeAngle;
            slopeAngle = 0;
        }
    }
}
