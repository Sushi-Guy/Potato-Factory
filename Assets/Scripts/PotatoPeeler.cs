using UnityEngine;

public class PotatoPeeler : MonoBehaviour
{
    [Header("Prefab Swap Settings")]
    [Tooltip("Drag your custom PEELED potato prefab here.")]
    [SerializeField] private GameObject peeledPotatoPrefab;

    [Header("Conveyor Hand-off")]
    [Tooltip("Drag the Conveyor Belt object this machine sits on here.")]
    [SerializeField] private Transform conveyorBelt;
    [SerializeField] private float beltSpeed = 2.5f;

    [Header("Height Adjustment")]
    [Tooltip("Adjust this if the peeled potato spawns too high or sinks.")]
    [SerializeField] private float yOffset = 0f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            PotatoData potatoData = other.GetComponent<PotatoData>();

            // REQUIREMENT: Only peel if it is washed AND not already peeled!
            if (potatoData != null && potatoData.IsPotatoWashed() && !potatoData.IsPotatoPeeled())
            {
                // 1. SAVE STATE: Capture current position, rotation, and mutation data
                Vector3 spawnPos = other.transform.position;
                spawnPos.y += yOffset; 
                Quaternion spawnRot = other.transform.rotation;

                // Grab the mutation right before the object is destroyed
                PotatoData.MutationTier activeMutation = potatoData.GetCurrentMutation();

                // 2. VAPORIZE: Destroy the washed potato
                Destroy(other.gameObject);

                // 3. SPAWN: Instantiate the beautiful peeled potato prefab
                if (peeledPotatoPrefab != null)
                {
                    GameObject newPeeledPotato = Instantiate(peeledPotatoPrefab, spawnPos, spawnRot);
                    
                    // 4. INJECT MUTATION: Pass the mutation status to the new prefab
                    PotatoData newPotatoData = newPeeledPotato.GetComponent<PotatoData>();
                    if (newPotatoData != null)
                    {
                        newPotatoData.SetMutation(activeMutation);
                    }

                    // 5. PHYSICS HAND-OFF: Send it down the belt smoothly
                    Rigidbody peeledRb = newPeeledPotato.GetComponent<Rigidbody>();
                    if (peeledRb != null)
                    {
                        peeledRb.WakeUp();
                        if (conveyorBelt != null) 
                        {
                            peeledRb.linearVelocity = conveyorBelt.forward * beltSpeed;
                        }
                    }
                }
            }
        }
    }
}