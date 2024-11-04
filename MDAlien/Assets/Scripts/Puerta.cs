using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Puerta : MonoBehaviour
{
    private bool jugadorEnRango = false;
    private Animator anim;
    private PlayerMovement playerMovement;

    [SerializeField] private string nextSceneName = "GameOver"; // Name of the next scene
    [SerializeField] private float doorOpenDelay = 1f; // Delay for the door animation

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (jugadorEnRango && Input.GetKeyDown(KeyCode.W) && playerMovement != null && playerMovement.tieneLlave)
        {
            StartCoroutine(AbrirPuerta());
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jugadorEnRango = false;
            playerMovement = null;
        }
    }

    private IEnumerator AbrirPuerta()
    {
        anim.SetTrigger("Open");

        yield return new WaitForSeconds(doorOpenDelay);

        SceneManager.LoadScene(nextSceneName);
    }
}
