using UnityEngine;

public class HoverbikeCameraController : MonoBehaviour
{
    [SerializeField]
    private HoverbikeInputProvider inputProvider;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float sensitivity;

    [SerializeField]
    private Vector2 upDownLimits;

    [SerializeField]
    private bool useRightLeftLimits;

    [SerializeField]
    private Vector2 rightLeftLimits;

    private float cameraYRot;
    private float cameraXRot;

    private void Update()
    {
        cameraYRot += inputProvider.lookInput.x * sensitivity * Time.deltaTime;
        cameraXRot += inputProvider.lookInput.y * sensitivity * Time.deltaTime;

        cameraXRot = Mathf.Clamp(cameraXRot, upDownLimits.x, upDownLimits.y);

        if (useRightLeftLimits)
        {
            cameraYRot = Mathf.Clamp(cameraYRot, rightLeftLimits.x, rightLeftLimits.y);
        }

        cameraTransform.localRotation = Quaternion.Euler(cameraXRot, cameraYRot, 0);
    }
}