using UnityEngine;

public class MouseControlledCamera : MonoBehaviour
{
    public Transform target; // Ссылка на мяч
    public float distance = 5.0f; // Расстояние от мяча
    public float minDistance = 2.0f;
    public float maxDistance = 10.0f;

    public float mouseSensitivity = 3.0f; // Чувствительность мыши
    public float smoothTime = 0.1f; // Время сглаживания
    public LayerMask collisionLayers; // Слои для проверки столкновений

    private float currentRotationX = 0.0f;
    private float currentRotationY = 30.0f; // Начальный угол наклона
    private Vector3 currentVelocity;
    private Vector3 desiredPosition;

    // Ограничения для вертикального угла
    private float minVerticalAngle = -20.0f;
    private float maxVerticalAngle = 80.0f;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Не указан целевой объект для камеры!");
            return;
        }

        // Скрыть и заблокировать курсор (опционально)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Управление мышью (при зажатой правой кнопке мыши)
        if (Input.GetMouseButton(1)) // Правая кнопка мыши
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            currentRotationX += mouseX;
            currentRotationY -= mouseY; // Инвертируем для более естественного управления

            // Ограничиваем вертикальный угол
            currentRotationY = Mathf.Clamp(currentRotationY, minVerticalAngle, maxVerticalAngle);
        }

        // Изменение расстояния колесиком мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            distance = Mathf.Clamp(distance - scroll * 5.0f, minDistance, maxDistance);
        }

        // Вычисляем позицию камеры
        CalculateDesiredPosition();

        // Проверка столкновений
        HandleCameraCollisions();

        // Плавное перемещение камеры
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, smoothTime);

        // Направляем камеру точно на мяч
        transform.LookAt(target.position);
    }

    void CalculateDesiredPosition()
    {
        // Конвертируем углы в радианы
        float horizontalRadians = currentRotationX * Mathf.Deg2Rad;
        float verticalRadians = currentRotationY * Mathf.Deg2Rad;

        // Вычисляем позицию с учетом обоих углов
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