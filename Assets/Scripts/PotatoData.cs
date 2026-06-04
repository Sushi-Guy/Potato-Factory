using UnityEngine;

public class PotatoData : MonoBehaviour
{
    // Define the types of mutations available
    public enum MutationTier { None, Gold, Diamond, Rainbow }

    [Header("Potato Custom Base Stats")]
    [SerializeField] private string potatoName = "Potato";
    [SerializeField] private double cashValue = 2.0;

    [Header("Processing State")]
    [SerializeField] private bool startsAsWashed = false;
    [SerializeField] private bool startsAsPeeled = false;
    [SerializeField] private bool startsAsCut = false;
    [SerializeField] private bool startsAsFried = false;

    [Header("Mutation System")]
    [SerializeField] private MutationTier currentMutation = MutationTier.None;

    [Header("Mutation Materials")]
    [Tooltip("Create and drag a transparent Gold material here!")]
    [SerializeField] private Material goldMaterial;
    [SerializeField] private Material diamondMaterial;
    [SerializeField] private Material rainbowMaterial;

    void Start()
    {
        Renderer potatoRenderer = GetComponentInChildren<Renderer>();
        
        if (potatoRenderer != null)
        {
            switch (currentMutation)
            {
                case MutationTier.Gold:
                    if (goldMaterial != null) potatoRenderer.material = goldMaterial;
                    break;
                    
                case MutationTier.Diamond:
                    if (diamondMaterial != null) potatoRenderer.material = diamondMaterial;
                    break;
                    
                case MutationTier.Rainbow:
                    if (rainbowMaterial != null) potatoRenderer.material = rainbowMaterial;
                    break;
            }
        }
    }

    // Calculates the final cash value based on its tier multipliers
    public double GetValue()
    {
        double finalValue = cashValue;

        // Apply mutation multiplier over the base item price
        switch (currentMutation)
        {
            case MutationTier.Gold:
                finalValue *= 2.0;
                break;
            case MutationTier.Diamond:
                finalValue *= 5.0;
                break;
            case MutationTier.Rainbow:
                finalValue *= 10.0;
                break;
        }

        return finalValue;
    }

    // Prefixes the name nicely (e.g., "Gold Golden French Fries")
    public string GetName()
    {
        if (currentMutation != MutationTier.None)
        {
            return currentMutation.ToString() + " " + potatoName;
        }
        return potatoName;
    }

    // Public setter so the spawner can assign mutations randomly later
    public void SetMutation(MutationTier newMutation)
    {
        currentMutation = newMutation;
    }
    public PotatoData.MutationTier GetCurrentMutation()
    {
        return currentMutation;
    }

    public bool IsPotatoWashed() => startsAsWashed;
    public bool IsPotatoPeeled() => startsAsPeeled;
    public bool IsPotatoCut() => startsAsCut;
    public bool IsPotatoFried() => startsAsFried;
}