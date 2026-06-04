using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    [Header("Frame Rate Settings")]
    [Tooltip("Set your desired maximum frame rate. Set to -1 for unlimited.")]
    [SerializeField] private int targetFrameRate = 60;
    
    [Tooltip("Turn on VSync if you want to match your monitor's refresh rate exactly to prevent screen tearing.")]
    [SerializeField] private bool useVSync = false;

    void Awake()
    {
        ApplyFrameRateSettings();
    }

    // Called automatically if you change values in the Inspector while testing
    void OnValidate()
    {
        // Prevent negative values other than -1
        if (targetFrameRate < -1) targetFrameRate = -1;
        if (targetFrameRate == 0) targetFrameRate = 60; 
    }

    public void ApplyFrameRateSettings()
    {
        if (useVSync)
        {
            // VSync count options: 
            // 0 = Don't sync (use targetFrameRate)
            // 1 = Sync with monitor refresh rate (e.g., 60Hz = 60fps, 144Hz = 144fps)
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            // Explicitly turn off VSync so Unity respects our custom target frame rate
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
    }
}