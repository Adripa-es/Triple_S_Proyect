using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Items_Script : MonoBehaviour
{
    [Header("Item Type Data")]
    [Space]
    public Scriptable_Object objectoData; // Referencia al objeto Scriptable_Object que contiene los datos del objeto.
    public enum ItemType { Coin, Health, Tramp, ImmunityShield, Pem } // Enumeración que define los tipos de objetos disponibles.
    public ItemType itemType; // Tipo de objeto actual.
    [SerializeField] private float rotationSpeed; // Velocidad de rotación del objeto.
    private bool collisionShieldBubble = false;

    [Header("Item Effects")]
    [Space]
    public GameObject despawnEffect; // Efecto de desaparición del objeto.
    public GameObject getItemEffect; // Efecto de obtención del objeto.

    private void Start()
    {
        SetEffectActive(despawnEffect, false); // Desactiva el efecto de desaparición al inicio.
        SetEffectActive(getItemEffect, false); // Desactiva el efecto de obtención al inicio.

        StartCoroutine(DespawnItem(objectoData.timeDespawn)); // Inicia la rutina de desaparición del objeto después de un tiempo determinado.
    }

    private void Update()
    {
        RotateItem(); // Rota el objeto.
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HandleItemType(); // Maneja el tipo de objeto cuando el jugador entra en contacto con él.
            StartCoroutine(GetItemEffect()); // Inicia la rutina del efecto de obtención del objeto.
        }
        else if (other.CompareTag("ShieldBubble"))
        {
            collisionShieldBubble = true;
        }
    }

    // Maneja el tipo de objeto y realiza las acciones correspondientes.
    private void HandleItemType()
    {
        SFX_Controller.SoundType sound;
        switch (itemType)
        {
            case ItemType.Coin: // Si es una moneda
                sound = SFX_Controller.SoundType.Coins; // Establece el sonido de monedas.
                Manager_Controller.instance.setTotalCoins(objectoData.coinValue); // Actualiza la cantidad total de monedas en el juego.
                break;

            case ItemType.Health: // Si es un objeto de salud
                sound = SFX_Controller.SoundType.Healing; // Establece el sonido de curación.
                Manager_Controller.instance.setVidas(objectoData.healthValue); // Actualiza la cantidad de vidas del jugador.
                break;

            case ItemType.Tramp: // Si es una trampa
                sound = SFX_Controller.SoundType.Bomb; // Establece el sonido de explosión.

                // Si no se colisionó con el escudo, entonces se restan vidas
                if (!collisionShieldBubble)
                {
                    Manager_Controller.instance.setVidas(-objectoData.healthValue); // Resta vidas al jugador.
                }
                break;

            case ItemType.ImmunityShield: // Si es un escudo de inmunidad
                sound = SFX_Controller.SoundType.Shield; // Establece el sonido de escudo.
                break;

            case ItemType.Pem: // Si es un objeto PEM
                sound = SFX_Controller.SoundType.Pem; // Establece el sonido de PEM.
                ApplyPemEffectToTurrets(); // Aplica el efecto PEM a las torretas.
                break;

            default:
                return;
        }
        SFX_Controller.instance.PlaySound(sound); // Reproduce el sonido correspondiente.
    }

    // Aplica el efecto PEM a las torretas dentro de un radio determinado.
    private void ApplyPemEffectToTurrets()
    {
        TurretTracking[] allTurrets = FindObjectsOfType<TurretTracking>(); // Encuentra todas las torretas en la escena.
        foreach (TurretTracking turret in allTurrets)
        {
            if (Vector3.Distance(transform.position, turret.transform.position) <= objectoData.pemRadius) // Comprueba la distancia entre el objeto y la torreta.
            {
                turret.ApplyPemEffect(objectoData.pemDuration); // Aplica el efecto PEM a la torreta.
            }
        }
    }

    // Rutina de desaparición del objeto después de un tiempo determinado.
    private IEnumerator DespawnItem(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        EffectDespawn(); // Activa el efecto de desaparición.
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject); // Destruye el objeto.
    }

    // Activa el efecto de desaparición del objeto.
    public void EffectDespawn()
    {
        SetEffectActive(despawnEffect, true); // Activa el efecto de desaparición.
        if (itemType == ItemType.Tramp || itemType == ItemType.Pem) // Si el objeto es una trampa o un objeto PEM.
        {
            SFX_Controller.SoundType sound = itemType == ItemType.Tramp ? SFX_Controller.SoundType.Bomb : SFX_Controller.SoundType.Pem; // Establece el sonido correspondiente.
            SFX_Controller.instance.PlaySound(sound); // Reproduce el sonido.
        }
    }

    // Rutina del efecto de obtención del objeto.
    public IEnumerator GetItemEffect()
    {
        SetEffectActive(getItemEffect, true); // Activa el efecto de obtención.
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject); // Destruye el objeto.
    }

    // Rota el objeto.
    private void RotateItem()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
    }

    // Activa o desactiva un efecto.
    private void SetEffectActive(GameObject effect, bool isActive)
    {
        if (effect != null)
        {
            effect.SetActive(isActive); // Activa o desactiva el efecto.
        }
    }
}
