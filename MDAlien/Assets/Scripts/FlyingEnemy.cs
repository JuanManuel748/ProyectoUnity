using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    [Header("Hover Settings")]
    public float hoverAmplitude = 0.5f; // La altura del movimiento flotante
    public float hoverFrequency = 2f; // Velocidad del movimiento flotante

    private float hoverOffset;
    private Vector3 startPosition;

    protected override void Start()
    {
        base.Start();
        startPosition = transform.position; // Almacena la posición inicial para el efecto de flotación
    }

    protected override void Update()
    {
        base.Update();
        // Aplica el efecto de flotación
        if (!follow)
        {
            
            Hover();
        }
    }

    private void Hover()
    {
        // Calcula un desplazamiento vertical de flotación basado en una onda sinusoidal
        hoverOffset = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;

        // Aplica el efecto de flotación a la posición Y mientras sigue la posición del jugador en los ejes X y Y
        if (objective != null && follow)
        {
            // Sigue la posición del objetivo en los ejes X y Y, pero agrega el desplazamiento de flotación a Y
            transform.position = Vector2.MoveTowards(transform.position, 
                new Vector2(objective.position.x, objective.position.y + hoverOffset), speed * Time.deltaTime);
        }
        else
        {
            // Si no está siguiendo, flota en el lugar alrededor de la posición inicial
            transform.position = Vector2.MoveTowards(transform.position, startPosition, speed * Time.deltaTime);
            // Esperar hasta llegar a la ubicación inicial
            if ((Vector2.Distance(transform.position, startPosition) < 1f))
            {
                transform.position = new Vector3(startPosition.x, startPosition.y + hoverOffset, transform.position.z);
            }
        }
    }

    protected override void Follow()
    {
        transform.position = Vector2.MoveTowards(transform.position, objective.position, speed * Time.deltaTime);
        transform.localScale = new Vector3(absoluteDistanceX < 0 ? 1 : -1, 1, 1); // Flip sprite direction
    }
}
