using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    public bool tieneLlave;
    private bool jugadorEnRango;
    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        tieneLlave = false;
        jugadorEnRango = false;
    }

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.E) && tieneLlave)
        {
            AbrirPuerta();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = false;
        }
    }

    private void AbrirPuerta()
    {
        anim.SetTrigger("open");
        // Aquí puedes agregar animación de abrir puerta si es necesario
        SceneManager.LoadScene("Game Over");
    }
}