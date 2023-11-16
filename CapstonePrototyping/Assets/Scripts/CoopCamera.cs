using UnityEngine;

public class CoopCamera : MonoBehaviour
{
    [field: SerializeField]
    public Camera Cam { get; private set; }

    [field: SerializeField]
    public GameObject cameraContainer { get; private set; }

    public int PlayerNumber { get; set; }

    public void Initialize(int playerNumber)
    {
        PlayerNumber = playerNumber;
        CameraSplitScreenManager.Instance.AddCamera(this);
        Cam.cullingMask |= 1 << (32 - playerNumber);
        cameraContainer.gameObject.layer = 32 - playerNumber;
    }

    private void OnDestroy()
    {
        CameraSplitScreenManager.Instance.RemoveCamera(this);
    }
}