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
    [SerializeField] int totalJumps;
    int availableJumps;
    float horizontalValue;
    float runSpeedModifier = 2f;
    float crouchSpeedModifier = 0.5f;

    [SerializeField] bool isGrounded = false;
    bool isRunning = false;
    bool facingRight = true;
    bool crouchPressed = false;
    bool multipleJump;
    bool coyoteJump;

    void Awake()
    {
        availableJumps = totalJumps;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //Set the yVelocity in the animator
        anim.SetFloat("yVelocity", rb.velocity.y);

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
            Jump();
                  
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
        Move(horizontalValue, crouchPressed);
    }

    void GroundCheck()
    {
        bool wasGrounded = isGrounded;
        isGrounded = false;

        //Check if the player is touching the ground 
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckColl.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
            if (!wasGrounded)
            {
                availableJumps = totalJumps;
                //Figure out if remove the code below:
                multipleJump = false;
            }                                      
        }
        else
        {
            if (wasGrounded)
                StartCoroutine(CoyoteJumpDelay());
        }
            
        //If detect ground the jump bool is disabled in anim
        anim.SetBool("Jump", !isGrounded);

    }

    #region Jump
    IEnumerator CoyoteJumpDelay()
    {
        coyoteJump = true;
        yield return new WaitForSeconds(0.2f); //This float value can be changed depending on the gameplay
        coyoteJump = false;
    }

    void Jump()
    {
        //If crouch - standingCollider = false + enable crouch anim
        //Reduce the x velocity
        //If uncrouch - standingCollider = true + disable crouch anim
        //Jump
        if (isGrounded)
        {
            multipleJump = true;
            availableJumps--;

            rb.velocity = Vector2.up * jumpPower;
            anim.SetBool("Jump", true);
        }
        else
        {
            if(coyoteJump)
            {
                multipleJump = true;
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                anim.SetBool("Jump", true);
                Debug.Log("Coyote Jump");
            }

            if (multipleJump && availableJumps > 0)
            {
                availableJumps--;

                rb.velocity = Vector2.up * jumpPower;
                anim.SetBool("Jump", true);
            }
        }        
    }
    #endregion

    void Move(float direction, bool crouchFlag)
    {
        #region Crouch

        //If we are crouch and then disable crouching
        //Check above player head for collisions
        //If there are any - stay crouched | if not, uncrouch
        if(!crouchFlag)
        {
            if (Physics2D.OverlapCircle(ceilingCheckColl.position, ceilingCheckRadius, groundLayer))
                crouchFlag = true;
        }

        anim.SetBool("Crouch", crouchFlag);
        standingCollider.enabled = !crouchFlag;
        

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
