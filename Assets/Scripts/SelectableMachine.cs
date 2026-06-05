using UnityEngine;

public class SelectableMachine : MonoBehaviour
{
    private Renderer[] myRenderers;
    private Color[] originalColors;
    private bool isHighlighted = false;

    void Awake()
    {
        myRenderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[myRenderers.Length];
        
        // Save original colors so we can revert back cleanly
        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (myRenderers[i] != null && myRenderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = myRenderers[i].material.color;
            }
        }
    }

    public void SetHighlight(bool shouldHighlight)
    {
        isHighlighted = shouldHighlight;

        for (int i = 0; i < myRenderers.Length; i++)
        {
            if (myRenderers[i] == null) continue;

            if (isHighlighted)
            {
                // Tint the machine a clean, bright selection blue
                myRenderers[i].material.color = new Color(0.3f, 0.6f, 1f, 1f);
            }
            else
            {
                // Snap back to normal factory colors
                if (myRenderers[i].material.HasProperty("_Color"))
                {
                    myRenderers[i].material.color = originalColors[i];
                }
            }
        }
    }
}