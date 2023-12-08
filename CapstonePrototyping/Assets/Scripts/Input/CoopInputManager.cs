using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class CoopInputManager : MonoBehaviour
{
    private enum CoopInputType
    {
        Existing,
        Instantiate
    }

    [SerializeField]
    private CoopInputType coopInputType;

    [SerializeField]
    private int maxPlayers = 4;

    [Header("Existing Providers"), SerializeField]
    private List<InputProvider> inputProviders;

    [Header("Instantiate"), SerializeField]
    private GameObject playerPrefab;

    private readonly List<InputProvider> playerInputProviders = new();
    private int playerCount;

    private void Start()
    {
        InputUser.listenForUnpairedDeviceActivity = maxPlayers;
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
    }

    private void OnDestroy()
    {
        InputUser.listenForUnpairedDeviceActivity = 0;
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
    }

    private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr ptr)
    {
        // Debug.Log($"Unpaired device used: {control.device.description}");
        var device = control.device;

        if (!(device is Gamepad))
        {
            return;
        }

        if (coopInputType == CoopInputType.Existing)
        {
            SetupExistingProvider(device);
        }
        else if (coopInputType == CoopInputType.Instantiate)
        {
            InstantiateNewPlayer(device);
        }
    }

    private void SetupExistingProvider(InputDevice device)
    {
        var inputProvider = inputProviders[playerCount];
        inputProvider.SetupNewPlayerInput(playerCount + 1, new[] { device });
        if (InputUser.listenForUnpairedDeviceActivity > 0)
        {
            InputUser.listenForUnpairedDeviceActivity--;
            playerCount++;
        }
    }

    private void InstantiateNewPlayer(InputDevice device)
    {
        var player = Instantiate(playerPrefab, Vector3.up * 2f, Quaternion.identity);
        var inputProvider = player.GetComponentInChildren<InputProvider>();
        if (inputProvider == null)
        {
            return;
        }

        playerInputProviders.Add(inputProvider);
        inputProvider.SetupNewPlayerInput(playerInputProviders.Count, new[] { device });

        if (InputUser.listenForUnpairedDeviceActivity > 0)
        {
            InputUser.listenForUnpairedDeviceActivity--;
            playerCount++;
        }
    }
}