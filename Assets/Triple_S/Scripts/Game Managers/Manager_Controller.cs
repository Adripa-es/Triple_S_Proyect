using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;

// Clase Manager_Controller: gestiona todos los aspectos del juego
public class Manager_Controller : MonoBehaviour
{
    // Singleton de la instancia del administrador
    public static Manager_Controller instance;

    // Variables para configurar los modos de juego
    [Header("Modos de juego")]
    private bool modoSupervivencia;

    // Atributos para configurar las variables de juego
    [Header("Atributos del juego")]
    public int totalHealth; // Salud total del jugador
    public int maxHealth; // Salud máxima que puede tener el jugador
    private int barLength; // Longitud de la barra de salud
    private int totalCoins; // Monedas totales recogidas
    private int points; // Puntos totales
    private float timer; // Cronómetro del juego
    private int pointsMultiplier; // Multiplicador de puntos

    // Variables de control de juego
    public bool EndGame; // Controla el estado de final de juego
    private float timeSinceLastPoint; // Tiempo transcurrido desde el último punto
    private float pointInterval; // Intervalo para agregar puntos

    // Variable para manejar el estado de pausa
    public bool isPaused;

    // Variables para los componentes del Canvas
    [Header("Canvas")]
    private Canvas playUI; // Canvas de la interfaz de juego
    private Canvas pauseMenu; // Canvas del menú de pausa

    // Variables para los marcadores de juego
    [Header("Marcadores")]
    private string dataStats = "PlayerStats.txt"; // Ruta al archivo de estadísticas del juego
    private TextMeshProUGUI valueHealth; // Componente de texto para la salud
    private TextMeshProUGUI valueTime; // Componente de texto para el tiempo
    private TextMeshProUGUI valuePoints; // Componente de texto para los puntos
    private TextMeshProUGUI valueCoins; // Componente de texto para las monedas

    // Variables para los botones
    [Header("Botones")]
    private Button restartButton; // Botón de reinicio
    private Button resumeButton; // Botón de reanudar

    // Awake se llama al iniciar el script
    // Aquí se realiza la configuración inicial del Manager_Controller
    private void Awake()
    {
        // Verifica si ya existe una instancia del Manager_Controller
        if (Manager_Controller.instance == null)
        {
            // Establece esta instancia como la instancia única y evita que se destruya al cargar nuevas escenas
            Manager_Controller.instance = this;
            DontDestroyOnLoad(gameObject);

            // Suscribe el método OnSceneLoaded al evento sceneLoaded
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            // Si ya existe una instancia, destruye este objeto para mantener la unicidad del Manager_Controller
            Destroy(gameObject);
        }
    }

    // Start se llama antes del primer frame update
    private void Start()
    {
        // Inicializa los valores del juego al inicio
        InitializeGame();
    }

    // Update se llama una vez por frame
    private void Update()
    {
        // Si el juego no ha terminado, verifica si se presionó la tecla Q para pausar el juego
        // y actualiza el tiempo
        if (!EndGame)
        {
            if (Input.GetKeyDown(KeyCode.Q))
            {
                TogglePause();
            }

            setTime();
        }
    }

    // Inicializa las variables del juego
    public void InitializeGame()
    {
        // Asegura que el juego continúe después de un reinicio
        Time.timeScale = 1f;

        // Inicializa las variables de juego
        totalHealth = 10;
        maxHealth = 10;
        barLength = 20;

        totalCoins = 0;
        points = 0;
        timer = 0;

        pointsMultiplier = 1;
        EndGame = false;

        isPaused = false;
        modoSupervivencia = true;

        timeSinceLastPoint = 0f;
        pointInterval = 1f;

        // Si la escena actual es Scene_Game, recarga la escena
        if (SceneManager.GetActiveScene().name == "Scene_Game")
        {
            SceneManager.LoadScene("Scene_Game");
        }
    }

