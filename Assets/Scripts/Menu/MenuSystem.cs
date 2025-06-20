using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuSystem : MonoBehaviour
{
   public void Jugar(string NombreJuego)
    {
        Time.timeScale = 1.0f;  
        SceneManager.LoadScene(NombreJuego);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}
