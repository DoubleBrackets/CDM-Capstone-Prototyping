using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ObstacleSpawnGroup", fileName = "ObstacleSpawnGroup")]
public class EnvironmentSpawnGroup : ScriptableObject
{
    [field: SerializeField] public List<EnvironmentalObstacle> PotentialObstacles { get; set; }

    [field: SerializeField] public int SpawnCount { get; set; }
}