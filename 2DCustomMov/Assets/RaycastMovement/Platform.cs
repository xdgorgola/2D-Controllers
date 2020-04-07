using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Platform : MonoBehaviour
{
    private Collider2D coll;

    public bool mooved = false;
    public List<MovementCollisionSolver> inPlatform = new List<MovementCollisionSolver>();
    public List<MovementCollisionSolver> sidePlatform = new List<MovementCollisionSolver>();

    public float speed = 5f;
    public Vector2 direction = Vector2.right;

    private void Awake()
    {
        coll = GetComponent<Collider2D>();
    }

    private void Update()
    {
        MovePlatform();
    }

    public void MovePlatform()
    {
        mooved = true;
        CheckSidePassenger();
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
        Physics2D.SyncTransforms();
        mooved = false;
        foreach (MovementCollisionSolver inp in inPlatform)
        {
            //Debug.Log(speed * Time.deltaTime);
            //inp.GetComponent<Rigidbody2D>().MovePosition((Vector2)inp.transform.position + direction * speed * Time.fixedDeltaTime);
            //inp.GetComponent<Rigidbody2D>().MovePosition(Vector2.up * 1000);

            //Debug.Log(inp.GetComponent<Rigidbody2D>().position);
            //inp.Move(direction * speed * Time.deltaTime + Vector2.down * 0.01f);
            inp.MovePlatform(direction * speed * Time.deltaTime);
            Physics2D.SyncTransforms();
        }
        foreach (MovementCollisionSolver inp in sidePlatform)
        {
            //Debug.Log(speed * Time.deltaTime);
            //inp.GetComponent<Rigidbody2D>().MovePosition((Vector2)inp.transform.position + direction * speed * Time.fixedDeltaTime);
            //inp.GetComponent<Rigidbody2D>().MovePosition(Vector2.up * 1000);

            //Debug.Log(inp.GetComponent<Rigidbody2D>().position);
            //inp.Move(direction * speed * Time.deltaTime + Vector2.down * 0.01f);
            inp.MovePlatform(direction * speed * Time.deltaTime);
            Physics2D.SyncTransforms();
        }
    }

    private void CheckSidePassenger()
    {
        Bounds bounds = coll.bounds;
        float vertSep = Mathf.Abs(bounds.max.y - bounds.min.y) / 2;
        float dirX = Mathf.Sign(direction.x);
        Vector2 origin = (dirX == 1 ? (bounds.max.y * Vector2.up + bounds.max.x * Vector2.right) : (bounds.max.y * Vector2.up + bounds.min.x * Vector2.right));
        List<MovementCollisionSolver> final = new List<MovementCollisionSolver>();
        for (int i = 0; i < 3; i++)
        {
            Vector2 movedOrig = origin - Vector2.up * (i * vertSep);
            RaycastHit2D hit = Physics2D.Raycast(movedOrig, Vector2.right * dirX, direction.x * speed * Time.deltaTime, 1 << 10);
            if (hit)
            {
                if (hit.collider.TryGetComponent(out MovementCollisionSolver passenger))
                {
                    final.Add(passenger);
                    if (!sidePlatform.Contains(passenger))
                    {
                        sidePlatform.Add(passenger);
                        passenger.transform.Translate(direction * -dirX * hit.distance);
                        Physics2D.SyncTransforms();
                    }
                }
            }
        }
        sidePlatform = sidePlatform.Intersect(final).ToList<MovementCollisionSolver>();
    }
}
