using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// La clase "Spawner" es responsable de generar diferentes objetos en el juego.
public class Spawner_Controller : MonoBehaviour
{
    // Referencias a objetos en el editor
    [Header("Suelo")]
    [SerializeField] private GameObject floor;

    [Header("Atributos Time")]
    [SerializeField] private float tiempoEsperaSpawn;  // tiempo de espera antes de spawnear

    [Header("GameObjects to Spawn")]
    [SerializeField] private GameObject spawn;  // objeto a spawnear
    public GameObject bala;  // referencia a la bala lineal
    [SerializeField] private GameObject balaPerseguidora;  // referencia a la bala perseguidora
    [SerializeField] private GameObject spawnBalas;  // objeto donde se spawnearán las balas

    // Listas para mantener diferentes objetos en el juego
    [SerializeField] private List<GameObject> listaMonedas = new List<GameObject>();
    [SerializeField] private List<GameObject> turretsList = new List<GameObject>();
    private List<GameObject> spawnTorretList = new List<GameObject>();
    [SerializeField] private List<GameObject> vidaList = new List<GameObject>();
    [SerializeField] private List<GameObject> trampasList = new List<GameObject>();
    private List<GameObject> listaMunicion = new List<GameObject>();
    private List<GameObject> listaBalasPerseguidoras = new List<GameObject>();

    [Header("Probabilidades de Spawn")]
    // Probabilidades de spawnear ciertos objetos
    [Range(0f, 1f)] public float probabilityCoins;
    [Range(0f, 1f)] public float probabilityHealth;
    [Range(0f, 1f)] public float probabilityTramps;

    [Header("Atributos Items")]
    // Variables para controlar la generación de objetos
    private Vector3 centro;
    private float dimensionX;
    private float dimensionZ;
    private bool generarItems = true;
    private int indiceListaBala = 0;

    void Start()
    {
        // Agregar todos los hijos de 'spawn' a 'spawnTorretList'
        for (int i = 0; i < spawn.transform.childCount; i++)
        {
            spawnTorretList.Add(spawn.transform.GetChild(i).gameObject);
        }

        // Instanciar nuevas balas y agregarlas a 'listaMunicion' y 'listaBalasPerseguidoras'
        for (int i = 0; i < 60; i++)
        {
            GameObject auxBala = Instantiate(bala, spawnBalas.transform.position, Quaternion.identity, transform);
            listaMunicion.Add(auxBala);

            GameObject auxBalaPerseguidora = Instantiate(balaPerseguidora, spawnBalas.transform.position, Quaternion.identity, transform);
            listaBalasPerseguidoras.Add(auxBalaPerseguidora);
        }

        StartCoroutine(SpawnTorret());
        ObtenerDimensiones();
    }

    // Método para asignar balas a las torretas que lo requieran
    public GameObject PedirBalas()
    {
        GameObject balaElegida = listaMunicion[indiceListaBala];
        indiceListaBala = (indiceListaBala + 1) % listaMunicion.Count;  // garantizar que el índice siempre esté en rango
        return balaElegida;
    }

    public GameObject PedirBalasPerseguidoras()
    {
        GameObject balaPerseguidoraElegida = listaBalasPerseguidoras[indiceListaBala];
        indiceListaBala = (indiceListaBala + 1) % listaBalasPerseguidoras.Count;  // garantizar que el índice siempre esté en rango
        return balaPerseguidoraElegida;
    }

    void Update()
    {
        // Solo continuar si se puede generar items
        if (!generarItems) return;

        generarItems = false;
        StartCoroutine(TimeSpawnItems());
    }

    // Coroutine para generar torretas
    IEnumerator SpawnTorret()
    {
        int cantidadSpawn = spawnTorretList.Count;
        for (int i = 0; i < cantidadSpawn; i++)
        {
            yield return new WaitForSeconds(5f);
            int random = Random.Range(0, turretsList.Count);
            int randomSpawn = Random.Range(0, spawnTorretList.Count);

            Instantiate(turretsList[random], spawnTorretList[randomSpawn].transform.position, Quaternion.identity);
            spawnTorretList.RemoveAt(randomSpawn); // Descartar la torreta que se ha generado de la lista
        }
    }

    // Coroutine para generar items en intervalos
    IEnumerator TimeSpawnItems()
    {
        yield return new WaitForSeconds(tiempoEsperaSpawn);
        SpawnItems();
    }

    // Obtener las dimensiones del suelo
    void ObtenerDimensiones()
    {
        Renderer objetoRenderer = floor.GetComponent<Renderer>();
        Bounds objetoBounds = objetoRenderer.bounds;

        dimensionX = objetoBounds.size.x;
        dimensionZ = objetoBounds.size.z;
        centro = objetoBounds.center;
    }

    // Generar items aleatoriamente
    void SpawnItems()
    {
        float x = Random.Range(centro.x - dimensionX / 2f, centro.x + dimensionX / 2f);
        float z = Random.Range(centro.z - dimensionZ / 2f, centro.z + dimensionZ / 2f);

        Vector3 coordenadasAleatorias = new Vector3(x, 2.5f, z);
        SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.Spawn);

        float random = Random.Range(0f, 1f);
        if (random <= probabilityCoins)
        {
            int randomMoneda = Random.Range(0, listaMonedas.Count);
            Instantiate(listaMonedas[randomMoneda], coordenadasAleatorias, Quaternion.identity);
        }
        else if (random <= probabilityCoins + probabilityHealth)
        {
            int randomVida = Random.Range(0, vidaList.Count);
            Instantiate(vidaList[randomVida], coordenadasAleatorias, Quaternion.identity);
        }
        else if (random <= probabilityCoins + probabilityTramps)
        {
            int randomTrampa = Random.Range(0, trampasList.Count);
            Instantiate(trampasList[randomTrampa], coordenadasAleatorias, Quaternion.identity);
        }

        generarItems = true;  // habilitar la generación de items para el próximo ciclo
    }
}
