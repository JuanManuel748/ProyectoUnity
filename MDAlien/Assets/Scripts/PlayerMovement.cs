using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private Rigidbody2D rb;
    private Animator anim;
    public float walkSpeed = 10f;
    public float jumpForce = 10f;
    private float xAxis;

    [Header("Jumping")]
    private int jumpBuffer = 0;
    private int jumpBufferFrames = 10;
    private float coyoteTimeCounter = 0;
    private float coyoteTime = 0.2f;
    private int airJumpCounter = 0;
    private int airJumpMax = 1;

    [Header("Dashing")]
    private float dashSpeed = 50f;
    private float dashTime = 0.1f;
    private float dashCooldown = 1f;
    private bool canDash = true;
    private bool isDashing = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckY = 0.2f; 
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer; 

    private bool isGrounded;
    private bool isJumping; 
    private float gravity;
    private PlayerStateList pstate;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pstate = GetComponent<PlayerStateList>();
        gravity = rb.gravityScale;
    }

    void Update() 
    {
        GetInputs();
        UpdateJump(); 
        if (isDashing) return; // Si está en el dash, no ejecutar más lógica.

        Move();
        CheckGround();
        Jump();
        Flip();
        
        anim.SetBool("jumping", isJumping);
        // Actualizar animaciones
        anim.SetBool("falling", !isGrounded && rb.velocity.y < 0);
    }

    public void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");

        // Llamar a StartDash para verificar si se inicia un dash
        StartDash();
    }

    void Flip()
    {
        if (xAxis > 0)
        {
            transform.localScale = new Vector2(2.5f, transform.localScale.y);
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(-2.5f, transform.localScale.y); 
        }
    }

    public void Move()
    {
        rb.velocity = new Vector2(xAxis * walkSpeed, rb.velocity.y);
        anim.SetBool("running", xAxis != 0 && isGrounded);
    }

    private void CheckGround()
    {
        isGrounded = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckY, groundLayer) || 
                      Physics2D.Raycast(groundCheck.position + new Vector3(groundCheckX, 0f, 0f), Vector2.down, groundCheckY, groundLayer) || 
                      Physics2D.Raycast(groundCheck.position + new Vector3(-groundCheckX, 0f, 0f), Vector2.down, groundCheckY, groundLayer);
    }

    private void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jumpBuffer = jumpBufferFrames; // Buffer para el salto
        }

        if (jumpBuffer > 0)
        {
            if (isGrounded || (airJumpCounter < airJumpMax))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                pstate.jumping = true;
                isJumping = true; // Marcamos que está saltando
                jumpBuffer = 0; // Reiniciar el buffer después de usarlo
                if (!isGrounded) airJumpCounter++; // Contar el salto aéreo
                if (rb.velocity.y < 0) isJumping = false;
            }
        }
    }

    void UpdateJump()
    {
        if (isGrounded)
        {
            pstate.jumping = false; 
            airJumpCounter = 0; // Reiniciar el contador de saltos aéreos
            isJumping = false; // Resetear el estado de salto al tocar el suelo
        }

        if (jumpBuffer > 0)
        {
            jumpBuffer--; 
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true; // Indicar que el jugador está en el dash
        pstate.dashing = true;
        anim.SetTrigger("dashing");
        rb.gravityScale = 0;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, 0);
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity;
        pstate.dashing = false;
        isDashing = false; // Resetear el estado de dash
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    void StartDash()
    {
        if (Input.GetButtonDown("Fire3") && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * groundCheckY);
        Gizmos.DrawRay(groundCheck.position + new Vector3(groundCheckX, 0f, 0f), Vector2.down * groundCheckY);
        Gizmos.DrawRay(groundCheck.position + new Vector3(-groundCheckX, 0f, 0f), Vector2.down * groundCheckY);
    }
}
