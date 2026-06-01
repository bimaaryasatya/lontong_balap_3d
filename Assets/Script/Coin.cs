using UnityEngine;

public class Coin : MonoBehaviour
{
    private CoinManager coinManager;

    void Start()
    {
        coinManager = FindObjectOfType<CoinManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.GetComponentInParent<mobil>() != null)
        {
            if (coinManager != null)
            {
                coinManager.AddCoin();
            }

            Destroy(gameObject);
        }
    }
}