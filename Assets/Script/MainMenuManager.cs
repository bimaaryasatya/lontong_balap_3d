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
    }

    public void StartNewGame()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 1);
        PlayerPrefs.SetInt("LastPlayedLevel", 1);

        // Reset semua data auto-save posisi & koin jika tekan New Game
        PlayerPrefs.SetInt("IsContinuousSave", 0);
        PlayerPrefs.SetInt("SavedCoins", 0);
        PlayerPrefs.DeleteKey("CollectedCoinsData");
        PlayerPrefs.DeleteKey("CarX");
        PlayerPrefs.DeleteKey("CarY");
        PlayerPrefs.DeleteKey("CarZ");
        PlayerPrefs.DeleteKey("CarRotY");
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelScene);
    }

    public void ContinueGame()
    {
        int lastPlayedLevel = PlayerPrefs.GetInt("LastPlayedLevel", 0);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);
        int targetLevel = lastPlayedLevel > 0 ? lastPlayedLevel : unlockedLevel;

        // AKTIFKAN FITUR CONTINUE: Beri tahu CoinManager di level tujuan untuk memuat posisi terakhir
        PlayerPrefs.SetInt("IsContinuousSave", 1);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene("Level" + targetLevel);
    }

    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
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