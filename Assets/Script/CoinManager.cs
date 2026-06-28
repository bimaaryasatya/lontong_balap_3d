using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Tambahkan ini untuk mendeteksi nama level

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

    // ===== VARIABEL TAMBAHAN AUTO SAVE =====
    private Transform playerCarTransform;
    private float saveInterval = 2f; // Auto-save posisi mobil berjalan tiap 2 detik
    private float timer;
    // =======================================

    void Start()
    {
        // 1. Catat level aktif saat ini sebagai level terakhir yang dimainkan
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.StartsWith("Level"))
        {
            int levelNum = int.Parse(currentScene.Replace("Level", ""));
            PlayerPrefs.SetInt("LastPlayedLevel", levelNum);
            PlayerPrefs.Save();
        }

        // 2. Cari mobil di dalam map (menggunakan komponen 'mobil')
        mobil car = FindObjectOfType<mobil>();
        if (car != null)
        {
            playerCarTransform = car.transform;
        }

        // 3. Hitung jumlah koin dari skrip parent CoinsGroupManager
        CoinsGroupManager groupManager = FindObjectOfType<CoinsGroupManager>();
        if (groupManager != null)
        {
            totalCoin = groupManager.transform.childCount;
        }
        else
        {
            totalCoin = GameObject.FindGameObjectsWithTag("Coin").Length;
        }

        collectedCoin = 0;

        if (levelCompletePanel != null)
        {
            levelCompletePanel.SetActive(false);
        }

        // 4. MEMUAT DATA: Jika masuk lewat tombol Continue, ambil posisi mobil & koin terakhir
        if (PlayerPrefs.GetInt("IsContinuousSave", 0) == 1)
        {
            LoadGameplayState();
        }

        UpdateCoinText();
        Debug.Log("Total coin di level ini: " + totalCoin);
    }

    void Update()
    {
        // Jalankan interval waktu untuk auto-save posisi koordinat mobil
        if (playerCarTransform != null)
        {
            timer += Time.deltaTime;
            if (timer >= saveInterval)
            {
                SaveGameplayState();
                timer = 0f;
            }
        }
    }

    public void SaveGameplayState()
    {
        if (playerCarTransform == null) return;

        // Simpan posisi X, Y, Z dan Rotasi mobil ke PlayerPrefs
        PlayerPrefs.SetFloat("CarX", playerCarTransform.position.x);
        PlayerPrefs.SetFloat("CarY", playerCarTransform.position.y);
        PlayerPrefs.SetFloat("CarZ", playerCarTransform.position.z);
        PlayerPrefs.SetFloat("CarRotY", playerCarTransform.eulerAngles.y);

        // Simpan jumlah angka koin terkumpul
        PlayerPrefs.SetInt("SavedCoins", collectedCoin);
        PlayerPrefs.Save();
    }

    private void LoadGameplayState()
    {
        // Teleportasi posisi mobil kembali ke titik koordinat terakhir saat disave
        if (PlayerPrefs.HasKey("CarX") && playerCarTransform != null)
        {
            float x = PlayerPrefs.GetFloat("CarX");
            float y = PlayerPrefs.GetFloat("CarY");
            float z = PlayerPrefs.GetFloat("CarZ");
            float rotY = PlayerPrefs.GetFloat("CarRotY");

            Rigidbody rb = playerCarTransform.GetComponent<Rigidbody>();
            if (rb != null) rb.isKinematic = true; // Nonaktifkan physics agar tidak bentrok saat teleport

            playerCarTransform.position = new Vector3(x, y, z);
            playerCarTransform.rotation = Quaternion.Euler(0, rotY, 0);

            if (rb != null) rb.isKinematic = false; // Aktifkan physics kembali
        }

        // Kembalikan jumlah nilai koin terkumpul ke dalam hitungan variabel
        collectedCoin = PlayerPrefs.GetInt("SavedCoins", 0);
    }

    public void AddCoin(GameObject coinObj = null)
    {
        collectedCoin++;
        UpdateCoinText();

        // Catat koordinat posisi koin yang diambil ke database string
        if (coinObj != null)
        {
            string coinPos = coinObj.transform.position.ToString();
            string savedData = PlayerPrefs.GetString("CollectedCoinsData", "");
            
            if (string.IsNullOrEmpty(savedData))
                savedData = coinPos;
            else
                savedData += "|" + coinPos;

            PlayerPrefs.SetString("CollectedCoinsData", savedData);
        }

        // Simpan progres instan setiap kali mendapatkan koin baru
        SaveGameplayState();

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

    public void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = "Coin: " + collectedCoin + " / " + totalCoin;
        }
    }

    // Melakukan save otomatis darurat apabila pemain menekan tombol Back To Menu / Quit Game di tengah jalan
    void OnDisable()
    {
        SaveGameplayState();
    }
}