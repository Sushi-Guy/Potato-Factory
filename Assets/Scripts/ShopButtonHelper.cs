using UnityEngine;

public class ShopButtonHelper : MonoBehaviour
{
    [Header("Linked Manager")]
    public BuildManager buildManager;

    [Header("This Item's Settings")]
    public GameObject realPrefab;
    public GameObject ghostPrefab;
    public int cost = 150;

    void Start()
    {
        // Automatically find the BuildManager in your scene
        buildManager = Object.FindFirstObjectByType<BuildManager>();
    }

    // This is a 0-argument function that WILL show up perfectly in your dropdown!
    public void SendItemToBuildManager()
    {
        // THIS LINE WILL PRINT TO YOUR CONSOLE TO PROVE THE BUTTON IS WORKING!
        Debug.Log($"Button clicked! Sending {realPrefab?.name} to manager.");

        if (buildManager != null)
        {
            buildManager.SelectItemToBuy(realPrefab, ghostPrefab, cost);
        }
        else
        {
            Debug.LogError("The buildManager variable is EMPTY! Drag your GameManager into the slot.");
        }
    }
}