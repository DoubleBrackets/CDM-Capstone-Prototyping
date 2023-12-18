using UnityEngine;

public class EnvironmentalObstacle : MonoBehaviour
{
    public float heightOffset;

    [SerializeField]
    private Transform container;

    [SerializeField]
    private Vector2 yRotRange;

    [SerializeField]
    private Vector2 xRotRange;

    [SerializeField]
    private Vector2 zRotRange;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * heightOffset);
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    public void PlaceSelfWithRandomOrientation(Vector3 location)
    {
        transform.position = location;

        // Random rotation
        var rot = Quaternion.Euler(
            Random.Range(xRotRange.x, xRotRange.y),
            Random.Range(yRotRange.x, yRotRange.y),
            Random.Range(zRotRange.x, zRotRange.y)
        );

        container.localRotation = rot;
        transform.position += Vector3.up * heightOffset;

        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
}