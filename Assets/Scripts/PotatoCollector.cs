using UnityEngine;
using TMPro;

public class PotatoCollector : MonoBehaviour
{
    [Header("Collector Settings")]
    [Tooltip("How much extra value this specific collector adds to a potato (e.g., a premium exit lane).")]
    public int bonusValue = 0;

    [Header("Local UI (Optional)")]
    [Tooltip("If you want a little floating text canvas above this specific box showing its earnings.")]
    public TextMeshProUGUI localScoreText;

    private int cashGeneratedHere = 0;
    private BuildManager buildManager;

    void Start()
    {
        // Automatically find the master build manager in the scene to access the main economy
        buildManager = Object.FindFirstObjectByType<BuildManager>();
        UpdateLocalUI();
    }

    // This is called automatically when a potato physics object physical rolls into the box trigger
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            // FIX: Successfully updated to look for your PotatoData script!
            PotatoData potatoScript = other.GetComponent<PotatoData>();
            int potatoValue = 10; // Default fallback value if it can't find the script

            if (potatoScript != null)
            {
                // NOTE: If the integer inside your PotatoData script is named 
                // something else (like 'price' or 'value'), change 'cashValue' below to match it!
                potatoValue = (int)potatoScript.cashValue;
            }

            // Calculate final payout with this collector's unique bonus modifier
            int finalPayout = potatoValue + bonusValue;
            cashGeneratedHere += finalPayout;

            // Send the earnings straight up to the master player wallet!
            if (buildManager != null && buildManager.playerWallet != null)
            {
                buildManager.playerWallet.AddGlobalCash(finalPayout);
            }

            UpdateLocalUI();

            // Vaporize the potato object so it doesn't pile up and lag the engine
            Destroy(other.gameObject);
        }
    }

    void UpdateLocalUI()
    {
        if (localScoreText != null)
        {
            localScoreText.text = $"${cashGeneratedHere}";
        }
    }
}