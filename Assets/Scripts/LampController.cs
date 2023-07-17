using UnityEngine;
using DG.Tweening;

public class LampController : MonoBehaviour
{
    public GameObject lampObject;
    public float minDelay = 2f;
    public float maxDelay = 5f;
    public float fadeInTime = 0.5f;
    public float fadeOutTime = 0.5f;

    private bool isLampOn = true;

    private void Start()
    {
        lampObject.SetActive(true);
        StartCoroutine(ToggleLamp());
    }

    private System.Collections.IEnumerator ToggleLamp()
    {
        while (true)
        {
            float waitTime = Random.Range(minDelay, maxDelay);
            yield return new WaitForSeconds(waitTime);

            if (isLampOn)
            {
                isLampOn = false;
                lampObject.transform.DOScale(0f, fadeOutTime)
                    .OnComplete(() =>
                    {
                        lampObject.SetActive(false);
                    });
            }
            else
            {
                isLampOn = true;
                lampObject.SetActive(true);
                lampObject.transform.DOScale(1f, fadeInTime);
            }
        }
    }
}
