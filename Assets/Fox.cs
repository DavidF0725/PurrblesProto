using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{
    //Public Fields
    public float speed = 1;

    //Inspector Variables
     Rigidbody2D rb;
    Animator anim;
    float horizontalValue;
    float runSpeedModifier = 2f;
    bool isRunning = false;
    bool facingRight = true;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //Store the horizontal value 
        horizontalValue = Input.GetAxisRaw("Horizontal");

        //If the user presses Left shift to run
        if(Input.GetKeyDown(KeyCode.LeftShift))        
            isRunning = true;
        //If the user releases left shift, stops running
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
    }


    //Every time the user presses a key
    void FixedUpdate()
    {
        Move(horizontalValue);
    }


    void Move(float direction)
    {
        //Set value of x and direction and speed
        float xValue = direction * speed * 100 * Time.deltaTime;

        //If we are running, multiply with the running modifier 
        if (isRunning)
            xValue *= runSpeedModifier;
        //Set Vector2 for the velocity
        Vector2 targetVelocity = new Vector2(xValue, rb.velocity.y);
        //Set the player's velocity
        rb.velocity = targetVelocity;

        //Move Left
        if (facingRight && direction < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            facingRight = false;
        }
        //Move Right
        else if (!facingRight && direction > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
            facingRight = true;
        }

        //(0 idle, 4 walking, 8 running)
        //Set the float xVelocity according to the x value 
        // of the RigidBody2D velocity
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
    }
}
