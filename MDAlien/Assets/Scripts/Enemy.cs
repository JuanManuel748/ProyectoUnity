using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public float maxHealth;
    protected float health;     
    public float speed;
    public float damagePower;

    public Transform objective;
    protected bool follow;
    public bool isInvincible;
    protected float distance;
    protected float absoluteDistance;
    public float recoilForce = 6f;

    protected Rigidbody2D rb;
    protected Animator anim;

    private float attackRange = 0.5f; // Added for clarity of attack range

    protected virtual void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        distance = objective.position.x - transform.position.x;
        absoluteDistance = Mathf.Abs(distance);

        follow = absoluteDistance < 20f;

        if (follow)
        {
            Follow();
            Attack(); // Call attack while following
        }
    }

    protected virtual void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, objective.position, speed * Time.deltaTime);
        //transform.localScale = new Vector3(distance < 0 ? 1 : -1, 1, 1); // Flip sprite direction based on distance from objective 
    }

    public void EnemyHit(float _damage, Vector2 _direction)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            health -= _damage;
            anim.SetTrigger("hurt");

            Vector2 rebote = new Vector2(_direction.x, 0.2f).normalized; // Use passed-in _direction
            rb.AddForce(rebote * recoilForce, ForceMode2D.Impulse);

            if (health <= 0)
            {
                Die();
            }
            StartCoroutine(Invincible()); // Corrected coroutine call
        }
    }

    private IEnumerator Invincible()
    {
        yield return new WaitForSeconds(2);
        isInvincible = false;
    }

    protected void Die()
    {
        anim.SetTrigger("death");
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 20f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange); // Visualize attack range
    }

    protected void Attack()
    {
        if (absoluteDistance < attackRange) // Use defined attack range
        {
            rb.velocity = Vector2.zero;

            if (Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player")))
            {
                PlayerMovement player = objective.GetComponent<PlayerMovement>(); // Use objective to find player
                if (player != null)
                {
                    Debug.Log(this + " hited the player");
                    anim.SetTrigger("attack");
                    Vector2 _direction = (transform.position - player.transform.position).normalized;
                    player.Hurt(damagePower, _direction);
                }
            }
        }
    }
}
