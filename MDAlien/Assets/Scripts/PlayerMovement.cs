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
    
    [Header("GroundCheck")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2 (0.5f, 0.05f);
    public LayerMask groundLayer;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = new Vector2(horizontalMovement * moveSpeed, rb.velocity.y);
        flip();
        animator.SetFloat("yVelocity", rb.velocity.y);
        animator.SetFloat("magnitude", rb.velocity.magnitude);
    }
	
	public void Move(InputAction.CallbackContext context) {
		horizontalMovement = context.ReadValue<Vector2>().x;
	}

    public void Jump(InputAction.CallbackContext context) {
		if (isGrounded()) {
            if (context.performed) {
                rb.velocity=new Vector2(rb.velocity.x, jumpPower);
                animator.SetTrigger("jump");
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
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
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