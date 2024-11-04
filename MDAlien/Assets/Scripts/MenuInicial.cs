using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuInicial : MonoBehaviour
{
    public void IniciarJuego()
    {
        SceneManager.LoadScene("Lvl_1"); 
    }
    public void SalirJuego()
    {
        Debug.Log("Salir del juego");
        Application.Quit();
    }
}
