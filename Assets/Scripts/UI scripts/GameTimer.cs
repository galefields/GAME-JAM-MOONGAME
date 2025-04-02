using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    public GameObject VictoryPanel;
    public float timeRemaining = 300f;
    public TMP_Text timerText;       

        void Start()
        {
            StartCoroutine(StartCountdown());
        }

        private IEnumerator StartCountdown()
        {
            while (timeRemaining > 0)
            {
                UpdateTimerDisplay();
                yield return new WaitForSeconds(1f);
                timeRemaining--;
            }
            
            TimerEnded();
        }

        private void UpdateTimerDisplay()
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            if (timerText != null)
            {
                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
        }
        private void TimerEnded()
    { 
            Debug.Log("Freshman boyfrienddd!!!");
            SceneManager.LoadScene("WinScene");
    }
}
