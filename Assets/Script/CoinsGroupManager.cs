using UnityEngine;

public class CoinsGroupManager : MonoBehaviour
{
    private CoinManager coinManager;

    void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();

        // Ambil data posisi koin yang sudah diambil sebelumnya dari PlayerPrefs
        string savedCoinsData = PlayerPrefs.GetString("CollectedCoinsData", "");
        string[] collectedPositions = string.IsNullOrEmpty(savedCoinsData) ? new string[0] : savedCoinsData.Split('|');
        bool isContinue = PlayerPrefs.GetInt("IsContinuousSave", 0) == 1;

        // Kumpulkan semua object koin (anak-anak dari parent ini) ke dalam array
        Transform[] allChildCoins = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            allChildCoins[i] = transform.GetChild(i);
        }

        // Lakukan filter dan automasi trigger pada setiap anak koin
        foreach (Transform child in allChildCoins)
        {
            string currentCoinPos = child.position.ToString();
            bool alreadyCollected = false;

            if (isContinue)
            {
                foreach (string pos in collectedPositions)
                {
                    if (currentCoinPos == pos)
                    {
                        alreadyCollected = true;
                        break;
                    }
                }
            }

            if (alreadyCollected)
            {
                // JIKA SUDAH DIAMBIL: Langsung hancurkan koin ini dari map
                Destroy(child.gameObject);
            }
            else
            {
                // JIKA BELUM DIAMBIL: Pastikan komponen Collider anak diset ke IsTrigger
                Collider col = child.GetComponent<Collider>();
                if (col != null)
                {
                    col.isTrigger = true;
                }
                
                // Pasang skrip deteksi tabrakan secara otomatis lewat kode (Runtime)
                ChildCoinProxy proxy = child.gameObject.AddComponent<ChildCoinProxy>();
                proxy.Setup(coinManager);
            }
        }
    }
}

// Skrip pembantu otomatis (Proxy) yang akan menempel pada tiap anak koin secara mandiri
public class ChildCoinProxy : MonoBehaviour
{
    private CoinManager coinManager;

    public void Setup(CoinManager manager)
    {
        coinManager = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Deteksi jika ditabrak oleh player / mobil
        if (other.CompareTag("Player") || other.GetComponentInParent<mobil>() != null)
        {
            if (coinManager != null)
            {
                // Kirim koordinat koin ini ke CoinManager untuk disimpan ke data Auto-Save
                coinManager.AddCoin(gameObject);
            }

            // Hancurkan koin yang berhasil diambil
            Destroy(gameObject);
        }
    }
}