using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteManager : MonoBehaviour
{
    [Header("Level Settings")]
    public int currentLevel = 1;
    public int maxLevel = 3;

    public void CompleteLevel()
    {
        int nextLevel = currentLevel + 1;

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (nextLevel > unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
        }

        if (nextLevel <= maxLevel)
        {
            SceneManager.LoadScene("Level" + nextLevel);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}