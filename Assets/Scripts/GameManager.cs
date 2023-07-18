using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("References")]
    public TMP_Text currentWaveText;
    public TMP_Text killAndSpawnEnemyText;
    public TMP_Text healthText;
    public GameObject mainCanvas;
    public GameObject dieCanvas;
    public GameObject playerCamObj;

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
    private void Start()
    {
        Time.timeScale = 1f;
        mainCanvas.SetActive(true);
        dieCanvas.SetActive(false);
        currentWaveText.text = "Current Wave : " + (WaveManager.Instance.currentWave + 1).ToString() + " / " + WaveManager.Instance.enemiesPerWave.Length.ToString();
        healthText.text = "HEALTH : " + PlayerManager.Instance.playerHealth.ToString();
    }
    private void Update()
    {
        killAndSpawnEnemyText.text = "ENEMIES " + WaveManager.Instance.enemiesKilled.ToString() + " / " + WaveManager.Instance.enemiesSpawned.ToString();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void Die()
    {
        Time.timeScale = 0.0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        playerCamObj.GetComponent<PlayerCam>().enabled = false;
        playerCamObj.transform.GetChild(0).GetComponent<Gun>().enabled=false;
        mainCanvas.SetActive(false);
        dieCanvas.SetActive(true);

    }

}
