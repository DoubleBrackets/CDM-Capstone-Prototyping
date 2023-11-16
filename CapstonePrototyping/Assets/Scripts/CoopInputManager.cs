using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class CoopInputManager : MonoBehaviour
{
    [SerializeField]
    private CoopPlayer playerPrefab;

    private readonly List<CoopPlayer> players = new();

    private readonly int maxPlayers = 4;

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
        Debug.Log($"Unpaired device used: {control.device.description}");
        var device = control.device;

        if (device is Gamepad)
        {
            var player = Instantiate(playerPrefab, Vector3.up * 2f, Quaternion.identity);
            players.Add(player);
            player.Initialize(players.Count, new[] { device });
            if (InputUser.listenForUnpairedDeviceActivity > 0)
            {
                InputUser.listenForUnpairedDeviceActivity--;
            }

            Debug.Log($"{InputUser.listenForUnpairedDeviceActivity}");
        }

        /*
        if (device is Mouse || device is Gamepad)
        {
            var player = Instantiate(playerPrefab, Vector3.up * 2f, Quaternion.identity);
            players.Add(player);
            player.Initialize(players.Count, 
                new []
                {
                    InputSystem.devices.FirstOrDefault(a => a is Mouse),
                    InputSystem.devices.FirstOrDefault(a => a is Keyboard)
                });
            InputUser.listenForUnpairedDeviceActivity--;
        }*/
    }
}