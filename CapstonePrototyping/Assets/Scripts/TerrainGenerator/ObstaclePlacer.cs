using System.Collections.Generic;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [SerializeField]
    private Vector2 spawnRange;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private List<EnvironmentSpawnGroup> spawnGroups;

    private void Awake()
    {
        foreach (var spawnGroup in spawnGroups)
        {
            SpawnGroup(spawnGroup);
        }
    }

    private void SpawnGroup(EnvironmentSpawnGroup spawnGroup)
    {
        for (var i = 0; i < spawnGroup.SpawnCount; i++)
        {
            if (GetRandomSpawnPos(out var location))
            {
                var obstacle = Instantiate(
                    spawnGroup.PotentialObstacles[Random.Range(0, spawnGroup.PotentialObstacles.Count)],
                    transform);
                obstacle.PlaceSelfWithRandomOrientation(location);
            }
        }
    }

    private bool GetRandomSpawnPos(out Vector3 result)
    {
        var checkPos = new Vector3(
            Random.Range(50, spawnRange.x - 50),
            300,
            Random.Range(50, spawnRange.y - 50)
        );

        // Cast down
        if (Physics.Raycast(checkPos, Vector3.down, out var hit, 300, groundLayer))
        {
            result = hit.point;
            return true;
        }

        result = default;
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var range = new Vector3(spawnRange.x, 50, spawnRange.y);
        Gizmos.DrawWireCube(transform.position + range / 2, range);
    }
}