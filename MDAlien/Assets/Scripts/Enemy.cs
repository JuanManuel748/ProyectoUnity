using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
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
        } 
    }

    public virtual void EnemyHited(float _damageDone) {
        health = Mathf.Max(health - _damageDone, 0);
    }


}
