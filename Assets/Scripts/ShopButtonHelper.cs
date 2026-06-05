using UnityEngine;

public class ShopButtonHelper : MonoBehaviour
{
    private BuildManager buildManager;

    [Header("This Item's Settings")]
    public GameObject realPrefab;
    public GameObject ghostPrefab;
    public int cost = 150;
    
    [Tooltip("How high to lift THIS specific ghost model so it sits flat.")]
    public float yOffset = 0f; // <--- ADD THIS VARIABLE

    void Start()
    {
        buildManager = Object.FindFirstObjectByType<BuildManager>();
    }

    public void SendItemToBuildManager()
    {
        if (buildManager != null)
        {
            // Send the custom yOffset over to the manager along with the prefabs
            buildManager.SelectItemToBuy(realPrefab, ghostPrefab, cost, yOffset);
        }
    }
}