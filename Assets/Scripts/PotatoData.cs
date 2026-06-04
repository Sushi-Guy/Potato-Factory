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
    [SerializeField] private Material goldMaterial;
    [SerializeField] private Material diamondMaterial;
    [SerializeField] private Material rainbowMaterial;

    void Start()
    {
        // 1. Grab ALL renderers inside this prefab (crucial for multi-mesh objects like fries)
        Renderer[] potatoRenderers = GetComponentsInChildren<Renderer>();
        
        // 2. Loop through every single fry piece/mesh found
        foreach (Renderer rand in potatoRenderers)
        {
            if (rand != null)
            {
                switch (currentMutation)
                {
                    case MutationTier.Gold:
                        // Option A: If you are using the custom Material approach:
                        if (goldMaterial != null) rand.material = goldMaterial;
                        
                        // Option B: If you are using the flat color tint approach:
                        // rand.material.color = new Color(1f, 0.84f, 0f, 0.75f);
                        break;
                        
                    case MutationTier.Diamond:
                        if (diamondMaterial != null) rand.material = diamondMaterial;
                        break;
                        
                    case MutationTier.Rainbow:
                        if (rainbowMaterial != null) rand.material = rainbowMaterial;
                        break;
                }
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