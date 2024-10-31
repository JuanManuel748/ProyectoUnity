using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Patrolling")]
    public GameObject patrolLeftPoint;
    public GameObject patrolRightPoint;
    public float patrolSpeed = 2f;
    private Rigidbody2D rb;
    private Transform patrolTargetPoint;

    // Start is called before the first frame update
    void Start()
    {
        patrolTargetPoint = patrolRightPoint.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (patrolTargetPoint == patrolRightPoint.transform) {
            rb.velocity = new Vector2(patrolSpeed, 0);
        } else {
            rb.velocity = new Vector2(-patrolSpeed, 0);
        }
        if (Vector2.Distance(transform.position, patrolTargetPoint.position) < 0.5f) {
            if (patrolTargetPoint == patrolRightPoint.transform) {
                patrolTargetPoint = patrolLeftPoint.transform;
                Flip();
            } else {
                patrolTargetPoint = patrolRightPoint.transform;
                Flip();
            }
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(patrolLeftPoint.transform.position, 0.5f);
        Gizmos.DrawWireSphere(patrolRightPoint.transform.position, 0.5f);
        Gizmos.DrawLine(patrolLeftPoint.transform.position, patrolRightPoint.transform.position);
    }
}