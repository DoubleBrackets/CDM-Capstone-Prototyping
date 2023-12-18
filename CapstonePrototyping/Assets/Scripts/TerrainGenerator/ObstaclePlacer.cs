using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;

public class ObstaclePlacer : MonoBehaviour
{
    [SerializeField]
    private Vector2 spawnRange;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private List<EnvironmentSpawnGroup> spawnGroups;

    [SerializeField]
    private bool spawn;

    [SerializeField]
    private Transform batchRoot;

    [SerializeField]
    private bool staticBatch;

    [ReadOnly]
    public int instanced;

    private async void Awake()
    {
        if (!spawn)
        {
            return;
        }

        SpawnObstacles();
    }

    private async UniTaskVoid SpawnObstacles()
    {
        foreach (var spawnGroup in spawnGroups)
        {
            await SpawnGroup(spawnGroup);
        }

        if (staticBatch)
        {
            StaticBatchingUtility.Combine(batchRoot.gameObject);
        }
    }

    private async UniTask SpawnGroup(EnvironmentSpawnGroup spawnGroup)
    {
        for (var i = 0; i < spawnGroup.SpawnCount; i++)
        {
            if (GetRandomSpawnPos(out var location))
            {
                var obstacle = Instantiate(
                    spawnGroup.PotentialObstacles[Random.Range(0, spawnGroup.PotentialObstacles.Count)],
                    batchRoot);
                obstacle.PlaceSelfWithRandomOrientation(location);
                instanced++;
            }

            await UniTask.Yield(PlayerLoopTiming.PreUpdate);
            await UniTask.Yield(PlayerLoopTiming.PreUpdate);
            if (destroyCancellationToken.IsCancellationRequested)
            {
                return;
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

        result = checkPos;
        return true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        var range = new Vector3(spawnRange.x, 50, spawnRange.y);
        Gizmos.DrawWireCube(transform.position + range / 2, range);
    }
}