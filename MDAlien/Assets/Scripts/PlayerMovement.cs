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
    private float xAxis, yAxis;

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

    [Header("Attacking")]
    public float damagePower = 10f;
    public float attackCooldown = 1f;
    public float attackTime = 0f;
    public Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;   
    public Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    public LayerMask attackLayer;



    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckY = 0.2f; 
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer; 

    private bool isGrounded;
    private bool isJumping; 
    private bool isAttacking;
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
        Attack();
        
        anim.SetBool("jumping", rb.velocity.y > 0.1 && !isGrounded);
        anim.SetBool("falling", !isGrounded && rb.velocity.y < 0);
    }

    
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(groundCheck.position, Vector2.down * groundCheckY);
        Gizmos.DrawRay(groundCheck.position + new Vector3(groundCheckX, 0f, 0f), Vector2.down * groundCheckY);
        Gizmos.DrawRay(groundCheck.position + new Vector3(-groundCheckX, 0f, 0f), Vector2.down * groundCheckY);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(SideAttackTransform.position, SideAttackArea);
        Gizmos.DrawWireCube(UpAttackTransform.position, UpAttackArea);
        Gizmos.DrawWireCube(DownAttackTransform.position, DownAttackArea);
    }

    public void GetInputs()
    {
        xAxis = Input.GetAxisRaw("Horizontal");
        yAxis = Input.GetAxisRaw("Vertical");
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
                isJumping = true; // Marcamos que está saltando
                pstate.jumping = true;
                jumpBuffer = 0; // Reiniciar el buffer después de usarlo
                if (!isGrounded) airJumpCounter++; // Contar el salto aéreo
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


    void Attack()
    {
        attackTime += Time.deltaTime;
        if (Input.GetButtonDown("Fire1") && attackTime >= attackCooldown)
        {
            attackTime = 0;
            isAttacking = true;
            anim.SetTrigger("attacking");

            if (xAxis == 0 || yAxis < 0 && isGrounded)
            {
                Hit(SideAttackTransform, SideAttackArea);
            }
            else if (yAxis > 0)
            {
                Hit(UpAttackTransform, UpAttackArea);
            }
            else if (yAxis < 0 && !isGrounded)
            {
                Hit(DownAttackTransform, DownAttackArea);
            }
        }
    }

    public void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsHited = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);

        if (objectsHited.Length > 0)
        {
           Debug.Log("Hited: " + objectsHited.Length + " objects");
        }

        for (int i = 0; i < objectsHited.Length; i++)
        {
            if (objectsHited[i].GetComponent<Enemy>() != null) 
            {
                objectsHited[i].GetComponent<Enemy>().EnemyHited(damagePower);

            }
        }
    }


}
