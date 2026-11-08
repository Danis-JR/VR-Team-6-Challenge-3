using UnityEngine;
using UnityEngine.XR.Hands;

public class JointMapper : MonoBehaviour
{
    [Header("Hand Selection")]
    public bool isLeftHand = true;

    [Header("Character Model Wrist Transform")]
    [Tooltip("Drag the character model's actual Wrist/Hand bone here.")]
    public Transform modelWrist;

    [Header("All Finger Bones (Treated Identically)")]
    public Transform thumb1;  public Transform thumb2;  public Transform thumb3;
    public Transform index1;  public Transform index2;  public Transform index3;
    public Transform middle1; public Transform middle2; public Transform middle3;
    public Transform ring1;   public Transform ring2;   public Transform ring3;
    public Transform little1; public Transform little2; public Transform little3;

    [Header("Wrist Alignment Fix")]
    [Tooltip("Fixes the perpendicular wrist issue.")]
    public Vector3 wristRotationOffset = new Vector3(-90f, 0f, 0f);

    [Header("Live Correction Sliders")]
    [Tooltip("Adjust this to straighten the minor fingertip skewness. Tweak by 5 or 10 degrees at a time.")]
    public Vector3 fingerSkewCorrection = new Vector3(0f, 0f, 0f);

    [Tooltip("Use this to flip the thumb upright if it points down. Try typing 180 or 90 on the axis that flips it up.")]
    public Vector3 thumbInvertCorrection = new Vector3(15f, 0f, 0f);

    [Header("Finger Crosstalk Stabilization")]
    [Range(0f, 1f)]
    [Tooltip("Higher values force the middle/ring tips to stay straight if their base bones are straight, overriding tracking errors.")]
    public float middleRingIndependenceBias = 0.65f;

    private Transform[] _bonesArray;
    private Quaternion[] _initialModelSpaceRotations;
    private XRHandSubsystem _handSubsystem;

    void Start()
    {
        if (modelWrist == null)
        {
            Debug.LogError("Please assign the Model Wrist to stop finger warping!", this);
            return;
        }

        // Pack all 15 bones into an indexed array
        _bonesArray = new Transform[] {
            thumb1,  thumb2,  thumb3,
            index1,  index2,  index3,
            middle1, middle2, middle3,
            ring1,   ring2,   ring3,
            little1, little2, little3
        };

        _initialModelSpaceRotations = new Quaternion[_bonesArray.Length];

        // Snapshot how the fingers are rotated relative to the model's wrist *before* tracking starts
        for (int i = 0; i < _bonesArray.Length; i++)
        {
            if (_bonesArray[i] != null)
            {
                // Isolate the finger's original modeling direction relative ONLY to the wrist root
                _initialModelSpaceRotations[i] = Quaternion.Inverse(modelWrist.rotation) * _bonesArray[i].rotation;
            }
        }

        var subsystems = new System.Collections.Generic.List<XRHandSubsystem>();
        SubsystemManager.GetSubsystems(subsystems);
        if (subsystems.Count > 0) _handSubsystem = subsystems[0];
    }

