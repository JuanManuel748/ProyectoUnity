using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void ReiniciarJuego()
    {
        SceneManager.LoadScene("SampleScene"); 
    }
    public void SalirJuego()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}
