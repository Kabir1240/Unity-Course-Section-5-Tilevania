using System.Text.RegularExpressions;
// using System.Threading.Tasks.Dataflow;
using System;
// using System.Threading.Tasks.Dataflow;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Numerics;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 2f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] float deathJump = 30f;
    float gravity;
    bool isAlive = true;


    // Transform playerTransform;
    Animator myAnimator;
    UnityEngine.Vector2 moveInput;
    Rigidbody2D rb2d;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    SpriteRenderer sprite;
    [SerializeField] Transform gun;
    [SerializeField] GameObject bullet;


    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        gravity = rb2d.gravityScale;
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        // playerTransform = GetComponent<Transform>();
    }

    void Update()
    {
        if (!isAlive){return;}
        Run();
        RunAnimation();
        ClimbLadder();
        ClimbAnimation();
        Die();
    }

    void OnMove(InputValue value)
    {
        if (!isAlive){return;}
        moveInput = value.Get<UnityEngine.Vector2>();
    }

    void OnJump (InputValue value)
    {
        if (!isAlive){return;}
        if(value.isPressed && myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")) && !rb2d.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            rb2d.velocity += new UnityEngine.Vector2 (0f, jumpSpeed);
        }
        else{}{return;}
    }

    void OnFire(InputValue value)
    {
        if (!isAlive){return;}
        Instantiate(bullet, gun.position, transform.rotation);
    }

    void Run()
    {
        UnityEngine.Vector2 playerVelocity = new UnityEngine.Vector2 (moveInput.x * runSpeed, rb2d.velocity.y);
        rb2d.velocity = playerVelocity;
    }   

    void RunAnimation()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(rb2d.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            myAnimator.SetBool("isRunning", true);
            transform.localScale = new UnityEngine.Vector2 (Mathf.Sign(rb2d.velocity.x), 1f);
        }
        else
        {
            myAnimator.SetBool("isRunning", false);
        }
    }

    void ClimbLadder()
    {
        if (myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            UnityEngine.Vector2 climbVelocity = new UnityEngine.Vector2 (rb2d.velocity.x, moveInput.y * climbSpeed);
            rb2d.velocity = climbVelocity;
            rb2d.gravityScale = 0f;
        }
        else
        {
            rb2d.gravityScale = gravity;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
    }

    void ClimbAnimation()
    {
        bool playerHasVerticalSpeed = Mathf.Abs(rb2d.velocity.y) > Mathf.Epsilon;
        if (playerHasVerticalSpeed && rb2d.IsTouchingLayers(LayerMask.GetMask("Ladder")))
        {
            myAnimator.SetBool("isClimbing", true);
        }
        else
        {
            myAnimator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemy", "Hazards"))) 
        {
            isAlive = false;
            rb2d.velocity += new UnityEngine.Vector2 (0f, deathJump);
            myAnimator.SetTrigger("Dead");
            sprite.color = new Color(1f, 0f, 0f, 1f);
            
            FindObjectOfType<GameSession>().processPlayerDeath();
        }
    }
}
