using UnityEngine;

public class MouseControlledCamera : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float mouseSensitivity = 3.0f;
    public float smoothTime = 0.1f;
    public LayerMask collisionLayers;

    private float currentRotationX = 0.0f;
    private float currentRotationY = 30.0f;
    private Vector3 currentVelocity;
    private Vector3 desiredPosition;

    private float minVerticalAngle = -20.0f;
    private float maxVerticalAngle = 80.0f;

    public bool followBallDirection = true;
    public float followSmoothness = 2.0f;
    private Vector3 lastBallPosition;
    private Vector3 ballDirection;
    private float targetRotationX;
    private bool isBallMoving = false;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("�� ������ ������� ������ ��� ������!");
            return;
        }
        lastBallPosition = target.position;
    }

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 currentBallPosition = target.position;
        isBallMoving = (currentBallPosition - lastBallPosition).magnitude > 0.01f;

        if (followBallDirection && isBallMoving)
        {
            ballDirection = (currentBallPosition - lastBallPosition).normalized;
            targetRotationX = Mathf.Atan2(-ballDirection.x, -ballDirection.z) * Mathf.Rad2Deg;
        }
        lastBallPosition = currentBallPosition;

        if (followBallDirection && isBallMoving)
        {
            currentRotationX = Mathf.LerpAngle(currentRotationX, targetRotationX, followSmoothness * Time.deltaTime);
        }

        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            currentRotationX += mouseX;
            currentRotationY -= mouseY;
            currentRotationY = Mathf.Clamp(currentRotationY, minVerticalAngle, maxVerticalAngle);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            distance = Mathf.Clamp(distance - scroll * 5.0f, minDistance, maxDistance);
        }

        CalculateDesiredPosition();
        HandleCameraCollisions();

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);
        transform.LookAt(target.position);
    }

    void CalculateDesiredPosition()
    {
        float horizontalRadians = currentRotationX * Mathf.Deg2Rad;
        float verticalRadians = currentRotationY * Mathf.Deg2Rad;

        float horizontalDistance = distance * Mathf.Cos(verticalRadians);
        float verticalDistance = distance * Mathf.Sin(verticalRadians);

        desiredPosition = new Vector3(
            target.position.x + horizontalDistance * Mathf.Sin(horizontalRadians),
            target.position.y + verticalDistance,
            target.position.z + horizontalDistance * Mathf.Cos(horizontalRadians)
        );
    }

    void HandleCameraCollisions()
    {
        RaycastHit hit;
        Vector3 directionToCamera = desiredPosition - target.position;

        if (Physics.SphereCast(target.position, 0.3f, directionToCamera.normalized,
                              out hit, directionToCamera.magnitude, collisionLayers))
        {
            float distanceToHit = hit.distance;
            desiredPosition = target.position + directionToCamera.normalized * (distanceToHit - 0.1f);
        }
    }
}