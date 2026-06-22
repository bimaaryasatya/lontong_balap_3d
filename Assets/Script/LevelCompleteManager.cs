using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 3;

    public void UnlockNextLevel()
    {
        int nextLevel = currentLevel + 1;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
        }

        Debug.Log("Level terbaru terbuka: " + PlayerPrefs.GetInt("UnlockedLevel", 1));
    }

    public void ContinueToNextLevel()
    {
        Time.timeScale = 1f;

        int nextLevel = currentLevel + 1;

        if (nextLevel <= maxLevel)
        {
            SceneManager.LoadScene("Level" + nextLevel);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}