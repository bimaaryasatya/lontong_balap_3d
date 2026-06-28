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
        // PENGAMAN: Jika baru pertama kali atau data error, set ke 1f (volume penuh)
        if (!PlayerPrefs.HasKey("Volume"))
        {
            PlayerPrefs.SetFloat("Volume", 1f);
        }

        float savedVolume = PlayerPrefs.GetFloat("Volume", 1f);

        // KOREKSI OTOMATIS: Jika slider kamu terkunci di kiri (0), baris ini akan memaksa reset ke kanan (1)
        if (savedVolume <= 0.05f)
        {
            savedVolume = 1f;
        }

        AudioListener.volume = savedVolume;

        if (volumeSlider != null)
        {
            // Putus hubungan event dulu agar tidak memicu fungsi SetVolume(0) secara tidak sengaja saat awal play
            volumeSlider.onValueChanged.RemoveAllListeners();

            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = savedVolume; // Set nilai slider ke posisi aman

            // Sambungkan kembali fungsi SetVolume ke slider secara aman lewat kode
            volumeSlider.onValueChanged.AddListener(SetVolume);
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
        Debug.Log("Volume diatur ke: " + volume);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}