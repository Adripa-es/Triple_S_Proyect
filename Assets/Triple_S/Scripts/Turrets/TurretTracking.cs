using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretTracking : MonoBehaviour
{
    [Header("Scriptable Object")]
    public Scriptable_Turret turretData; // Objeto de datos para la torreta

    [Header("Player")]
    private GameObject player; // Referencia al jugador

    [Header("Shooting Control")]
    public Transform[] spawnerBullets; // Array de puntos de generación de balas
    public ParticleSystem[] fireFXs; // Array de sistemas de partículas para los efectos de disparo
    [SerializeField] private float trackingBulletProbability; // Probabilidad de disparar una bala rastreadora. Se puede ajustar desde el inspector de Unity


    // Variables privadas para el control de disparo
    private Transform bulletSpawnPoint;
    private int bulletIndex;
    private IEnumerator shootingCoroutine;
    public GameObject pemEffect; // Efecto visual para cuando se aplica el PEM

    [Header("Turret Parts")]
    [SerializeField] private Transform barrelTransform; // Transformación del cañón de la torreta
    [SerializeField] private Transform bodyTransform; // Transformación del cuerpo de la torreta
    private Quaternion initialBarrelRotation; // Almacena la rotación inicial del cañón

    [Header("Map")]
    private GameObject map; // Referencia al mapa
    private Spawner_Controller spawnerScript; // Script del generador de balas
    private Vector3 lastPlayerPosition = Vector3.zero; // Almacena la última posición conocida del jugador

    // Propiedad para saber si el PEM está activo
    private bool PemActive { get; set; }

    // Método Awake se ejecuta cuando se crea la instancia del script
    private void Awake()
    {
        // Buscar al jugador y el mapa por etiqueta
        player = GameObject.FindGameObjectWithTag("Player");
        map = GameObject.FindGameObjectWithTag("Mapa");

        // Obtener el componente Spawner del mapa
        spawnerScript = map.GetComponent<Spawner_Controller>();
        // Inicializar la variable de índice de bala
        bulletIndex = 0;

        // Desactivar inicialmente el efecto PEM
        pemEffect.SetActive(false);

        // Inicializar la corrutina de disparo
        shootingCoroutine = ShootCoroutine();
    }

    // Método Start se ejecuta antes del primer frame del juego
    private void Start()
    {
        // Rotar la torreta hacia el jugador al inicio
        RotateTurretTowardsPlayer();
        // Almacenar la rotación inicial del cañón
        initialBarrelRotation = barrelTransform.localRotation;

        // Iniciar la corrutina de disparo
        StartCoroutine(shootingCoroutine);
    }

    // Método Update se ejecuta en cada frame del juego
    private void Update()
    {
        // Si PEM no está activo, la torreta debería seguir al jugador
        if (!PemActive)
        {
            RotateTurretTowardsPlayer();
        }
    }

    // Método para rotar la torreta hacia la posición del jugador
    private void RotateTurretTowardsPlayer()
    {
        // Obtener la posición del jugador si existe, de lo contrario usar el Vector3 zero
        Vector3 playerPosition = player ? player.transform.position : Vector3.zero;
        // Si la posición del jugador ha cambiado desde la última actualización
        if (playerPosition != lastPlayerPosition)
        {
            // Almacenar la nueva posición del jugador
            lastPlayerPosition = playerPosition;
            // Ajustar la posición del jugador a la misma altura que la torreta
            playerPosition.y = bodyTransform.position.y;
            // Hacer que la torreta mire al jugador
            bodyTransform.LookAt(playerPosition, Vector3.up);
        }
    }

    // Corrutina para controlar los disparos de la torreta
    private IEnumerator ShootCoroutine()
    {
        // Tiempo para el próximo disparo
        float nextShootTime = 0f;

        // Bucle infinito para controlar los disparos
        while (true)
        {
            // Si ha pasado el tiempo para el próximo disparo y PEM no está activo
            if (Time.time >= nextShootTime && !PemActive)
            {
                // Decidir si la próxima bala será una bala rastreadora
                bool isTrackingBullet = Random.value <= trackingBulletProbability; 

                // Rotar el cañón en preparación para el disparo
                yield return RotateBarrel(isTrackingBullet ? -45f : 0f);

                // Determinar el número de balas a disparar
                int bulletsToShoot = isTrackingBullet ? turretData.AmountTrackingBullets : 1;
                for (int i = 0; i < bulletsToShoot; i++)
                {
                    // Disparar la bala
                    yield return ShootBullet(isTrackingBullet);
                }

                // Calcular el tiempo para el próximo disparo
                nextShootTime = Time.time + turretData.shootSpeed;
            }
            else
            {
                // Si no es el momento de disparar, simplemente devolver null
                yield return null;
            }
        }
    }

    // Corrutina para rotar el cañón de la torreta
    private IEnumerator RotateBarrel(float targetAngle)
    {
        // Calcular la rotación objetivo basada en el ángulo objetivo
        Quaternion targetRotation = Quaternion.Euler(targetAngle, 0f, 0f);
        float elapsedTime = 0f;
        Quaternion startRotation = barrelTransform.localRotation;

        // Lerp the rotation over time
        while (elapsedTime < turretData.rotationSpeed)
        {
            elapsedTime += Time.deltaTime;
            barrelTransform.localRotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / turretData.rotationSpeed);
            yield return null;
        }

        // Establecer la rotación final del cañón
        barrelTransform.localRotation = targetRotation;
    }

    // Corrutina para disparar una bala
    private IEnumerator ShootBullet(bool isTrackingBullet)
    {
        // Alternar entre los puntos de generación de balas
        bulletIndex = (bulletIndex + 1) % 2;
        bulletSpawnPoint = spawnerBullets[bulletIndex];

        // Obtener una bala del Spawner
        GameObject bulletObject = isTrackingBullet ? spawnerScript.PedirBalasPerseguidoras() : spawnerScript.PedirBalas();
        // Colocar la bala en el punto de generación
        bulletObject.transform.position = bulletSpawnPoint.position;
        // Rotar la bala para que mire en la misma dirección que la torreta
        bulletObject.transform.rotation = bodyTransform.rotation;

        // Ultima comprobacion si el efecto del pem está activo antes de disparar
        if (!PemActive)
        {
            // Play the fire effect
            fireFXs[bulletIndex].Play();

            // Obtener los scripts de bala
            Bullet_Lineal linearBulletScript = bulletObject.GetComponent<Bullet_Lineal>();
            Bullet_Tracking trackingBulletScript = bulletObject.GetComponent<Bullet_Tracking>();

            // Activar la bala según el tipo
            if (linearBulletScript != null)
            {
                linearBulletScript.move = true;
            }
            else if (trackingBulletScript != null)
            {
                SetTrackingBulletStartAndTargetPosition(bulletObject.transform);
                trackingBulletScript.move = true;
            }
        }

        // Reproducir el sonido de disparo
        SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.Shoot);

        // Finalizar la corrutina
        yield return null;
    }

    // Método para establecer la posición inicial y objetivo de una bala rastreadora
    private void SetTrackingBulletStartAndTargetPosition(Transform bulletTransform)
    {
        float areaImpacto = 1f;
        float randomX = Random.Range(player.transform.position.x - areaImpacto, player.transform.position.x + areaImpacto);
        float randomZ = Random.Range(player.transform.position.z - areaImpacto, player.transform.position.z + areaImpacto);

        Vector3 startPosition = new Vector3(randomX, 20f, randomZ);
        bulletTransform.position = startPosition;
        bulletTransform.rotation = Quaternion.LookRotation(Vector3.down);
    }

    // Método para aplicar el efecto PEM
    public void ApplyPemEffect(float pemDuration)
    {
        // Si se va a aplicar el efecto del PEM cuando ya había uno activo, entonces se "refresca" el efecto
        if (PemActive)
        {
            StopCoroutine(PemEffectCoroutine(pemDuration));
            pemEffect.SetActive(false);
            PemActive = false;
        }

        // Iniciar la corrutina para aplicar el efecto PEM
        StartCoroutine(PemEffectCoroutine(pemDuration));
    }

    // Corrutina para aplicar el efecto PEM
    private IEnumerator PemEffectCoroutine(float pemDuration)
    {
        // Activar el PEM
        PemActive = true;
        pemEffect.SetActive(PemActive);
        // Rotar el cañón para dar el efecto de "desactivada" la torreta
        yield return RotateBarrel(45f);
        // Esperar la duración del PEM
        yield return new WaitForSeconds(pemDuration);

        // Desactivar el PEM
        PemActive = false;
        pemEffect.SetActive(PemActive);
        // Reiniciar la corrutina de disparo
        StartCoroutine(shootingCoroutine);
    }
}
