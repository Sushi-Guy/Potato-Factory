using UnityEngine;
using TMPro; // Required to control TextMeshPro objects

public class UIManager : MonoBehaviour
{
    // Singleton pattern so the Collector can easily find this script
    public static UIManager Instance { get; private set; }

    [Header("UI Component Elements")]
    [SerializeField] private TextMeshProUGUI balanceText;
    [SerializeField] private TextMeshProUGUI logText;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    /// <summary>
    /// Updates the main wallet balance display on screen.
    /// </summary>
    public void UpdateBalanceDisplay(double currentTotal)
    {
        if (balanceText != null)
        {
            balanceText.text = "Wallet: $" + CurrencyFormatter.FormatMoney(currentTotal);
        }
    }

    /// <summary>
    /// Spits out a cozy transaction alert showing how much you got and from what.
    /// </summary>
    public void DisplayTransactionNotify(double amountGot, string potatoSource)
    {
        if (logText != null)
        {
            string formattedAmount = CurrencyFormatter.FormatMoney(amountGot);
            
            // Displays: "+$5.00 from Standard Spud"
            logText.text = $"+${formattedAmount} from {potatoSource}";
        }
    }
}