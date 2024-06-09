using UnityEngine;
using TMPro;

[System.Serializable]
public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    private float elapsedTime = 0f;

    void Update()
    {
        // Update elapsed time
        elapsedTime += Time.deltaTime;

        // Calculate minutes and seconds
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);

        // Update timer text
        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
    }
}
