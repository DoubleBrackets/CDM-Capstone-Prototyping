using UnityEngine;

public class BeaconCompass : MonoBehaviour
{
    [SerializeField]
    private Transform currentPosReference;

    [SerializeField]
    private Beacon targetBeacon;

    [SerializeField]
    private Transform compassTransform;

    private void Update()
    {
        var pos = currentPosReference.position;
        var dir = targetBeacon.transform.position - pos;

        var angle = Vector3.SignedAngle(dir, currentPosReference.forward, Vector3.up);

        compassTransform.localRotation = Quaternion.Lerp(
            compassTransform.localRotation,
            Quaternion.Euler(0, 0, 180 + angle),
            1 - Mathf.Pow(0.01f, Time.deltaTime));
    }
}