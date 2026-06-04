using UnityEngine;

public class PotatoCutter : MonoBehaviour
{
    [Header("Prefab Swap Settings")]
    [Tooltip("Drag your custom CUT/RAW FRY prefab here.")]
    [SerializeField] private GameObject cutPotatoPrefab;

    [Header("Conveyor Hand-off")]
    [SerializeField] private Transform conveyorBelt;
    [SerializeField] private float beltSpeed = 2.5f;

    [Header("Height Adjustment")]
    [SerializeField] private float yOffset = 0.15f; // Prevents clipping through belt

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            PotatoData potatoData = other.GetComponent<PotatoData>();

            // Only chop if it's peeled and not yet cut
            if (potatoData != null && potatoData.IsPotatoPeeled() && !potatoData.IsPotatoCut())
            {
                // 1. SAVE STATE: Capture position, rotation, and active mutation
                Vector3 spawnPos = other.transform.position;
                spawnPos.y += yOffset; 
                Quaternion spawnRot = other.transform.rotation;

                PotatoData.MutationTier activeMutation = potatoData.GetCurrentMutation();

                // 2. VAPORIZE
                Destroy(other.gameObject);

                // 3. SPAWN
                if (cutPotatoPrefab != null)
                {
                    GameObject newCutPotato = Instantiate(cutPotatoPrefab, spawnPos, spawnRot);
                    
                    // 4. INJECT MUTATION
                    PotatoData newPotatoData = newCutPotato.GetComponent<PotatoData>();
                    if (newPotatoData != null)
                    {
                        newPotatoData.SetMutation(activeMutation);
                    }

                    // 5. PHYSICS HAND-OFF
                    Rigidbody rb = newCutPotato.GetComponent<Rigidbody>();
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