using UnityEngine;

public class CoopPlayer : MonoBehaviour
{
    [field: SerializeField]
    public CoopCamera Camera { get; private set; }

    [field: SerializeField]
    public InputProvider InputProvider { get; private set; }

    private void Awake()
    {
        InputProvider.OnSetup.AddListener(Initialize);
    }

    private void OnDestroy()
    {
        InputProvider.OnSetup.RemoveListener(Initialize);
    }

    public void Initialize(int playerNumber)
    {
        Camera.Initialize(playerNumber);
    }
}