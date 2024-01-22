using UnityEngine;

public class enemy_spawner : MonoBehaviour
{

    [SerializeField] private GameObject skeletonPrefab; // Prefab of the skeleton enemy
    [SerializeField] private GameObject castleCenter; // The center point of the castle
    [SerializeField] private float spawnRadius; // The distance from the castle center to spawn enemies
    [SerializeField] private float spawnInterval; // Time between enemy spawns

    private float timer; // Timer for spawn interval

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            SpawnSkeleton();
            timer = spawnInterval;
        }
    }

    private void SpawnSkeleton()
    {
        // Generate a random angle around the castle center
        float angle = Random.Range(0.0f, 2.0f * Mathf.PI);

        // Calculate the spawn position on the circle using trigonometry
        Vector3 spawnPosition = castleCenter.transform.position + new Vector3(Mathf.Cos(angle) * spawnRadius, 0f, Mathf.Sin(angle) * spawnRadius);

        // Check for valid spawn location (avoid obstacles, etc.)
        // TODO: Implement your own logic for obstacle checking here

        // Spawn the skeleton
        Instantiate(skeletonPrefab, spawnPosition, Quaternion.identity);
    }
}
