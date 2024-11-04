using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogEnemy : Enemy
{

    [Header("Dashing")]
    public Transform dashAreaTransform;
    public Vector2 dashAreaVector = new Vector2(1.5f, 1.5f);
    private float dashSpeed = 50f;
    private float dashTime = 0.1f;
    private float dashCooldown = 5f;
    private float sinceDash = 0f;
    private float gravity = 5;

    protected override void Start()
    {
        base.Start();
        
    }

    protected override void Update()
    {

        // Update the sinceDash timer
        sinceDash += Time.deltaTime;
        
        base.Update();
        // Choose an attack based on the probabilities
        if (sinceAttack >= attackCooldown)
        {
            Attack(); // Call the attack method if the cooldown time has expired
        }
    }

    protected override void Attack()
    {
        sinceAttack = 0f; // Reset attack cooldown timer
        int attackType = 0;

        // Asegúrate de que haya un jugador en el área de dash
        bool playerInDashArea = Physics2D.OverlapBoxAll(dashAreaTransform.position, dashAreaVector, 0, LayerMask.GetMask("Player")).Length > 0;
        bool playerInAttackArea = Physics2D.OverlapBoxAll(attackAreaTransform.position, attackAreaVector, 0, LayerMask.GetMask("Player")).Length > 0;

        if (playerInDashArea && sinceDash >= dashCooldown)
        {
            attackType = Choose(0, 0, 40);
        }
        else if (playerInAttackArea)
        {
            attackType = Choose(40, 40, 20); 
        }

        switch (attackType)
        {
            case 0:
                break;
            case 1:
                Attack1();
                break;
            case 2:
                Attack2();
                break;
            case 3:
                DashAttack();
                break;
            default:
                Debug.LogError("Invalid attack type: " + attackType);   
                break;
        }
    }

    private void Attack1()
    {
        anim.SetTrigger("attack1");
        rb.velocity = Vector2.zero;
        Vector2 direction = (objective.transform.position - transform.position).normalized;
        objective.GetComponent<PlayerMovement>().Hurt(damagePower, direction);
    }

    private void Attack2()
    {
        anim.SetTrigger("attack2");
        rb.velocity = Vector2.zero;
        Vector2 direction = (objective.transform.position - transform.position).normalized;
        objective.GetComponent<PlayerMovement>().Hurt(damagePower, direction);
    }

    private void DashAttack()
    {
        anim.SetTrigger("dash");
        StartCoroutine(Dash());
    }

    private IEnumerator Dash()
    {
        
        sinceDash = 0f; 
        rb.gravityScale = 0;
        Vector2 dashDirection = (objective.transform.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed; 
        yield return new WaitForSeconds(dashTime);
        rb.gravityScale = gravity; 
    }

    private int Choose(float percentageA, float percentageB, float percentageC)
    {
        float totalPercentage = percentageA + percentageB + percentageC;
        float randomValue = Random.Range(0f, totalPercentage);

        if (randomValue < percentageA)
        {
            return 1; // Select Attack 1
        }
        else if (randomValue < percentageA + percentageB)
        {
            return 2; // Select Attack 2
        }
        else
        {
            return 3; // Select Dash Attack
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.DrawWireCube(dashAreaTransform.position, dashAreaVector);
    }

    protected override void Follow()
    {
        base.Follow();
    }

}
