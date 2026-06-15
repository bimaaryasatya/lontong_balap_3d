using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CoinManager : MonoBehaviour
{
    [Header("Coin Data")]
    public int totalCoin;
    public int collectedCoin;

    [Header("UI")]
    public TextMeshProUGUI coinText;

    [Header("Level")]
    public string nextSceneName = "Level2";

    void Start()
    {
        totalCoin = GameObject.FindGameObjectsWithTag("Coin").Length;
        collectedCoin = 0;

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
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coin: " + collectedCoin + " / " + totalCoin;
        }
    }
}