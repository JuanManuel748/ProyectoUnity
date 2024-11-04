using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : Enemy
{
    // ATAQUES
    // ATAQUE SALTO
    // ATAQUE DASH
    // ATAQUE SPIN
    [Header("Jump")]
    public Transform jumpAreaTransform;
    public Vector2 jumpAreaVector = new Vector2(0.5f, 0.5f);
    public float jumpForce = 10f;
    public float jumpTime = 0.5f;
    public float jumpCoolDown = 2f;
    private float sinceJump = 0;
    private float gravity = 5;

    [Header("Dashing")]
    public Transform dashAreaTransform;
    public Vector2 dashAreaVector = new Vector2(1.5f, 1.5f);
    private float dashSpeed = 50f;
    private float dashTime = 0.1f;
    private float dashCooldown = 5f;
    private float sinceDash = 0f;


    protected override void Start()
    {
        base.Start();
        StartCoroutine(prueba());
    }

    protected override void Update()
    {
        base.Update();
        sinceJump += Time.deltaTime;
        if (sinceAttack >= attackCooldown)
        {
            Attack(); // Call the attack method if the cooldown time has expired
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // change color to green
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(jumpAreaTransform.position, jumpAreaVector);
        Gizmos.DrawWireCube(dashAreaTransform.position, dashAreaVector);
    }

    IEnumerator prueba()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Salto");
        JumpAttack();
        yield return new WaitForSeconds(5);
        Debug.Log("Dash");
        DashAttack();
        yield return new WaitForSeconds(5);
        Debug.Log("Normal");
        NormalAttack();
        yield return new WaitForSeconds(5);
    }



    private void JumpAttack()
    {

        anim.SetTrigger("jump");
        sinceJump = 0;
        rb.velocity = Vector2.zero;
        Vector2 direction = (objective.transform.position - transform.position).normalized;
        rb.AddForce(direction * jumpForce, ForceMode2D.Impulse);
    }

    private void DashAttack()
    {
        anim.SetTrigger("charge");
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

    private void NormalAttack()
    {
        anim.SetTrigger("attack");
    }

    private int Choose(float percentageA, float percentageB, float percentageC)
    {
        float totalPercentage = percentageA + percentageB + percentageC;
        float randomValue = Random.Range(0f, totalPercentage);

        if (randomValue < percentageA)
        {
            return 1; 
        }
        else if (randomValue < percentageA + percentageB)
        {
            return 2; 
        }
        else
        {
            return 3; 
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
                NormalAttack();
                break;
            case 2:
                JumpAttack();
                break;
            case 3:
                DashAttack();
                break;
            default:
                Debug.LogError("Invalid attack type: " + attackType);   
                break;
        }
    }


}
