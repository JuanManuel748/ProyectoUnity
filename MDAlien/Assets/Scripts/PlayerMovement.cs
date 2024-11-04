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
    private float recoilForce = 6f;
    public float attackTime = 0f;
    public Transform SideAttackTransform, UpAttackTransform, DownAttackTransform;   
    public Vector2 SideAttackArea, UpAttackArea, DownAttackArea;
    public LayerMask attackLayer;
    public GameObject SlashEffect;

    [Header("Health")]
    public float maxHealth = 3f;
    private float health;



    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; 
    [SerializeField] private float groundCheckY = 0.2f; 
    [SerializeField] private float groundCheckX = 0.5f;
    [SerializeField] private LayerMask groundLayer; 

    private bool isGrounded;
    public bool tieneLlave;
    private bool isJumping; 
    private bool isAttacking;
    private float gravity;
    public bool facingRight;
    public bool isInvincible;
    private PlayerStateList pstate;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pstate = GetComponent<PlayerStateList>();
        gravity = rb.gravityScale;
        health = maxHealth;
        tieneLlave = false;
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
            facingRight = true;
        }
        else if (xAxis < 0)
        {
            transform.localScale = new Vector2(-2.5f, transform.localScale.y); 
            facingRight = false;
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

        // Verifica si el jugador puede atacar y no está en medio de un ataque
        if (Input.GetButtonDown("Fire1") && attackTime >= attackCooldown && !isAttacking)
        {
            bool animated = false;
            attackTime = 0;
            isAttacking = true; // Indica que el jugador está atacando
            anim.SetTrigger("attacking");

            // Ataque lateral
            
            // Ataque hacia arriba
            if (yAxis > 0 && !animated) 
            {
                Hit(UpAttackTransform, UpAttackArea);
                SlashEffectAngle(SlashEffect, 90, UpAttackTransform);
                animated = true;
            }

            // Ataque hacia abajo
            if (yAxis < 0 && !isGrounded && !animated) 
            {
                Hit(DownAttackTransform, DownAttackArea);
                SlashEffectAngle(SlashEffect, -90, DownAttackTransform);
                animated = true;
            }

            if (xAxis <= 0 && !animated) 
            {
                Hit(SideAttackTransform, SideAttackArea);
                SlashEffectAngle(SlashEffect, 0, SideAttackTransform);
                animated = true;
            }

            // Ataque lateral
            if (xAxis >= 0 && !animated) 
            {
                Hit(SideAttackTransform, SideAttackArea);
                SlashEffectAngle(SlashEffect, 180, SideAttackTransform);
                animated = true;
            }


            // Reiniciar isAttacking después del tiempo de ataque (puedes ajustar esto si es necesario)
            StartCoroutine(ResetAttack());
        }
    }

    private IEnumerator ResetAttack()
    {
        // Espera un tiempo que sea igual a la duración del ataque
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false; // Permite que el jugador ataque de nuevo
    }




    public void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsHited = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackLayer);

        

        foreach (Collider2D obj in objectsHited)
        {
            // Asegúrate de que el objeto tenga el componente Enemy antes de intentar acceder a él
            Enemy enemy = obj.GetComponent<Enemy>();
            Debug.Log(obj.name + "hited");
            if (enemy != null)
            {
                Vector2 _direction = new Vector2(transform.position.x - obj.transform.position.x, 0).normalized;
                enemy.EnemyHit(damagePower, _direction);
            }
        }
    }


    private void SlashEffectAngle(GameObject _slash, int _angle, Transform _attackTransform)
    {
        // Instancia el efecto de "slash"
        GameObject slashInstance = Instantiate(_slash, _attackTransform.position, Quaternion.Euler(0, 0, _angle));

        // Ajusta la escala del efecto para que ocupe todo el transform
        float attackWidth = (_attackTransform.localScale.x) * 0.4f;
        float attackHeight = (_attackTransform.localScale.y) * 0.4f;

        // Ajusta la dirección del slash según la orientación del personaje
        if (facingRight)
        {
            // Si está mirando a la derecha, mantén la escala positiva
            slashInstance.transform.localScale = new Vector3(attackWidth, attackHeight, 1);
        }
        else
        {
            // Si está mirando a la izquierda, invierte la escala en el eje X
            slashInstance.transform.localScale = new Vector3(-attackWidth, attackHeight, 1);
        }

        // Rotación adicional para ataques hacia arriba y hacia abajo
        if (_angle == 90) // Ataque hacia arriba
        {
            slashInstance.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (_angle == -90) // Ataque hacia abajo
        {
            slashInstance.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
    }


    public void Hurt(float _damage, Vector2 _direction)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            Debug.Log("Player hurted");
            health -= _damage;
            anim.SetTrigger("hurt");
            Vector2 rebote = new Vector2(transform.position.x - _direction.x, 0.2f).normalized;
            rb.AddForce(rebote * recoilForce, ForceMode2D.Impulse);
            if (health <= 0)
            {
                Die();
            }
            // despues de 2 segundos poner invincible a false
            StartCoroutine(Invincible());

        }
    }

    IEnumerator Invincible()
    {
        yield return new WaitForSeconds(2);
        isInvincible = false;
    }

    private void Die()
    {
        anim.SetTrigger("death");
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Key"))
        {
            tieneLlave = true;
            Destroy(collision.gameObject);
        }
    }

    public bool TieneLlave()
    {
        return tieneLlave;
    }

}
