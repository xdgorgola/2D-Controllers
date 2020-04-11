using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb2d = null;
    private Collider2D col2d = null;

    public float gravedad = 10f;
    // Va de -1 a 1
    public float inercia = 0f;
    public float rapidezX = 6f;

    public Vector2 velocidad = Vector2.zero;
    private Vector2 destranque = Vector2.zero;

    private Vector2 input = Vector2.zero;

    public ImportantInfo info = new ImportantInfo();

    private bool moved = false;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        col2d = GetComponent<Collider2D>();
    }

    private void Update()
    {
        moved = false;
        destrancao = false;
        destranque = Vector2.zero;
        HandleGravity();
        HandlePlayerInput();
        HandleInertia();
        // Y ahora te mueves
    }

    private void HandlePlayerInput()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        if (!info.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("metele cania para que saltes jeje");
        }
    }

    private void HandleGravity()
    {
        if (!info.isGrounded)
        {
            velocidad.y -= gravedad * Time.deltaTime;
        }
    }

    private void HandleInertia()
    {
        float velSign = Mathf.Sign(velocidad.x);
        float inputSign = Mathf.Sign(input.x);
        if (info.isGrounded)
        {
            inercia = 0;
        }
        if (inercia != 0)
        {
            if (velocidad.x != 0 && input.x != 0)
            {
                float inSign = Mathf.Sign(inercia);
                if (velSign == inSign)
                {
                    // Se puede cambiar
                    velocidad.x += velSign * 2f;
                }
                else
                {
                    velocidad.x -= velSign * rapidezX / 4f;
                    if (inSign == -1)
                    {
                        inercia = Mathf.Clamp(inercia - inSign * 0.15f, -1f, 0f);
                    }
                    else
                    {
                        inercia = Mathf.Clamp(inercia - inSign * 0.15f, 0f, 1f);
                    }
                }
            }
        }
        else
        {
            velocidad.x = input.x * rapidezX;
        }
    }

    public struct ImportantInfo
    {
        public bool isGrounded;
    }

    private bool destrancao = false;

    private void FixedUpdate()
    {
        if (moved)
        {
            if (!destrancao)
            {
                GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + destranque);
                destrancao = true;
                Debug.Log("Reacomodando");
            }
        }
        GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + velocidad * Time.deltaTime);
        moved = true;
        destranque = Vector2.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("epa mamahuevo, piso");
        if (!info.isGrounded)
        {
            float dirY = 0;
            float penetration = Mathf.Abs(collision.bounds.max.y - col2d.bounds.min.y);
            //GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + Vector2.up * penetration);
            //transform.Translate(Vector2.up * penetration);
            //Physics2D.SyncTransforms();
            destranque += Vector2.up * penetration;
            info.isGrounded = true;
            velocidad.y = 0;
        }
    }
}
