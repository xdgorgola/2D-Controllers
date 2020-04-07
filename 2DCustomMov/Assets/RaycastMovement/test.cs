using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Physics2D.SyncTransforms();
        GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + Vector2.right);
        Physics2D.SyncTransforms();
        GetComponent<Rigidbody2D>().MovePosition((Vector2)transform.position + Vector2.left);
        Physics2D.SyncTransforms();
    }
}