    void LateUpdate()
    {
        if (_handSubsystem == null || !_handSubsystem.running || modelWrist == null) return;

        XRHand hand = isLeftHand ? _handSubsystem.leftHand : _handSubsystem.rightHand;

        if (hand.isTracked)
        {
            // 1. FIRST FIX THE WRIST ORIENTATION
            XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);
            if (wristJoint.TryGetPose(out Pose wristPose))
            {
                Quaternion rawWristRotation = wristPose.rotation;
                Quaternion correction = Quaternion.Euler(wristRotationOffset);

                Quaternion axisInversionMatrix = Quaternion.Euler(0f, 180f, 180f); 
                Quaternion liveCalculatedRotation = (rawWristRotation * axisInversionMatrix) * correction;

                Quaternion swingForwardCorrection = Quaternion.Euler(0f, 180f, 0f);
                modelWrist.rotation = swingForwardCorrection * liveCalculatedRotation;
            }

            // 2. MAP THE FINGERS Cleanly relative to our newly aligned wrist
            MapJointWorld(hand, XRHandJointID.ThumbMetacarpal, 0);
            MapJointWorld(hand, XRHandJointID.ThumbProximal, 1);
            MapJointWorld(hand, XRHandJointID.ThumbDistal, 2);

            MapJointWorld(hand, XRHandJointID.IndexProximal, 3);
            MapJointWorld(hand, XRHandJointID.IndexIntermediate, 4);
            MapJointWorld(hand, XRHandJointID.IndexDistal, 5);

            MapJointWorld(hand, XRHandJointID.MiddleProximal, 6);
            MapJointWorld(hand, XRHandJointID.MiddleIntermediate, 7);
            MapJointWorld(hand, XRHandJointID.MiddleDistal, 8);

            MapJointWorld(hand, XRHandJointID.RingProximal, 9);
            MapJointWorld(hand, XRHandJointID.RingIntermediate, 10);
            MapJointWorld(hand, XRHandJointID.RingDistal, 11);

            MapJointWorld(hand, XRHandJointID.LittleProximal, 12);
            MapJointWorld(hand, XRHandJointID.LittleIntermediate, 13);
            MapJointWorld(hand, XRHandJointID.LittleDistal, 14);
        }
    }

    private void MapJointWorld(XRHand hand, XRHandJointID jointID, int arrayIndex)
    {
        Transform modelBone = _bonesArray[arrayIndex];
        if (modelBone == null) return;

        XRHandJoint joint = hand.GetJoint(jointID);
        XRHandJoint wristJoint = hand.GetJoint(XRHandJointID.Wrist);
        
        if (joint.TryGetPose(out Pose jointPose) && wristJoint.TryGetPose(out Pose wristPose))
        {
            // 1. Isolate tracking data from the device space
            Quaternion localTrackingRot = Quaternion.Inverse(wristPose.rotation) * jointPose.rotation;

            // 2. Combine the tracking movement offset with the model's baseline layout
            Quaternion modelSpaceRot = _initialModelSpaceRotations[arrayIndex] * localTrackingRot;

            // 3. Inject it back into World space using the live model wrist's current rotation.
            Quaternion targetWorldRotation = modelWrist.rotation * modelSpaceRot;

            // 4. --- APPLY ALIGNMENT CORRECTIONS ---
            Quaternion skewFix = Quaternion.Euler(fingerSkewCorrection);
            
            if (arrayIndex >= 0 && arrayIndex <= 2)
            {
                Quaternion thumbFlip = Quaternion.Euler(thumbInvertCorrection);
                targetWorldRotation = targetWorldRotation * skewFix * thumbFlip;
            }
            else
            {
                targetWorldRotation = targetWorldRotation * skewFix;
            }

            // 5. Final bone assignments with crosstalk prevention for Middle and Ring fingertips
            if (arrayIndex == 0)
            {
                modelBone.rotation = Quaternion.Slerp(modelBone.rotation, targetWorldRotation, 0.3f);
            }
            // Array indices 8 (Middle Distal) and 11 (Ring Distal) are the fingertips experiencing crosstalk
            else if ((arrayIndex == 8 || arrayIndex == 11) && middleRingIndependenceBias > 0f)
            {
                // Find the base/proximal bone of this exact finger (Middle Proximal is 6, Ring Proximal is 9)
                Transform baseBone = _bonesArray[arrayIndex - 2];
                
                if (baseBone != null)
                {
                    // Evaluate how much the root finger bone has deviated from its start layout. 
                    // If the base bone isn't curling, the tip shouldn't be curling either!
                    Quaternion baseExpectedRot = modelWrist.rotation * _initialModelSpaceRotations[arrayIndex - 2];
                    float baseCurlDifference = Quaternion.Angle(baseBone.rotation, baseExpectedRot);

                    // If the root bone is straight (low curl angle difference), force the tip 
                    // to ignore the erroneous tracking and follow the straight base path.
                    if (baseCurlDifference < 25f) 
                    {
                        // Calculate a clean stabilized rotation mirroring the parent bone's local path
                        Transform intermediateBone = _bonesArray[arrayIndex - 1];
                        Quaternion cleanStabilizedRot = intermediateBone.rotation * (Quaternion.Inverse(_bonesArray[arrayIndex - 2].rotation) * intermediateBone.rotation);
                        
                        targetWorldRotation = Quaternion.Slerp(targetWorldRotation, cleanStabilizedRot, middleRingIndependenceBias);
                    }
                }
                
                modelBone.rotation = targetWorldRotation;
            }
            else
            {
                modelBone.rotation = targetWorldRotation;
            }
        }
    }
}