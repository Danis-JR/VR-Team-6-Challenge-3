
 // SnowballAudioBridge.cs — Snowball Spawn and Target Hit Audio
using UnityEngine;
using System.Collections.Generic;

// Detect snowball spawning from either hand
public class SnowballAudioBridge : MonoBehaviour
{
    [Header("Audio Manager")]
    public GameAudioManager audioManager;

    [Header("Hand Trackers")]
    public GestureTracker leftHand;
    public GestureTracker rightHand;

    private bool snowballDetected = false;

    void Update()
    {
        // Start Hero music when the first snowball spawns
        if (snowballDetected) return;

        bool leftSpawned = leftHand != null && leftHand.isCurledActive;
        bool rightSpawned = rightHand != null && rightHand.isCurledActive;

        if (leftSpawned || rightSpawned)
        {
            snowballDetected = true;
            audioManager?.OnPalmOpened();
        }
    }

    // Forward successful hits to GameAudioManager
    public void OnTargetHit()
    {
        if (audioManager == null) return;

        audioManager.OnTargetHit();
    }
}