    // Se llama cada vez que una nueva escena se carga completamente
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Si la escena cargada es Scene_Game, inicializa los componentes de la UI
        if (scene.name == "Scene_Game")
        {
            InitializeUIComponents();
        }
    }

    // Inicializa los componentes de la UI
    private void InitializeUIComponents()
    {
        // Busca y almacena los componentes de la UI necesarios
        playUI = GameObject.Find("Play")?.GetComponent<Canvas>();
        pauseMenu = GameObject.Find("Pause")?.GetComponent<Canvas>();
        valueHealth = GameObject.Find("valueHealth")?.GetComponent<TextMeshProUGUI>();
        valueTime = GameObject.Find("valueTime")?.GetComponent<TextMeshProUGUI>();
        valuePoints = GameObject.Find("valuePoints")?.GetComponent<TextMeshProUGUI>();
        valueCoins = GameObject.Find("valueCoins")?.GetComponent<TextMeshProUGUI>();
        restartButton = GameObject.Find("Btn_Restart")?.GetComponent<Button>();
        resumeButton = GameObject.Find("Btn_Resume")?.GetComponent<Button>();

        // Configura los valores iniciales de la UI
        pauseMenu.enabled = false;
        valueHealth.text = GenerateHealthBar(totalHealth, maxHealth, barLength);
        valueTime.text = "" + timer;
        valuePoints.text = "" + points;
        valueCoins.text = "" + totalCoins;
    }

    // OnDestroy se llama al destruir el objeto
    // Aquí se anula la suscripción al evento sceneLoaded
    private void OnDestroy()
    {
        // Si esta instancia es la instancia única, desuscribe el método OnSceneLoaded del evento sceneLoaded
        if (Manager_Controller.instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    // Cambia el estado de pausa del juego
    public void TogglePause()
    {
        // Alterna el estado de pausa
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Detiene el tiempo del juego

            playUI.enabled = false; // Desactiva el canvas de la interfaz del jugador

            pauseMenu.enabled = true; // Activa el canvas del menú de pausa

            Cursor.lockState = CursorLockMode.None; // Desbloquea el cursor
            Cursor.visible = true; // Muestra el cursor
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el tiempo del juego

            playUI.enabled = true; // Activa el canvas de la interfaz del jugador
            pauseMenu.enabled = false; // Desactiva el canvas del menú de pausa

            Cursor.lockState = CursorLockMode.Locked; // Bloquea el cursor
            Cursor.visible = false; // Oculta el cursor
        }

        //si se acabó el juego, se muestra el menú de pausa y se oculta la UI del jugador
        //y se para la musica
        if (EndGame)
        {
            restartButton.gameObject.SetActive(true);
            resumeButton.gameObject.SetActive(false);

            // Guardar las estadísticas del juego en un archivo de texto cuando el juego termine
            string statsFilePath = Path.Combine(Application.dataPath, dataStats);
            using (StreamWriter sw = new StreamWriter(statsFilePath, true))
            {
                sw.WriteLine(System.DateTime.Now.ToShortDateString() + "||" + FormatTime(timer) + "||" + points + "||" + totalCoins);
            }
        }
        //y sino, entonces no se muestra y se muestra la UI del jugador
        else
        {
            restartButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(true);
        }

    }

    // Ajusta la cantidad de vidas y actualiza la barra de vida en la UI
    public void setVidas(int v)
    {
        // El jugador se cura
        if (v > 0)
        {
            totalHealth = Mathf.Min(totalHealth + v, maxHealth); // Limitar la curación al máximo de vida
        }

        // El jugador recibe daño
        else if (v < 0)
        {
            totalHealth += v;
            SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.ReceiveDamage);


            if ((totalHealth <= 0) && !EndGame)
            {
                SFX_Controller.instance.StopSound(SFX_Controller.SoundType.OST1);

                totalHealth = 0;
                EndGame = true;
                SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.Death);
            }
        }

        valueHealth.text = GenerateHealthBar(totalHealth, maxHealth, barLength);
    }


    // Ajusta la cantidad total de monedas y actualiza el texto de monedas en la UI
    public void setTotalCoins(int m)
    {
        totalCoins += m;
        valueCoins.text = "" + totalCoins;

        //cada vez que se coge una moneda se suman puntos
        setPoints(m);
    }

    // Ajusta los puntos y actualiza el texto de puntos en la UI
    public void setPoints(int p)
    {
        points += p * pointsMultiplier;
        valuePoints.text = "" + points;
    }

    // Ajusta el tiempo y actualiza el texto del tiempo en la UI
    public void setTime()
    {
        if (modoSupervivencia)
        {
            timer += Time.deltaTime;
        }

        valueTime.text = "" + FormatTime(timer);

        // Sumar puntos con el paso del tiempo (1 punto por segundo)
        timeSinceLastPoint += Time.deltaTime;
        if (timeSinceLastPoint >= pointInterval)
        {
            setPoints(1);
            timeSinceLastPoint = 0f;
        }
    }

    // Formatea el tiempo para ser mostrado en la UI
    private string FormatTime(float timeInSeconds)
    {
        int hours = (int)(timeInSeconds / 3600);
        int minutes = (int)((timeInSeconds % 3600) / 60);
        float seconds = timeInSeconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:00.000}", hours, minutes, seconds);
    }

    // Genera una representación visual de la barra de vida
    private string GenerateHealthBar(int currentHealth, int maxHealth, int barLength)
    {
        // Asegurarse de que la longitud de la barra de salud nunca sea menor que cero
        barLength = Mathf.Max(0, barLength);

        int filledLength = Mathf.Max(0, Mathf.RoundToInt((float)currentHealth / maxHealth * barLength));
        float healthPercentage = (float)currentHealth / maxHealth;

        string healthColor;

        if (healthPercentage > 0.5f) // Salud mayor al 50%, color verde
        {
            healthColor = "<color=#00FF00>";
        }
        else if (healthPercentage > 0.25f) // Salud entre 25% y 50%, color amarillo
        {
            healthColor = "<color=#FFFF00>";
        }
        else // Salud menor o igual al 25%, color rojo
        {
            healthColor = "<color=#FF0000>";
        }

        int emptyLength = Mathf.Max(0, barLength - filledLength);
        string healthBar = healthColor + new string('█', filledLength) + "</color>" + new string('░', emptyLength);
        return healthBar;
    }


}