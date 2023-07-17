using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public int playerHealth = 100;
    public static PlayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        playerHealth -= damage;

        if (playerHealth <= 0)
        {
            //die ui yaz
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("FallDetector"))
        {
            this.gameObject.transform.DOMove(new Vector3(0f, 7f, 7f), 1.5f);
        }
    }

}
