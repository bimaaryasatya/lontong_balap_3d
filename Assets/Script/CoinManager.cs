using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Data")]
    public int totalCoin;
    public int collectedCoin;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    [Header("Level Complete")]
    public GameObject levelCompletePanel;
    public LevelCompleteManager levelCompleteManager;

    void Start()
    {
        totalCoin = GameObject.FindGameObjectsWithTag("Coin").Length;
        collectedCoin = 0;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        UpdateCoinText();

        Debug.Log("Total coin di level ini: " + totalCoin);
    }

    public void AddCoin()
    {
        collectedCoin++;

        UpdateCoinText();

        Debug.Log("Coin terkumpul: " + collectedCoin + " / " + totalCoin);

        if (collectedCoin >= totalCoin)
        {
            CompleteAllCoins();
        }
    }

    void CompleteAllCoins()
    {
        Debug.Log("Semua coin terkumpul!");

        if (levelCompleteManager != null)
        {
            levelCompleteManager.UnlockNextLevel();
        }

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coin: " + collectedCoin + " / " + totalCoin;
        }
    }
}