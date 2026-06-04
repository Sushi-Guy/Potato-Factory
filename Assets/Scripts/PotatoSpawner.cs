using UnityEngine;

public class PotatoSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject potatoPrefab;
    [SerializeField] private Transform spawnPoint;
    
    [Header("Interaction Settings")]
    [Tooltip("Drag the specific button GameObject or Collider here.")]
    [SerializeField] private Transform buttonTransform;
    [SerializeField] private float reachDistance = 4f;

    private Camera playerCam;

    void Start()
    {
        playerCam = Camera.main;
        
        // Friendly warning in case you forget to assign the button
        if (buttonTransform == null)
        {
            Debug.LogWarning($"No Button assigned on {gameObject.name}. Clicking anywhere on this object will spawn potatoes.");
        }
    }

    void Update()
    {
        // Check for Left Mouse Click
        if (Input.GetMouseButtonDown(0))
        {
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        // Shoot a ray directly from the center of the first-person camera
        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, reachDistance))
        {
            // Target determination: If a button is assigned, look for the button. 
            // Otherwise, fall back to clicking the main machine body.
            Transform targetToCheck = (buttonTransform != null) ? buttonTransform : this.transform;

            // Check if the player's crosshair is looking directly at the target
            if (hit.transform == targetToCheck)
            {
                SpawnPotato();
            }
        }
    }

    private void SpawnPotato()
    {
        if (potatoPrefab != null && spawnPoint != null)
        {
            // 1. Calculate the offset position (Y is vertical)
            Vector3 adjustedSpawnPosition = spawnPoint.position;
            adjustedSpawnPosition.y -= 0.5f; 

            // 2. Spawn the potato at the new, lower position
            Instantiate(potatoPrefab, adjustedSpawnPosition, Random.rotation);
        }
    }
}