using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    private bool jugadorEnRango;
    private Animator anim;
    private PlayerMovement playerMovement;

    void Start()
    {
        anim = GetComponent<Animator>();
        jugadorEnRango = false;
    }

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.W) && playerMovement != null && playerMovement.tieneLlave)
        {
            AbrirPuerta();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = true;
            playerMovement = collision.GetComponent<PlayerMovement>();
        }
    }


    protected void AbrirPuerta()
    {
        anim.SetTrigger("Open");
        // Aquí puedes agregar animación de abrir puerta si es necesario
        SceneManager.LoadScene("GameOver");
    }
}