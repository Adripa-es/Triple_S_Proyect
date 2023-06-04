using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Lineal : Bullet
{
    public float velocity;

    protected override void InitializeBullet()
    {
        base.InitializeBullet();
        gameObject.tag = "Untagged";
    }

    private new void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone") && gameObject.tag == "Untagged")
        {
            gameObject.tag = "Bullet";
        }
        else if (gameObject.tag == "Bullet")
        {
            base.OnTriggerEnter(other);

            gameObject.tag = "Untagged";
        }
    }


    protected override void MoveBullet()
    {
        this.transform.position += this.transform.forward * Time.deltaTime * velocity;
    }

}


