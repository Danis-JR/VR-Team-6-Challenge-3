using UnityEngine;

public class ModelMovement : MonoBehaviour
{
    [SerializeField] private Transform xrOrigin;
    [SerializeField] private Transform head;


	[Header("Position")]
	[SerializeField] private float yaw_smoothing = 8F;
	[SerializeField] private float yaw_offset = 0F;
	[SerializeField] private float y_trim = 0.5F;
	[SerializeField] private float model_height = 2.5F;

    [Header("Rotation")]
    [SerializeField] private float rotationThreshold = 45f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float rotationDeadZone = 5f;

	void Awake()
	{
		if (this.head == null && Camera.main != null)
			this.head = Camera.main.transform;
	}

	void LateUpdate()
	{
		FollowPosition();
        FollowRotation();
	}

    private void FollowPosition()
    {
        Vector3 head_pos = this.head.position;
		head_pos.y = this.head.position.y - (this.model_height * 0.5F) + this.y_trim;
		this.transform.position = head_pos;

		Quaternion target = Quaternion.Euler(0F, this.head.eulerAngles.y + this.yaw_offset, 0F);
		this.transform.rotation = Quaternion.Slerp(
			this.transform.rotation, target,
			1F - Mathf.Exp(-this.yaw_smoothing * Time.deltaTime)	
		);
    }

    private void FollowRotation()
    {
        Vector3 cameraForward = head.forward;
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
