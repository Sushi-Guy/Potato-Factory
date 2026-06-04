using UnityEngine;

public class PotatoWasher : MonoBehaviour
{
    [Header("Prefab Swap Settings")]
    [Tooltip("Drag your custom WASHED potato prefab here.")]
    [SerializeField] private GameObject washedPotatoPrefab;

    [Header("Conveyor Hand-off")]
    [Tooltip("Drag your Conveyor Belt object here.")]
    [SerializeField] private Transform conveyorBelt;
    [SerializeField] private float beltSpeed = 2.5f;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Potato"))
        {
            PotatoData dirtyPotatoData = other.GetComponent<PotatoData>();

            // 1. Safety Check: Don't wash it if it's missing data or already washed
            if (dirtyPotatoData == null || dirtyPotatoData.IsPotatoWashed()) return;

            // 2. SAVE STATE: Capture position, rotation, and its mutation tier!
            Vector3 spawnPos = other.transform.position;
            Quaternion spawnRot = other.transform.rotation;
            
            // Get the current enum mutation (None, Gold, Diamond, Rainbow)
            PotatoData.MutationTier activeMutation = dirtyPotatoData.GetCurrentMutation();

            // 3. VAPORIZE: Destroy the old dirty potato model
            Destroy(other.gameObject);

            // 4. SPAWN: Instantiate the pristine washed potato prefab
            if (washedPotatoPrefab != null)
            {
                GameObject newWashedPotato = Instantiate(washedPotatoPrefab, spawnPos, spawnRot);
                
                // 5. INJECT MUTATION: Find the new script component and hand off the rare status
                PotatoData washedPotatoData = newWashedPotato.GetComponent<PotatoData>();
                if (washedPotatoData != null)
                {
                    washedPotatoData.SetMutation(activeMutation);
                }

                // 6. PHYSICS HAND-OFF: Wake up the physics and push it down the track
                Rigidbody washedRb = newWashedPotato.GetComponent<Rigidbody>();
                if (washedRb != null)
                {
                    washedRb.WakeUp();

                    if (conveyorBelt != null)
                    {
                        // Keeps it locked tightly to your belt's local blue arrow
                        washedRb.linearVelocity = conveyorBelt.forward * beltSpeed;
                    }
                }
            }
        }
    }
}