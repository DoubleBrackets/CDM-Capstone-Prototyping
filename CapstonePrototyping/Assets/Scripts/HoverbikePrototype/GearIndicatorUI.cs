using UnityEngine;
using UnityEngine.UI;

public class GearIndicatorUI : MonoBehaviour
{
    [SerializeField]
    private FloatyBoi floatyBoi;

    [SerializeField]
    private Slider gearProgressBar;

    private void Update()
    {
        gearProgressBar.normalizedValue = floatyBoi.Gear;
    }
}