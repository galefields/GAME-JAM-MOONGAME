using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AmmoCounter : MonoBehaviour
{
    [SerializeField] private Image scrapFillImage; // Assign in Inspector
    private PlayerController player;
    public GameObject VictoryPanel;
    private GameTimer gameTimer;
    public TMP_Text finalTime;
    public TMP_Text scrapText;

    private void Start()
    {
        gameTimer = FindAnyObjectByType<GameTimer>();
        player = FindObjectOfType<PlayerController>(); // Get reference to player
    }

    private void Update()
    {
        scrapText.text = $"{player.scraps}/{player.scraps2win}";
        if (player != null && player.scraps2win > 0)
        {
            scrapFillImage.fillAmount = (float)player.scraps / player.scraps2win;
        }
        if (player.scraps == player.scraps2win)
        {
            Win();
        }
    }
    void Win()
    {
        VictoryPanel.SetActive(true);
        finalTime = gameTimer.timerText;
    }
}

