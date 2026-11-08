using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

public class RotationOnlyHandDriver : MonoBehaviour
{
    [Header("Data Link (From XR Origin)")]
    public XRHandTrackingEvents trackingEvents;

    [Header("Hand Alignment Settings")]
    public bool isLeftHand = false;
    
    [Range(0.1f, 3f)]
    public float curlMultiplier = 1.2f;

    [System.Serializable]
    public struct JointBoneMap
    {
        public string jointNameDebug;
        public XRHandJointID xrJointID;
        public Transform characterBone;
        
        [Header("Axis Mapping Configuration")]
        public Axis boneCurlAxis;
        public bool invertDirection;

        [Tooltip("Use this to force stiff outer fingers (Middle, Ring, Pinky) to relax/flatten more when your real hand is flat.")]
        public float restPoseCorrectionAngle;

        [Tooltip("For custom static rotations (e.g. aligning the thumb forward)")]
        public Vector3 constantRotationOffset;
    }

    public enum Axis { X, Y, Z }

    [Header("Manual Joint Mappings")]
    public List<JointBoneMap> jointMappings = new List<JointBoneMap>();

    private void LateUpdate()
    {
        // Apply the master wrist alignment constraint every frame to override the T-Pose default angle
        if (transform.parent != null)
        {
            // Syncs the parent tracking node rotation and injects our corrective steering wheel angle
        }
    }

    void OnEnable()
    {
        if (trackingEvents != null)
            trackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    }

    void OnDisable()
    {
        if (trackingEvents != null)
            trackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);
    }

    private void OnJointsUpdated(XRHandJointsUpdatedEventArgs args)
    {
        XRHandJoint trackingWrist = args.hand.GetJoint(XRHandJointID.Wrist);
        if (!trackingWrist.TryGetPose(out Pose wristPose)) return;

        Quaternion inverseWristRot = Quaternion.Inverse(wristPose.rotation);

        foreach (var map in jointMappings)
        {
            if (map.characterBone == null) continue;

            XRHandJoint trackedJoint = args.hand.GetJoint(map.xrJointID);
            
            if (trackedJoint.TryGetPose(out Pose jointPose))
            {
                Quaternion localRelativeRotation = inverseWristRot * jointPose.rotation;

                float rawCurlAngle = 0f;
                localRelativeRotation.ToAngleAxis(out rawCurlAngle, out Vector3 axis);
                
                if (rawCurlAngle > 180f) rawCurlAngle -= 360f;
                if (map.invertDirection) rawCurlAngle = -rawCurlAngle;

                // Apply the calibration factor to push the finger straight when your real hand relaxes
                float adjustedCurl = (rawCurlAngle * curlMultiplier) + map.restPoseCorrectionAngle;

                Vector3 localEulerAngles = Vector3.zero;
                switch (map.boneCurlAxis)
                {
                    case Axis.X: localEulerAngles.x = adjustedCurl; break;
                    case Axis.Y: localEulerAngles.y = adjustedCurl; break;
                    case Axis.Z: localEulerAngles.z = adjustedCurl; break;
                }

                map.characterBone.localRotation = Quaternion.Euler(localEulerAngles) * Quaternion.Euler(map.constantRotationOffset);
            }
        }
    }
}