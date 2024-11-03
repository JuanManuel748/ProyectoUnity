using UnityEngine;
using UnityEngine.SceneManagement;

public class Puerta : MonoBehaviour
{
    public bool tieneLlave = false;
    private bool jugadorEnRango = false;

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.W) && tieneLlave)
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
        // Aquí puedes agregar animación de abrir puerta si es necesario
        SceneManager.LoadScene("GameOver");
    }
}