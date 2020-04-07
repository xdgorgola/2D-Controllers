using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovementController : MonoBehaviour
{
    public float flyingTime = 2.5f;
    public float maxHeight = 10f;

    private float gravity;
    private float jumpVel;
    public float movementSpeed = 5f;

    private MovementCollisionSolver solver;

    public Vector2 velocity = Vector2.zero;

    private void Awake()
    {
        solver = GetComponent<MovementCollisionSolver>();
    }


    private void Update()
    {
        Vector2 input = Vector2.right * Input.GetAxisRaw("Horizontal");
        gravity = -(maxHeight * 2f) / (flyingTime * flyingTime);
        jumpVel = Mathf.Abs(gravity * flyingTime);

        if (solver.collisionInfo.top || solver.collisionInfo.bottom)
        {
            velocity.y = 0;
        }

        if (solver.collisionInfo.bottom && Input.GetKeyDown(KeyCode.Space))
        {
                velocity.y = jumpVel;
        }
        velocity.y += gravity * Time.deltaTime;
        velocity.x = input.x * movementSpeed;
        solver.Move(velocity * Time.deltaTime);
        Physics2D.SyncTransforms();
    }
}