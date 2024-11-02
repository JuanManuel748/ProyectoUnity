using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{/*
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 0; 
    }

    protected override void Update()
    {
        base.Update();
        Vector2 followPos = new Vector2(player.transform.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, followPos, speed * Time.deltaTime);
    }

    public override void EnemyHited(float damageDone)
    {
        base.EnemyHited(damageDone);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void Attack()
    {
        base.Attack();
    }
    /**/
}
