using UnityEngine;

public class PotatoFryer : MonoBehaviour
{
    [Header("Prefab Swap Settings")]
    [Tooltip("Drag your custom FINISHED FRENCH FRIES prefab here.")]
    [SerializeField] private GameObject friedPotatoPrefab;

    [Header("Conveyor Hand-off")]
    [SerializeField] private Transform conveyorBelt;
    [SerializeField] private float beltSpeed = 2.5f;

    [Header("Height Adjustment")]
    [SerializeField] private float yOffset = 0.1f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            PotatoData potatoData = other.GetComponent<PotatoData>();

            // Only fry if it's cut and not yet fried
            if (potatoData != null && potatoData.IsPotatoCut() && !potatoData.IsPotatoFried())
            {
                // 1. SAVE STATE: Capture position, rotation, and active mutation
                Vector3 spawnPos = other.transform.position;
                spawnPos.y += yOffset;
                Quaternion spawnRot = other.transform.rotation;

                PotatoData.MutationTier activeMutation = potatoData.GetCurrentMutation();

                // 2. VAPORIZE
                Destroy(other.gameObject);

                // 3. SPAWN
                if (friedPotatoPrefab != null)
                {
                    GameObject newFriedPotato = Instantiate(friedPotatoPrefab, spawnPos, spawnRot);
                    
                    // 4. INJECT MUTATION
                    PotatoData newPotatoData = newFriedPotato.GetComponent<PotatoData>();
                    if (newPotatoData != null)
                    {
                        newPotatoData.SetMutation(activeMutation);
                    }

                    // 5. PHYSICS HAND-OFF
                    Rigidbody rb = newFriedPotato.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        rb.WakeUp();
                        if (conveyorBelt != null) 
                        {
                            rb.linearVelocity = conveyorBelt.forward * beltSpeed;
                        }
                    }
                }
            }
        }
    }
}