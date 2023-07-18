using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage = 10;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
            AudioManager.Instance.PlayOneShot("PlayerTakeHit");
            Destroy(gameObject);
        }
        //mermiyi karaktere atamazsa diye 2.5 saniye sonra yok ediyom. süreyi ayarlarım sonra.
        Destroy(gameObject,2.5f);

    }
}
