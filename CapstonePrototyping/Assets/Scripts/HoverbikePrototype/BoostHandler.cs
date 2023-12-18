using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoostHandler : MonoBehaviour
{
    [Header("Dependencies"), SerializeField]
    private HoverbikeInputProvider player1InputProvider;

    [SerializeField] private HoverbikeInputProvider player2InputProvider;

    [SerializeField]
    private FloatyBoi floatyBoi;

    [Header("Stats"), SerializeField]
    private float boostCooldown;

    [SerializeField] private float boostSyncDuration;

    [Header("UI"), SerializeField]
    private Slider boostBarUI;

    [SerializeField]
    private TMP_Text boostBarText;

    private float boostSyncTimer;
    private float boostCooldownTimer;

    private int firstBoosted;

    private enum State
    {
        WaitingBoostInitialize,
        WaitingBoostComplete,
        BoostCooldown
    }

    private State currentState = State.WaitingBoostInitialize;

    private void Awake()
    {
        player1InputProvider.OnBoostDown += DriverBoosted;
        player2InputProvider.OnBoostDown += NavigatorBoosted;
    }

    private void OnDestroy()
    {
        player1InputProvider.OnBoostDown -= DriverBoosted;
        player2InputProvider.OnBoostDown -= NavigatorBoosted;
    }

    private void Update()
    {
        if (currentState == State.WaitingBoostComplete)
        {
            boostSyncTimer -= Time.deltaTime;
            boostBarUI.value = boostSyncTimer / boostSyncDuration;
            if (boostSyncTimer <= 0f)
            {
                currentState = State.BoostCooldown;
                boostCooldownTimer = boostCooldown;
            }
        }

        if (currentState == State.BoostCooldown)
        {
            boostCooldownTimer -= Time.deltaTime;
            boostBarUI.value = boostCooldownTimer / boostCooldown;
            if (boostCooldownTimer <= 0f)
            {
                currentState = State.WaitingBoostInitialize;
            }
        }

        switch (currentState)
        {
            case State.WaitingBoostInitialize:
                boostBarText.text = "Boost Ready";
                break;
            case State.WaitingBoostComplete:
                boostBarText.text = "Boost Sync!";
                break;
            case State.BoostCooldown:
                boostBarText.text = "Boost Recharging";
                break;
        }
    }

    private void NavigatorBoosted()
    {
        HandleBoostInput(1);
    }

    private void DriverBoosted()
    {
        HandleBoostInput(0);
    }

    private void HandleBoostInput(int id)
    {
        if (currentState == State.WaitingBoostComplete)
        {
            if (firstBoosted == id)
            {
                return;
            }

            floatyBoi.PerformBoost();
            boostCooldownTimer = boostCooldown;
            currentState = State.BoostCooldown;
        }
        else if (currentState == State.WaitingBoostInitialize)
        {
            currentState = State.WaitingBoostComplete;
            boostSyncTimer = boostSyncDuration;
            firstBoosted = id;
        }
    }
}