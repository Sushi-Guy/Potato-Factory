using UnityEngine;
using TMPro;

public class TransactionLogManager : MonoBehaviour
{
    [Header("Prefabs and Parent Containers")]
    public GameObject logTextPrefab;      // Drag your 'LogTextNotification' prefab here
    public Transform logPanelContainer;    // Drag your 'TransactionLog' UI Panel here

    [Header("Log Settings")]
    public int maxLines = 5;               // How many receipts to show before deleting old ones
    public float DisplayDuration = 3f;     // How many seconds a line stays on screen

    public void AddLog(string message, Color textFolderColor)
    {
        // 1. Spawn a brand new text notification line inside the layout panel
        GameObject newLog = Instantiate(logTextPrefab, logPanelContainer);
        
        TextMeshProUGUI tmpText = newLog.GetComponent<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = message;
            tmpText.color = textFolderColor;
        }

        // 2. Auto-vaporize individual lines after a few seconds so it stays clean
        Destroy(newLog, DisplayDuration);

        // 3. Safety: If there are too many lines stacking up, delete the oldest one instantly
        if (logPanelContainer.childCount > maxLines)
        {
            Destroy(logPanelContainer.GetChild(0).gameObject);
        }
    }
}