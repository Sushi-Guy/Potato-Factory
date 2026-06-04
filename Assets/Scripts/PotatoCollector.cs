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
}