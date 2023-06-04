using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Tracking : Bullet
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject impactPoint;
    [SerializeField] private float followPlayerFactor;  // Factor de ajuste de la velocidad de la bala en relación con la velocidad del jugador
    [SerializeField] private float fallSpeed; // velocidad de caída de la bala
    private float heightImpactPoint = 1.5f;

    private Vector3 impactPointTargetPosition; // Posición objetivo del punto de impacto
    private float bulletSpeed; // Velocidad de la bala

    private Player_Movement playerController; // Referencia al controlador del jugador

    protected override void InitializeBullet()
    {
        base.InitializeBullet();
        impactPoint.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player");

        // Obtener la referencia al script del jugador
        playerController = player.GetComponent<Player_Movement>();
    }

    protected override void MoveBullet()
    {
        // Actualizar la velocidad de la bala en función de la velocidad del jugador
        bulletSpeed = playerController.moveSpeed * followPlayerFactor;

        // Actualizar la posición objetivo del punto de impacto para seguir al jugador
        impactPointTargetPosition = new Vector3(player.transform.position.x, heightImpactPoint, player.transform.position.z);

        // Interpolar la posición del punto de impacto hacia la posición objetivo
        impactPoint.transform.position = Vector3.Lerp(impactPoint.transform.position, impactPointTargetPosition, followPlayerFactor * Time.deltaTime);

        // Mover la bala horizontalmente (en los ejes X y Z) hacia el punto de impacto
        Vector3 horizontalDirection = new Vector3(impactPoint.transform.position.x - transform.position.x, 0, impactPoint.transform.position.z - transform.position.z).normalized;
        transform.position += horizontalDirection * Time.deltaTime * bulletSpeed;

        // Mover la bala verticalmente (en el eje Y) hacia abajo a la velocidad de caída
        transform.position -= new Vector3(0, fallSpeed * Time.deltaTime, 0);

        // Mantener el impactPoint en la altura deseada
        impactPoint.transform.position = new Vector3(impactPoint.transform.position.x, heightImpactPoint, impactPoint.transform.position.z);

        if (impactPoint != null)
        {
            impactPoint.SetActive(true);
        }
    }

    public override void ReturnBullet()
    {
        base.ReturnBullet();
        impactPoint.SetActive(false);
    }
}



