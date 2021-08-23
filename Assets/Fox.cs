using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    //Public Fields
    public float speed = 1;

    //Inspector Variables
    Rigidbody2D rb;
    float horizontalValue;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        horizontalValue = Input.GetAxisRaw("Horizontal");
    }


    //Every time the user presses a key
    void FixedUpdate()
    {
        Move(horizontalValue);
    }


    void Move(float direction)
    {
        float xValue = direction * speed * 100 * Time.deltaTime;
        Vector2 targetVelocity = new Vector2(xValue, rb.velocity.y);
        rb.velocity = targetVelocity;
    }
}
