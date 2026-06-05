using UnityEngine;
using TMPro;

public class PlayerWallet : MonoBehaviour
{
    [Header("Master Economy")]
    public int totalMoney = 200;

    [Header("UI Text Display")]
    public TextMeshProUGUI masterMoneyText;

    void Start()
    {
        UpdateWalletUI();
    }

    public bool CanAfford(int amount)
    {
        return totalMoney >= amount;
    }

    public void SpendCash(int amount)
    {
        totalMoney -= amount;
        UpdateWalletUI();
    }

    public void AddGlobalCash(int amount)
    {
        totalMoney += amount;
        UpdateWalletUI();
    }

    void UpdateWalletUI()
    {
        if (masterMoneyText != null)
        {
            // FIX: Running our fresh format math before sending it to the screen text mesh!
            masterMoneyText.text = $"Wallet:\n{FormatNumber(totalMoney)}";
        }
    }

    // TYCOON FORMATTER MECHANIC
    string FormatNumber(int num)
    {
        if (num >= 1000000000) // Billions
        {
            return "$" + (num / 1000000000f).ToString("F1") + "B";
        }
        if (num >= 1000000) // Millions
        {
            return "$" + (num / 1000000f).ToString("F1") + "M";
        }
        if (num >= 1000) // Thousands
        {
            return "$" + (num / 1000f).ToString("F1") + "K";
        }
        
        // Under $1000? Just print normal cash integers
        return "$" + num.ToString();
    }
}