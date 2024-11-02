using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{/*
    [Header("Objects")]
    public Rigidbody2D rb;
    public PlayerMovement player;

    [Header("Stats")]
    public float health;
    public float speed;
    public float attackPower;

    protected virtual void Start()
    {
        rb.gravityScale = 0; 
    }

    protected virtual void Awake() {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();

        if (rb == null) {
            Debug.LogError("Rigidbody2D no encontrado en " + gameObject.name);
        }

        if (player == null) {
            Debug.LogError("PlayerMovement no encontrado en el objeto Player");
        }
    }

    protected virtual void Update() {
        if (health <= 0) {
            Destroy(gameObject);
        } else {
            MoveTowardsPlayer();
        }
    }

    public virtual void EnemyHited(float _damageDone) {
        health = Mathf.Max(health - _damageDone, 0);
    }

    protected virtual void OnTriggerEnter2D(Collider2D _other) {
        if (_other.CompareTag("Player") && !player.invincible) {
            Attack();
        }
    }

    protected virtual void Attack() {
        player.PlayerHited(attackPower);
    }

    protected virtual void MoveTowardsPlayer() {
        if (player != null) {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            rb.velocity = direction * speed;
        }
    }
/**/
}
