using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraSplitScreenManager: MonoBehaviour
{
    public static CameraSplitScreenManager Instance { get; private set; }

    private List<CoopCamera> cameras = new();

    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        Debug.Log ("displays connected: " + Display.displays.Length);
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
        cameras.Sort((a,b) => a.PlayerNumber.CompareTo(b.PlayerNumber));

        for (int i = 0; i < cameras.Count; i++)
        {
            Debug.Log($"{cameras[i].PlayerNumber}");
        }
        
        // Separate display split screen if we have enough displays for all cameras
        int availableDisplays = Display.displays.Length;
        if (availableDisplays >= cameras.Count)
        {
            // Separate display split screen
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].Cam.targetDisplay = i;
                cameras[i].Cam.rect = new Rect(0, 0, 1f, 1f);
            }
        }
        else
        {
            // Split screen on one display
            for (int i = 0; i < cameras.Count; i++)
            {
                cameras[i].Cam.rect = new Rect(i * 1f / cameras.Count, 0, 1f / cameras.Count, 1f);
            }
        }
    }
}