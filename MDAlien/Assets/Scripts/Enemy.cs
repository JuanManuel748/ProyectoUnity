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
    public float range = 20f;

    public Transform objective;
    protected bool follow;
    public bool isInvincible;
    protected float distance;
    protected float absoluteDistance;
    public float recoilForce = 6f;

    protected Rigidbody2D rb;
    protected Animator anim;

    [Header("Attack Settings")]
    public Transform attackAreaTransform;
    public Vector2 attackAreaVector = new Vector2(1.5f, 1.5f);
    public float attackCooldown = 1f;
    protected float sinceAttack = 0f;

    protected virtual void Start()
    {
        health = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Ensure attackAreaTransform is assigned
        if (attackAreaTransform == null)
        {
            attackAreaTransform = this.transform;
            Debug.LogWarning("Attack Area Transform not assigned. Defaulting to enemy's transform.");
        }
    }

    protected virtual void Update()
    {
        sinceAttack += Time.deltaTime;
        if (objective == null) return; // Null check for objective

        distance = objective.position.x - transform.position.x;
        absoluteDistance = Mathf.Abs(distance);

        follow = absoluteDistance < range;

        if (follow)
        {
            Follow();
            Attack();
            Flip();
        }
        if (!follow)
        {
            anim.SetBool("running", false);
        }
    }

    protected virtual void Follow()
    {
        anim.SetBool("running", true);
        transform.position = Vector2.MoveTowards(transform.position, objective.position, speed * Time.deltaTime);
        
    }

    protected virtual void Flip()
    {
        Vector3 v3 = transform.localScale;

        // Flip only if necessary to avoid multiple unwanted flips
        if (objective.position.x > transform.position.x && v3.x > 0)
        {
            v3.x *= -1;  // Face right if objective is to the right
        }
        else if (objective.position.x < transform.position.x && v3.x < 0)
        {
            v3.x *= -1;  // Face left if objective is to the left
        }

        transform.localScale = v3;
    }


    public void EnemyHit(float _damage, Vector2 _direction)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            health -= _damage;
            anim.SetTrigger("hurt");

            Vector2 rebote = new Vector2(_direction.x, 0.2f).normalized;
            rb.AddForce(rebote * recoilForce, ForceMode2D.Impulse);

            if (health <= 0)
            {
                Die();
            }
            StartCoroutine(Invincible());
        }
    }

    protected IEnumerator Invincible()
    {
        yield return new WaitForSeconds(2);
        isInvincible = false;
    }

    protected void Die()
    {
        anim.SetTrigger("death");
        Destroy(gameObject, anim.GetCurrentAnimatorStateInfo(0).length);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
        
        Gizmos.color = Color.yellow;
        if (attackAreaTransform != null)
        {
            Gizmos.DrawWireCube(attackAreaTransform.position, attackAreaVector);
        }
    }

    protected virtual void Attack()
    {
        if (sinceAttack >= attackCooldown)
        {
            sinceAttack = 0f; // Reset cooldown timer
            if (absoluteDistance < attackAreaVector.x) // Check horizontal distance as basic range check
            {
                rb.velocity = Vector2.zero; // Stop movement for attack

                Collider2D[] objectsHit = Physics2D.OverlapBoxAll(attackAreaTransform.position, attackAreaVector, 0, LayerMask.GetMask("Player"));

                foreach (Collider2D obj in objectsHit)
                {
                    PlayerMovement player = obj.GetComponent<PlayerMovement>();
                    if (player != null)
                    {
                        Debug.Log(this + " hit the player");
                        anim.SetTrigger("attack");
                        
                        Vector2 _direction = (player.transform.position - transform.position).normalized;
                        Hit(_direction, player);
                    }
                }
            }
        }
    }

    protected virtual void Hit(Vector2 _attackDirection, PlayerMovement _player)
    {
        _player.Hurt(damagePower, _attackDirection);
    }
}
