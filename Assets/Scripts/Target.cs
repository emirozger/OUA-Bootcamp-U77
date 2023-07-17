using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Target : MonoBehaviour
{
    //bu script yüksek iht kullanmıyom artık , sonra bak sil.
    public float health = 50f;
  

    public void TakeDamage(float amount)
    {
        
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }
    public void Die()
    {
        
        Destroy(gameObject);
    }
}
