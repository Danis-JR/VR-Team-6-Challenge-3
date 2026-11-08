using UnityEngine;

public class RigTargetFollower : MonoBehaviour
{
    [Header("The VR Controller/Hand tracking node from XR Origin")]
    public Transform vrTrackingSource;

    [Header("Max length of your character's 3D arm (prevents stretching)")]
    public float maxArmLength = 0.75f;
    public Transform characterShoulder;

    void LateUpdate()
    {
        if (vrTrackingSource == null) return;

        // Copy position and rotation
        Vector3 targetPosition = vrTrackingSource.position;
        Quaternion targetRotation = vrTrackingSource.rotation;

        // Safe Clamp: Prevent the arm from ripping out of the shoulder socket
        if (characterShoulder != null)
        {
            Vector3 shoulderToTarget = targetPosition - characterShoulder.position;
            if (shoulderToTarget.magnitude > maxArmLength)
            {
                targetPosition = characterShoulder.position + (shoulderToTarget.normalized * maxArmLength);
            }
        }

        transform.position = targetPosition;
        transform.rotation = targetRotation;
    }
}
