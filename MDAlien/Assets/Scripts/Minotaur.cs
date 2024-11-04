using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minotaur : Enemy
{
    [Header("Jump")]
    public Transform jumpAreaTransform;
    public Vector2 jumpAreaVector = new Vector2(0.5f, 0.5f);
    public float jumpForce = 10f;
    public float jumpCoolDown = 2f;
    private float sinceJump = 0;

    [Header("Dashing")]
    public Transform dashAreaTransform;
    public Vector2 dashAreaVector = new Vector2(1.5f, 1.5f);
    private float dashSpeed = 50f;
    private float dashTime = 0.4f;
    private float dashCooldown = 5f;
    private float sinceDash = 0f;

    private bool playerInAttackArea, playerInDashArea, isAttacking, isDashing;
    private float originalGravityScale;

    protected override void Start()
    {
        base.Start();
        originalGravityScale = rb.gravityScale;  // Guardamos la gravedad original
        isDashing = false;
        isAttacking = false;
    }

    protected override void Update()
    {
        sinceAttack += Time.deltaTime;
        if (objective == null) return; // Null check for objective

        absoluteDistanceX = Mathf.Abs(objective.position.x - transform.position.x);
        absoluteDistanceY = Mathf.Abs(objective.position.y - transform.position.y);

        follow = (absoluteDistanceX < rangeX) && (absoluteDistanceY < rangeY);
        
        

        if (follow && !isDashing)
        {
            Follow();
            Flip();
            
        }
        if (!follow)
        {
            anim.SetBool("running", false);
        }
        if (sinceAttack >= attackCooldown && !isAttacking)
            {
                Attack();
            }
            
        
        sinceJump += Time.deltaTime;

        // Actualizar áreas de ataque
        playerInDashArea = Physics2D.OverlapBoxAll(dashAreaTransform.position, dashAreaVector, 0, LayerMask.GetMask("Player")).Length > 0;
        playerInAttackArea = Physics2D.OverlapBoxAll(attackAreaTransform.position, attackAreaVector, 0, LayerMask.GetMask("Player")).Length > 0;

        // Verificar si se puede atacar
        if (sinceAttack >= attackCooldown && !isAttacking)
        {
            Attack(); // Llamar al método Attack si no está atacando
        }
    }


    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(jumpAreaTransform.position, jumpAreaVector);
        Gizmos.DrawWireCube(dashAreaTransform.position, dashAreaVector);
    }

    protected override void Flip()
    {
        Vector3 v3 = transform.localScale;

        if (objective.position.x > transform.position.x && v3.x < 0)
        {
            v3.x *= -1;  // Mirar hacia la derecha
        }
        else if (objective.position.x < transform.position.x && v3.x > 0)
        {
            v3.x *= -1;  // Mirar hacia la izquierda
        }

        transform.localScale = v3;
    }

    private void JumpAttack()
    {
        if (sinceJump >= jumpCoolDown && !isAttacking)
        {
            StartCoroutine(jump());
        }       
    }

    private IEnumerator jump()
    {
        isAttacking = true; // Marcar como atacando
        sinceJump = 0;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);

        // Esperar hasta que la velocidad en Y sea menor a 0 (comienza a caer)
        yield return new WaitUntil(() => rb.velocity.y < 0);
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => Physics2D.OverlapBoxAll(jumpAreaTransform.position, jumpAreaVector, 0, LayerMask.GetMask("Ground")).Length > 0);
        anim.SetTrigger("jump");

        // Chequear si hay algún objeto del jugador en el área de ataque
        if (Physics2D.OverlapBoxAll(jumpAreaTransform.position, jumpAreaVector, 0, LayerMask.GetMask("Player")).Length > 0) {
            Vector2 _direction = (objective.transform.position - transform.position).normalized;
            objective.GetComponent<PlayerMovement>().Hurt(damagePower, _direction);
        }
        
        StartCoroutine(attackCooldownIE());
    }


    private void DashAttack()
    {
        if (playerInDashArea && !isAttacking)
        {
            isAttacking = true; // Marcar como atacando
            isDashing = true;
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        anim.SetTrigger("charge");
        speed = 0; // Detener la velocidad para preparar el dash

        yield return new WaitForSeconds(0.5f);
        speed = 8f;
        rb.gravityScale = originalGravityScale * 0.2f; // Reducir la gravedad para el dash
        Vector2 dashDirection = (objective.transform.position - transform.position).normalized;
        rb.velocity = dashDirection * dashSpeed;

        yield return new WaitForSeconds(dashTime);

        if (Physics2D.OverlapBoxAll(attackAreaTransform.position, attackAreaVector, 0, LayerMask.GetMask("Player")).Length > 0) {
            Vector2 _direction = (objective.transform.position - transform.position).normalized;
            objective.GetComponent<PlayerMovement>().Hurt(damagePower, _direction);
        }

        rb.gravityScale = originalGravityScale; // Restaurar gravedad al valor original
        StartCoroutine(attackCooldownIE());
    }

    private void NormalAttack()
    {
        
            isAttacking = true; // Marcar como atacando
            anim.SetTrigger("spin");

            if (Physics2D.OverlapBoxAll(attackAreaTransform.position, attackAreaVector, 0, LayerMask.GetMask("Player")).Length > 0) {
                Vector2 _direction = (objective.transform.position - transform.position).normalized;
                objective.GetComponent<PlayerMovement>().Hurt(damagePower, _direction);
            }
            StartCoroutine(attackCooldownIE());
        
    }

    private IEnumerator attackCooldownIE()
    {
        yield return new WaitForSeconds(1f); // Esperar antes de permitir otro ataque
        isDashing = false;
        yield return new WaitForSeconds(2f); // Esperar antes de permitir otro ataque
        isAttacking = false; // Permitir nuevos ataques
        sinceAttack = 0f; // Reiniciar el contador de ataque
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
        int attackType = Choose(0, 100, 0); // Elegir tipo de ataque

        switch (attackType)
        {
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