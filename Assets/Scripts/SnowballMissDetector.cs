// SnowballMissDetector.cs — Detect Missed Throws
using System.Collections;
using UnityEngine;

public class SnowballMissDetector : MonoBehaviour
{
    private GameAudioManager audioManager;
    private Rigidbody rb;
    private bool resolved = false;
    private bool trackingStarted = false;

    void Start()
    {
        audioManager = FindAnyObjectByType<GameAudioManager>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (resolved || trackingStarted || rb == null)
            return;

        // Start tracking after snowball release
        if (rb.useGravity && !rb.isKinematic)
        {
            trackingStarted = true;
            StartCoroutine(CheckMiss());
        }
    }

    public void RegisterHit()
    {
        resolved = true;
    }

    IEnumerator CheckMiss()
    {
        yield return new WaitForSeconds(5f);

        if (!resolved && audioManager != null)
        {
            resolved = true;
            audioManager.OnTargetMissed();
        }
    }
}