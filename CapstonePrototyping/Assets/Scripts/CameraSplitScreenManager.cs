using System.Collections.Generic;
using UnityEngine;

public class CameraSplitScreenManager : MonoBehaviour
{
    public static CameraSplitScreenManager Instance { get; private set; }

    private readonly List<CoopCamera> cameras = new();

    private int displays;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Debug.Log("displays connected: " + Display.displays.Length);
    }

    public void AddCamera(CoopCamera cam)
    {
        cameras.Add(cam);
        UpdateSplitScreen();
    }

    public void RemoveCamera(CoopCamera cam)
    {
        cameras.Remove(cam);
        UpdateSplitScreen();
    }

    private void UpdateSplitScreen()
    {
        cameras.Sort((a, b) => a.PlayerNumber.CompareTo(b.PlayerNumber));

        for (var i = 0; i < cameras.Count; i++)
        {
            // Debug.Log($"{cameras[i].PlayerNumber}");
        }

        // Separate display split screen if we have enough displays for all cameras
        var availableDisplays = Display.displays.Length;
        displays = availableDisplays;
        if (availableDisplays >= cameras.Count)
        {
            // Separate display split screen
            for (var i = 0; i < cameras.Count; i++)
            {
                cameras[i].Cam.targetDisplay = i;
                if (!Display.displays[i].active)
                {
                    Display.displays[i].Activate();
                }

                cameras[i].Cam.rect = new Rect(0, 0, 1f, 1f);
            }
        }
        else
        {
            // Split screen on one display
            for (var i = 0; i < cameras.Count; i++)
            {
                cameras[i].Cam.rect = new Rect(i * 1f / cameras.Count, 0, 1f / cameras.Count, 1f);
            }
        }
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 100, 20), $"Displays: {displays}");
    }
}