using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Prime31;

public class Player : MonoBehaviour
{
    public float gravity = -10, moveSpeed = 10f, jumpHeight = 8f, centreRadius = .5f;

    public CharacterController2D controller;
    private SpriteRenderer rend;
    public Animator anim;

    private Vector3 velocity;
    private bool isClimbing = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, centreRadius);
    }

    void Start()
    {
        controller = GetComponent<CharacterController2D>();
        rend = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        // If character is:
        if (!controller.isGrounded && // NOT grounded
            !isClimbing) // NOT climbing
        {
            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }
        // If space is pressed
        if (Input.GetButtonDown("Jump"))
        {
            // Make the player jump
            Jump();
        }
        // Climb up or down depending on Y value
        Climb(inputV, inputH);
        // Move left or right depending on X value
        Move(inputH);

        if (!isClimbing)
        {
            // Move the controller with modified motion
            controller.move(velocity * Time.deltaTime);
        }
    }

    public void Move(float inputH)
    {
        // 1
        velocity.x = inputH * moveSpeed;
        // 2
        anim.SetBool("IsRunning", inputH != 0);
        // 3
        rend.flipX = inputH < 0;
    }

    public void Climb(float inputV, float inputH)
    {
        bool isOverLadder = false; // Is the player overlapping the ladder?
        Vector3 inputDir = new Vector3(inputH, inputV, 0);

        #region Part 1 - Detecting Ladders
        // Get a list of all hit objects overlapping point
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, centreRadius);
        // Loop through all hit objects
        foreach (var hit in hits)
        {
            //  Check if tagged "Ladder"
            if (hit.tag == "Ladder")
            {
                // Player is overlapping a Ladder!
                isOverLadder = true;
                break; // Exit just the foreach loop (works for any loop)
            }
        }
        // If the player is overlapping AND input vertical is made
        if (isOverLadder && inputV != 0)
        {
            anim.SetBool("IsClimbing", true);
            // The player is in Climbing state!
            isClimbing = true;
        }
        #endregion

        #region Part 2 - Translating the Player
        // If player is climbing
        if (isClimbing)
        {
            velocity.y = 0;
            // Move player up and down on the ladder (additionally move left and right)
            transform.Translate(inputDir * moveSpeed * Time.deltaTime);
        }
        #endregion

        if (!isOverLadder)
        {
            anim.SetBool("IsClimbing", false);
            isClimbing = false;
        }

        anim.SetFloat("ClimbSpeed", inputDir.magnitude * moveSpeed);
    }

    public void Hurt()
    {

    }

    public void Jump()
    {
        // 4
        velocity.y = jumpHeight;
    }
}
