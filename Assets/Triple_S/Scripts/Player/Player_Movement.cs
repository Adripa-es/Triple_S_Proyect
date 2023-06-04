using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player_Movement : MonoBehaviour, IDamageable
{
    [Header("Stats Player")]
    [Space]
    [SerializeField] public float moveSpeed;
    private Vector3 moveDirection; // Dirección de movimiento actual
    private Rigidbody rb; // Referencia al componente Rigidbody del jugador
    [SerializeField] private GameObject modelPlayer;

    [Header("Dash Effect/Stats")]
    [Space]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    [SerializeField] private bool isDashing, dashReady; // Variables para controlar el estado del dash
    private Coroutine dashCoroutine; // Corrutina para manejar el dash

    [Header("Shield Effect/ Stats")]
    [Space]
    [SerializeField] private GameObject shieldBubble;  // Efecto visual de escudo
    [SerializeField] private float shieldDashDuration;
    [SerializeField] private float shieldDuration;
    private Coroutine immunityEffectCoroutine; //Corrutina para manejar el escudo

    [Header("Heal Effect/ Stats")]
    [Space]
    [SerializeField] private GameObject healingEffect; // Efecto visual de curacion
    [SerializeField] private float healBuffDuration;
    private Coroutine healingEffectCoroutine; //Corrutina para manejar la curacion

    // Inicialización
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        dashReady = true;
        shieldBubble.SetActive(false);
        healingEffect.SetActive(false);
    }

    // Actualización cada frame
    void Update()
    {
        // Si el juego está pausado, detenemos al jugador
        if (Manager_Controller.instance.isPaused)
        {
            StopPlayer();
            return;
        }

        // Recoge la entrada del jugador y la normaliza
        moveDirection = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical")).normalized;

        // Inicia la corutina de Dash si la tecla de Dash se ha presionado y el Dash está listo
        if (Input.GetKeyDown(KeyCode.Space) && dashReady)
        {
            dashCoroutine = StartCoroutine(Dash());
        }

        // Si el jugador no está dando entrada de movimiento, detiene al jugador
        if (moveDirection == Vector3.zero)
        {
            StopMovement();
        }
    }

    // Actualización en intervalos fijos
    void FixedUpdate()
    {
        // Solo mover y rotar si no está realizando un Dash
        if (!isDashing)
        {
            Move();
            Rotate();
        }
    }

    // Al entrar en una colisión con un objeto
    private void OnTriggerEnter(Collider other)
    {
        // Gestión de power-ups basada en su etiqueta
        switch (other.tag)
        {
            case "ShieldPowerUp":
                ToggleImmunity(shieldDuration);
                break;
            case "Heart":
                ToggleHealBuff(healBuffDuration);
                break;
        }
    }

    // Mover al jugador
    private void Move()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }

    // Rotar al jugador hacia la dirección de movimiento
    private void Rotate()
    {
        if (moveDirection == Vector3.zero) return; // No rotar si no se está moviendo
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        rb.rotation = Quaternion.Lerp(rb.rotation, targetRotation, Time.fixedDeltaTime * 15f);
    }

    // Detiene todas las acciones del jugador
    private void StopPlayer()
    {
        StopMovement();
        // Cancela Dash en curso
        if (dashCoroutine != null) StopCoroutine(dashCoroutine);
    }

    // Detiene el movimiento del jugador
    private void StopMovement()
    {
        rb.velocity = Vector3.zero;
    }

    // Llamada cuando el jugador recibe daño
    public void TakeDamage(int damage)
    {
        Manager_Controller.instance.setVidas(-damage);
    }

    // Coroutine para el Dash
    private IEnumerator Dash()
    {
        modelPlayer.SetActive(false);

        Vector3 dashDirection = moveDirection; // Guardar la dirección de Dash
        ToggleImmunity(shieldDashDuration); // Iniciar la inmunidad del Dash

        // Comprobar colisiones en la dirección del Dash
        RaycastHit hit;
        if (Physics.Raycast(rb.position, dashDirection, out hit, dashSpeed * dashDuration))
        {
            // Si hay una colisión, ajustar la posición del jugador al punto de colisión
            rb.MovePosition(hit.point);
        }
        else
        {
            // Si no hay colisión, mover al jugador en la dirección del Dash
            rb.MovePosition(rb.position + dashDirection * dashSpeed * dashDuration);
        }

        SFX_Controller.instance.PlaySound(SFX_Controller.SoundType.Shield); // Sonido de escudo

        isDashing = true; // Marcar Dash como activo
        dashReady = false; // Desactivar el Dash hasta que se complete el enfriamiento

        yield return new WaitForSeconds(dashDuration); // Esperar la duración del Dash

        modelPlayer.SetActive(true);

        isDashing = false; // Marcar Dash como no activo

        yield return new WaitForSeconds(dashCooldown); // Esperar el tiempo de enfriamiento

        dashReady = true; // Marcar Dash como listo para el próximo uso
    }


    // Inicia/cancela la inmunidad
    public void ToggleImmunity(float immunityDuration)
    {
        if (immunityEffectCoroutine != null) StopCoroutine(immunityEffectCoroutine); // Cancela la inmunidad existente
        immunityEffectCoroutine = StartCoroutine(ToggleImmunityEffect(immunityDuration)); // Inicia nueva inmunidad
    }

    // Coroutine para gestionar la duración de la inmunidad
    public IEnumerator ToggleImmunityEffect(float immunityDuration)
    {
        shieldBubble.SetActive(true); // Activar escudo
        yield return new WaitForSeconds(immunityDuration); // Esperar la duración de la inmunidad
        shieldBubble.SetActive(false); // Desactivar escudo
    }

    // Inicia/cancela el buff de curación
    public void ToggleHealBuff(float healBuffDuration)
    {
        if (healingEffectCoroutine != null) StopCoroutine(healingEffectCoroutine); // Cancela el buff de curación existente
        healingEffectCoroutine = StartCoroutine(ToggleHealEffect(healBuffDuration)); // Inicia nuevo buff de curación
    }

    // Coroutine para gestionar la duración del buff de curación
    public IEnumerator ToggleHealEffect(float healBuffDuration)
    {
        healingEffect.SetActive(true); // Activar el efecto visual de buff de curación
        yield return new WaitForSeconds(healBuffDuration); // Esperar la duración del buff
        healingEffect.SetActive(false); // Desactivar  el efecto visual de buff de curación
    }
}
