using UnityEngine;

public class AutoPotatoSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    [SerializeField] private GameObject potatoPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float spawnInterval = 2f;

    [Header("Mutation Chances (%)")]
    [Range(0f, 100f)] [SerializeField] private float goldChance = 10f;

    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnPotato();
            timer = 0f;
        }
    }

    void SpawnPotato()
    {
        if (potatoPrefab == null) return;

        // 1. Spawn the standard baseline potato prefab
        Transform targetPoint = spawnPoint != null ? spawnPoint : transform;
        GameObject newPotato = Instantiate(potatoPrefab, targetPoint.position, targetPoint.rotation);

        // 2. Grab its data component to assign the mutation
        PotatoData potatoData = newPotato.GetComponent<PotatoData>();
        if (potatoData != null)
        {
            // Roll a random decimal number between 0.0 and 100.0
            float roll = Random.Range(0f, 100f);

            // If the roll is 10 or under, it qualifies for the 10% bracket!
            if (roll <= goldChance)
            {
                potatoData.SetMutation(PotatoData.MutationTier.Gold);
            }
            else
            {
                // Otherwise, it's just a normal potato
                potatoData.SetMutation(PotatoData.MutationTier.None);
            }
        }
    }
}