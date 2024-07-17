using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StopBall()
    {
        rb.velocity = Vector2.zero;
        rb.isKinematic = true;
    }

    public void StartBall()
    {
        rb.isKinematic = false;
    }
}
