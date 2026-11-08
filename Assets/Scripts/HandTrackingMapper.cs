using UnityEngine;

public class HandTrackingMapper : MonoBehaviour
{
    [System.Serializable]
    public struct FingerJointPair
    {
        public Transform modelBone;     // The actual model's finger bone
        public Transform trackingSource; // The data stream/target from your hand tracker
    }

    [Header("Hand Setup")]
    // Assign these in the Unity Inspector by locking the inspector and dragging pairs in
    public FingerJointPair[] fingerJoints;

    // LateUpdate runs AFTER the Animator finishes, overriding its forced default poses
    void LateUpdate()
    {
        if (fingerJoints == null || fingerJoints.Length == 0) return;

        // Loop through every finger joint assigned in the inspector
        for (int i = 0; i < fingerJoints.Length; i++)
        {
            FingerJointPair joint = fingerJoints[i];

            if (joint.modelBone != null && joint.trackingSource != null)
            {
                // Overwrite the animator's frame data with tracking rotation
                joint.modelBone.localRotation = joint.trackingSource.localRotation;
                
                // Optional: Uncomment below if your tracker also handles finger stretching/positioning
                // joint.modelBone.localPosition = joint.trackingSource.localPosition;
            }
        }
    }
}