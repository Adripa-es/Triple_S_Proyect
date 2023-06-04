using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    protected float countdownTimer = 0f;
    public float countdownDuration;

    public Scriptable_Turret turretData;

    [HideInInspector]
    public bool move = false;

    protected Spawner_Controller spawnerScript;
    public GameObject bullet;
    

    void Start()
    {
        InitializeBullet();
    }

    protected virtual void InitializeBullet()
    {
        spawnerScript = GameObject.FindGameObjectWithTag("Mapa").GetComponent<Spawner_Controller>();
        bullet.SetActive(false);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageableObject = other.gameObject.GetComponent<IDamageable>();
        if (gameObject.tag == "Bullet" && ((other.CompareTag("Player")) || (other.CompareTag("DeathZone")) || (other.CompareTag("ShieldBubble"))))
        {
            if (other.CompareTag("Player"))
            {
                damageableObject.TakeDamage((int)turretData.damage);
            }

            ReturnBullet();
        }
    }

    void Update()
    {
        if (move)
        {
            bullet.SetActive(true);
            countdownTimer += Time.deltaTime;
            if (countdownTimer >= countdownDuration)
            {
                ReturnBullet();
                countdownTimer = 0f;
            }
            MoveBullet();
        }
    }

    protected virtual void MoveBullet()
    {
        
    }

    public virtual void ReturnBullet()
    {
        move = false;
        countdownTimer = 0f;
        bullet.SetActive(false);

        this.transform.position = spawnerScript.bala.transform.position;
    }
}

