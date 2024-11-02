using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    public Animator animator;
    bool isFacingRight = true;
    
    [Header("Movement")]
    public float moveSpeed = 5f;
    float horizontalMovement;

    [Header("Jumping")]
    public float jumpPower = 10f;
    private int jumpCount = 0;
    private int maxJumps = 1; // Permitir doble salto
    
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2 (0.5f, 0.05f);
    public LayerMask groundLayer;

    [Header("Attacking")]
    public Transform AttackCheck;
    public Vector2 AttackSize = new Vector2 (0.5f, 0.05f);
    public LayerMask Enemy;
    public float BetweenAttack, SinceAttack, AttackPower;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        SinceAttack += Time.deltaTime;
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        flip();
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);

        if (isGrounded()) {
            jumpCount = 0; // Resetear el contador de saltos al tocar el suelo
        }
    }
    
    public void Move(InputAction.CallbackContext context) {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context) {
        if (context.performed && jumpCount < maxJumps) {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            animator.SetTrigger("jump");
            jumpCount++;
        }
    }

    public void Attack(InputAction.CallbackContext context) {
        
        if (SinceAttack > BetweenAttack) {
            animator.SetTrigger("Attack");
            SinceAttack = 0;
            Hit(AttackCheck, AttackSize);
        }
        
    }

    private void Hit(Transform tr, Vector2 v2) {
        Collider2D[] objectsHited = Physics2D.OverlapBoxAll(tr.position, v2, 0, Enemy);

        if (objectsHited.Length > 0 ) {
            Debug.Log("Hited");
        }

        for(int i = 0; i < objectsHited.Length; i++) {
            if(objectsHited[i].GetComponent<Enemy>() != null) {
                objectsHited[i].GetComponent<Enemy>().EnemyHit(AttackPower, (transform.position - objectsHited[i].transform.position).normalized, 3);
            }
        }
    }

    private bool isGrounded() {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer)) {
            return true;
        }
        return false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(AttackCheck.position, AttackSize);
    }

    void flip() {
        if ((isFacingRight && horizontalMovement < 0) || (!isFacingRight && horizontalMovement > 0)) {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}