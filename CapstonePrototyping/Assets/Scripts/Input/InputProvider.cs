using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public abstract class InputProvider : MonoBehaviour
{
    public int PlayerNumber { get; private set; }

    protected InputUser user;
    protected IInputActionCollection inputMap;

    public UnityEvent<int> OnSetup;
    public UnityEvent<bool> OnIsMouseChange;

    public void SetupNewPlayerInput(int playerNumber, InputDevice[] boundDevices)
    {
        // New instance of the action map
        PlayerNumber = playerNumber;
        inputMap = CreateMap();
        inputMap.Enable();

        if (!user.valid)
        {
            user = InputUser.PerformPairingWithDevice(boundDevices[0]);
        }
        else
        {
            InputUser.PerformPairingWithDevice(boundDevices[0], user);
        }

        for (var i = 1; i < boundDevices.Length; i++)
        {
            InputUser.PerformPairingWithDevice(boundDevices[i], user);
        }

        user.AssociateActionsWithUser(inputMap);

        var scheme = InputControlScheme.FindControlSchemeForDevice(user.pairedDevices[0], user.actions.controlSchemes);
        if (scheme != null)
        {
            user.ActivateControlScheme(scheme.Value);
        }

        SetupMap();

        OnIsMouseChange?.Invoke(boundDevices[0] is Keyboard || boundDevices[0] is Mouse);

        OnSetup?.Invoke(playerNumber);
    }

    protected abstract IInputActionCollection CreateMap();

    protected abstract void SetupMap();
}