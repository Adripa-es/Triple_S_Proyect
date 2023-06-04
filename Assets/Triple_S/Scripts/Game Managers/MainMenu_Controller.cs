using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class MainMenu_Controller : MonoBehaviour
{
    // Declaracion de los canvas del menú
    private Canvas exitWindow; // Ventana emergente de salida
    private Canvas settingsWindow; // Ventana de configuración
    private Canvas scoreBoards; // Ventana de puntuaciones

    // Declaración de botones
    private Button btnExit; // Botón de salida
    private Button btnYes; // Botón "Sí" para confirmar salida
    private Button btnNo; // Botón "No" para cancelar salida

    private void Start()
    {
        FindItems(); // Buscar los elementos necesarios al inicio
    }

    private void FindItems()
    {
        // Encontrar los objetos necesarios en el escenario
        exitWindow = GameObject.Find("ExitWindow")?.GetComponent<Canvas>(); // Buscar la ventana emergente de salida
        settingsWindow = GameObject.Find("SettingsMenu")?.GetComponent<Canvas>(); // Buscar la ventana de configuración
        scoreBoards = GameObject.Find("ScoreBoards")?.GetComponent<Canvas>(); // Buscar la ventana de puntuaciones

        btnExit = GameObject.Find("Btn_Exit")?.GetComponent<Button>(); // Buscar el botón de salida
        btnYes = GameObject.Find("Btn_Yes")?.GetComponent<Button>(); // Buscar el botón "Sí"
        btnNo = GameObject.Find("Btn_No")?.GetComponent<Button>(); // Buscar el botón "No"
    }

    public void GoToPlayScene()
    {

        if (Manager_Controller.instance == null)
        {
            GameObject managerControllerObject = new GameObject("Manager_Controller");
            managerControllerObject.AddComponent<Manager_Controller>();
        }

        // Asegurar que el cursor esté oculto y bloqueado antes de cambiar a la siguiente escena
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        Manager_Controller.instance.InitializeGame();

        SceneManager.LoadScene("Scene_Game"); // Cambiar a la escena de juego
    }

    public void GoToMainMenu()
    {

        // Para evitar que se quede el menú de salida abierto
        if (exitWindow.enabled)
        {
            exitWindow.enabled = false;
        }
        Time.timeScale = 1f; // Restablecer la escala de tiempo a 1

        // Destruir la instancia de Manager_Controller
        if (Manager_Controller.instance != null)
        {
            Destroy(Manager_Controller.instance.gameObject);
        }

        SceneManager.LoadScene("Scene_Principal_Menu"); // Cambiar a la escena del menú principal
    }

    public void GoToSettings()
    {

        if (settingsWindow != null)
        {
            if (settingsWindow.enabled)
            {
                settingsWindow.enabled = false;

                if (scoreBoards != null)
                {
                    scoreBoards.enabled = true;
                }
            }
            else
            {
                settingsWindow.enabled = true;

                if (exitWindow != null)
                {
                    exitWindow.enabled = false;
                }

                if (scoreBoards != null)
                {
                    scoreBoards.enabled = false;
                }
            }
        }
    }

    public void ShowExitWindow()
    {

        if (exitWindow != null)
        {
            if (exitWindow.enabled)
            {
                exitWindow.enabled = false;

                if (scoreBoards != null)
                {
                    scoreBoards.enabled = true;
                }
            }
            else
            {
                exitWindow.enabled = true;

                if (settingsWindow != null)
                {
                    settingsWindow.enabled = false;
                }

                if (scoreBoards != null)
                {
                    scoreBoards.enabled = false;
                }
            }
        }
    }

    public void ShowHideScoreBoards()
    {
        if (scoreBoards != null)
        {
            if (scoreBoards.enabled)
            {
                scoreBoards.enabled = false;
                scoreBoards.enabled = true;
            }
            else
            {
                scoreBoards.enabled = true;
                exitWindow.enabled = false;
                settingsWindow.enabled = false;
            }
        }
    }

    public void Exit()
    {
        Application.Quit(); // Salir de la aplicación
    }

    public void Resume()
    {
        // Para evitar que se quede el menú de salida abierto
        if (exitWindow.enabled)
        {
            exitWindow.enabled = false;
        }
        // Para evitar que el menú de opciones siga abierto
        if (settingsWindow.enabled)
        {
            settingsWindow.enabled = false;
        }
        Manager_Controller.instance.TogglePause();
    }



}
