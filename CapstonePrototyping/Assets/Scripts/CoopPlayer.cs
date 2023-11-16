using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CoopPlayer : MonoBehaviour
{
    [field: SerializeField]
    public CoopCamera Camera { get; private set; }
    
    [field: SerializeField]
    public CoopInputProvider InputProvider { get; private set; }

    public void Initialize(int playerNumber, InputDevice[] device)
    {
        Camera.Initialize(playerNumber);
        InputProvider.SetupNewPlayerInput(playerNumber, device);
    }
}
