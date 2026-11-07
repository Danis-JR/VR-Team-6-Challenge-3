using UnityEngine;

public class ModelDisplacement : MonoBehaviour
{
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform mainCam;

    private Vector3 initialCameraLocalPosition;
    private Vector3 initialCharacterLocalPosition;

    [Header("Position")]
    [SerializeField] private float followSpeed = 10f;

    [Header("Rotation")]
    [SerializeField] private float rotationThreshold = 45f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float rotationDeadZone = 5f;


    // Much of this script found through resources online

    private void Start()
    {
        initialCameraLocalPosition =
            xrOrigin.InverseTransformPoint(mainCam.position);

        initialCharacterLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        //update character on late update
        FollowPosition();
        FollowRotation();
    }

    private void FollowPosition()
    {
        //copy x and zz
        Vector3 cameraLocalPosition =
            xrOrigin.InverseTransformPoint(mainCam.position);

        Vector3 displacement =
            cameraLocalPosition - initialCameraLocalPosition;

        // lock y
        displacement.y = 0f;


        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            initialCharacterLocalPosition + displacement,
            followSpeed * Time.deltaTime
        );
    }

    private void FollowRotation()
    {
        Vector3 cameraForward = mainCam.forward;
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude < 0.001f)
            return;

        cameraForward.Normalize();

        Vector3 bodyForward = transform.forward;
        bodyForward.y = 0f;
        bodyForward.Normalize();

        float angle = Vector3.SignedAngle(
            bodyForward,
            cameraForward,
            Vector3.up
        );

        float absAngle = Mathf.Abs(angle);

        // Don't rotate while inside the dead zone.
        if (absAngle <= rotationThreshold + rotationDeadZone)
            return;

        // Amount past the threshold.
        float excessAngle = absAngle - rotationThreshold;

        // Convert the excess angle into a rotation speed.
        float rotationAmount =
            Mathf.Sign(angle) *
            excessAngle *
            rotationSpeed *
            Time.deltaTime;

        transform.Rotate(0f, rotationAmount, 0f);
    }


}
