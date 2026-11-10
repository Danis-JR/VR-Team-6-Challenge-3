// SnowballTargetAudio.cs — Count Each Snowball Only Once
using UnityEngine;
using System.Collections.Generic;

public class SnowballTargetAudio : MonoBehaviour
{
    private SnowballAudioBridge audioBridge;

    // Shared across all targets to prevent duplicate hits
    private static readonly HashSet<EntityId> countedSnowballs = new HashSet<EntityId>();

    void Start()
    {
        audioBridge = FindAnyObjectByType<SnowballAudioBridge>();    
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Snowball")) return;

        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        EntityId snowballID = rb.gameObject.GetEntityId();

        // Ignore snowballs already counted by either target
        if (!countedSnowballs.Add(snowballID)) return;

    // Step 15 — Mark snowball as hit
    SnowballMissDetector detector =
        rb.GetComponent<SnowballMissDetector>();

    if (detector != null)
        detector.RegisterHit();

    audioBridge?.OnTargetHit();
    }
}