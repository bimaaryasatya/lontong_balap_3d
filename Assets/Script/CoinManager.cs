using UnityEngine;
using UnityEngine.SceneManagement;

public class CoinManager : MonoBehaviour
{
    public int totalCoin;
    public int collectedCoin;

    public string nextSceneName = "Level2";

    void Start()
    {
        totalCoin = GameObject.FindGameObjectsWithTag("Coin").Length;
        collectedCoin = 0;

        Debug.Log("Total coin di level ini: " + totalCoin);
    }

    public void AddCoin()
    {
        collectedCoin++;

        Debug.Log("Coin terkumpul: " + collectedCoin + " / " + totalCoin);

        if (collectedCoin >= totalCoin)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}