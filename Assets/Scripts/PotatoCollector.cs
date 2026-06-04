using UnityEngine;

public class PotatoCollector : MonoBehaviour
{
    [Header("Economy Settings")]
    [SerializeField] private double potatoValue = 2;

    // This is global, meaning any UI or upgrade script can read this value later
    public static double TotalMoney { get; private set; } = 0; 

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            double earnedAmount = 0;
            string potatoName = "Unknown Potato";

            // Read data directly from the potato instance
            PotatoData potato = other.GetComponent<PotatoData>();

            if (potato != null)
            {
                earnedAmount = potato.GetValue();
                potatoName = potato.GetName();
            }
            else
            {
                // Fallback values if the script component is missing
                earnedAmount = 2;
                potatoName = "Potato";
            }

            // 1. Accumulate wealth
            TotalMoney += earnedAmount;

            // 2. Push updates straight to our new screen UI panels
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateBalanceDisplay(TotalMoney);
                UIManager.Instance.DisplayTransactionNotify(earnedAmount, potatoName);
            }

            // 3. Vaporize physical potato clone
            Destroy(other.gameObject);
        }
    }

    // 1. Check if the player has enough cash
    public bool CanAfford(int cost)
    {
        // Replace 'totalMoney' with the exact name of your variable if it's named differently!
        return TotalMoney >= cost; 
    }

    // 2. Spend the cash and trigger the UI update chain
    public void SpendCash(int cost)
    {
        TotalMoney -= cost;

        UIManager.Instance.UpdateBalanceDisplay(TotalMoney);
    }
}