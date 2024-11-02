using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Objects")]
    public Rigidbody2D rb;

    [Header("Stats")]
    public float health;
    public float recoilLength;
    public float recoilFactor;
    public bool isRecoiling = false;

    


    private float recoilTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(health<= 0) {
            Destroy(gameObject);
        }

        if (isRecoiling) {
            if (recoilTimer < recoilLength) {
                recoilTimer += Time.deltaTime;
            } else {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    public void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitforce) {
    health -= _damageDone; 

    if (!isRecoiling) {
        rb.AddForce(-_hitforce * recoilFactor * _hitDirection.normalized, ForceMode2D.Impulse);
        isRecoiling = true; 
    }
}
}
