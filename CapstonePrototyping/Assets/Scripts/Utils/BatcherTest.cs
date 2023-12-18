using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BatcherTest : MonoBehaviour
{
    [SerializeField]
    public List<GameObject> prefabs;

    [SerializeField]
    private int count;

    [SerializeField]
    private bool batch;

    [SerializeField]
    private int batchCount;

    private void Start()
    {
        // instantiate prefabs randomly
        Spawn();
    }

    private async UniTaskVoid Spawn()
    {
        var batches = count / batchCount;
        for (var i = 0; i < batches; i++)
        {
            for (var j = 0; j < batchCount; j++)
            {
                var prefab = prefabs[Random.Range(0, prefabs.Count)];
                var instance = Instantiate(prefab, transform);
                instance.transform.position = Vector3.right * (i * batchCount + j) * 3;
            }

            await UniTask.Yield(PlayerLoopTiming.PreUpdate);
            if (destroyCancellationToken.IsCancellationRequested)
            {
                return;
            }
        }

        // static batch
        if (batch)
        {
            StaticBatchingUtility.Combine(transform.gameObject);
        }
    }
}