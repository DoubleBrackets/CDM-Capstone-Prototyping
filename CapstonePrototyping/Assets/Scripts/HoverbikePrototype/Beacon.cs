using TMPro;
using UnityEngine;

public class Beacon : MonoBehaviour
{
    [SerializeField]
    private GameObject collectedVFX;

    [SerializeField]
    private TextMeshProUGUI text;

    [SerializeField]
    private Vector2 spawnRange;

    [SerializeField]
    private LayerMask groundLayer;

    private static float count;

    private void Awake()
    {
        UpdateText();
        SpawnRandomly();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var vfx = Instantiate(collectedVFX, transform.position, Quaternion.identity);
            Destroy(vfx, 5f);
            count++;
            UpdateText();
            SpawnRandomly();
        }
    }

    private void UpdateText()
    {
        text.text = $"Beacons Collected: {count}";
    }

    private void SpawnRandomly()
    {
        var checkPos = new Vector3(
            Random.Range(50, spawnRange.x - 50),
            300,
            Random.Range(50, spawnRange.y - 50)
        );

        // Cast down
        if (Physics.Raycast(checkPos, Vector3.down, out var hit, 300, groundLayer))
        {
            transform.position = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var range = new Vector3(spawnRange.x, 50, spawnRange.y);
        Gizmos.DrawWireCube(range / 2, range);
    }
}