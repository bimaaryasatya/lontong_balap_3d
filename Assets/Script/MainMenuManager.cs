using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene")]
    public string firstLevelScene = "Level1";

    [Header("UI Panel")]
    public GameObject settingsPanel;

    [Header("Settings")]
    public Slider volumeSlider;

    [Header("Buttons")]
    public Button continueButton;
    public Button loadGameButton;

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            volumeSlider.value = savedVolume;
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }

        if (continueButton != null)
        {
            int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);
            int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

            continueButton.interactable = lastPlayedLevel > 0 || unlockedLevel > 1;
        }

        if (loadGameButton != null)
        {
            int hasSaveGame = PlayerPrefs.GetInt("HasSaveGame", 0);
            loadGameButton.interactable = hasSaveGame == 1;
        }
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("LastPlayedLevel", 1);

        // Reset save manual
        PlayerPrefs.SetInt("HasSaveGame", 0);
        PlayerPrefs.DeleteKey("SavedLevel");

        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelScene);
    }

    public void ContinueGame()
    {
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        int targetLevel = lastPlayedLevel > 0 ? lastPlayedLevel : unlockedLevel;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Level" + targetLevel);
    }

    public void LoadSavedGame()
    {
        int hasSaveGame = PlayerPrefs.GetInt("HasSaveGame", 0);

        if (hasSaveGame == 1)
        {
            int savedLevel = PlayerPrefs.GetInt("SavedLevel", 1);

            Time.timeScale = 1f;
            SceneManager.LoadScene("Level" + savedLevel);
        }
        else
        {
            Debug.Log("Belum ada save game.");
        }
    }

    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void SetVolume(float volume)
    {
        AudioListener.volume = volume;

        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.Save();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}