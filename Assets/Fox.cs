using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fox : MonoBehaviour
{   
    
    Rigidbody2D rb;
    Animator anim;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Transform groundCheckColl;
    [SerializeField] Transform ceilingCheckColl;
    [SerializeField] LayerMask groundLayer;

    //Inspector Variables
    const float groundCheckRadius = 0.2f;
    const float ceilingCheckRadius = 0.2f;
    [SerializeField] float speed = 2;
    [SerializeField] float jumpPower = 500;
    float horizontalValue;
    float runSpeedModifier = 2f;
    float crouchSpeedModifier = 0.5f;

    [SerializeField] bool isGrounded = false;
    bool isRunning = false;
    bool facingRight = true;
    bool jump = false;
    bool crouchPressed = false;

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

        //If we press jump
        if (Input.GetButtonDown("Jump"))
            jump = true;
        //If we release space bar - fall
        else if (Input.GetButtonUp("Jump"))
            jump = false;

        //If we press crouch
        if (Input.GetButtonDown("Crouch"))
            crouchPressed = true;
        //If we release crouch - stand
        else if (Input.GetButtonUp("Crouch"))
            crouchPressed = false;
    }


    //Every time the user presses a key
    void FixedUpdate()
    {
        GroundCheck();
        Move(horizontalValue, jump, crouchPressed);
    }

    void GroundCheck()
    {
        isGrounded = false;

        //Check if the player is touching the ground 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckColl.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
            isGrounded = true;
    }


    void Move(float direction, bool jumpFlag, bool crouchFlag)
    {
        #region Jump and Crouch

        //If we are crouch and then disable crouching
        //Check above player head for collisions
        //If there are any - stay crouched | if not, uncrouch
        if(!crouchFlag)
        {
            if (Physics2D.OverlapCircle(ceilingCheckColl.position, ceilingCheckRadius, groundLayer))
                crouchFlag = true;
        }



        //If crouch - standingCollider = false + enable crouch anim
        //Reduce the x velocity
        //If uncrouch - standingCollider = true + disable crouch anim
        if(isGrounded)
        {
            standingCollider.enabled = !crouchFlag;
            //Jump
            if (jumpFlag)
            {
                isGrounded = false;
                jumpFlag = false;
                //Jump force implemented
                rb.AddForce(new Vector2(0f, jumpPower));
            }
        }

        anim.SetBool("Crouch", crouchFlag);

        #endregion

        #region Move and Run
        //Set value of x and direction and speed
        float xValue = direction * speed * 100 * Time.fixedDeltaTime;

        //If we are running, multiply with the running modifier (higher)
        if (isRunning)
            xValue *= runSpeedModifier;
        //If we are running, multiply with the running modifier (higher)
        if (crouchFlag)
            xValue *= crouchSpeedModifier;
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
        #endregion
    }
}
