using UnityEngine;

public class GolfBallController : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Move")]
    public float moveSpeed = 5f;
    public float maxSpeed = 10f;
    public float jumpForce = 2f;

    [Header("Golf Settings")]
    public float minVelocity = 0.1f; // Минимальная скорость для определения движения
    public bool useGolfControls = false; // Переключение между режимами управления

    [Header("Settings")]
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Transform groundCheck;

    private bool isGrounded;
    private Vector3 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Если точка проверки земли не назначена, создаем ее
        if (groundCheck == null)
        {
            GameObject check = new GameObject("GroundCheck");
            check.transform.parent = transform;
            check.transform.localPosition = new Vector3(0, -0.5f, 0);
            groundCheck = check.transform;
        }
    }

    void Update()
    {
        // Проверка соприкосновения с землей
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        // Используем стандартное управление только если не в режиме гольфа
        if (!useGolfControls)
        {
            // Получаем ввод от пользователя
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            // Определяем направление движения относительно камеры
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            moveDirection = (forward * verticalInput + right * horizontalInput).normalized;

            // Прыжок
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }
    }

    void FixedUpdate()
    {
        if (!useGolfControls && moveDirection != Vector3.zero)
        {
            rb.AddForce(moveDirection * moveSpeed, ForceMode.Force);

            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            if (horizontalVelocity.magnitude > maxSpeed)
            {
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }
    }

    void Jump()
    {
        // Применяем меньшую силу прыжка
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // Методы из GolfBall

    /// <summary>
    /// Проверяет, движется ли мяч
    /// </summary>
    /// <returns>True, если мяч движется; иначе false</returns>
    public bool IsMoving()
    {
        return rb != null && rb.linearVelocity.magnitude > minVelocity;
    }

    /// <summary>
    /// Применяет силу к мячу для удара
    /// </summary>
    /// <param name="force">Вектор силы для удара</param>
    public void Hit(Vector3 force)
    {
        if (rb != null)
        {
            // Переключаемся в режим гольфа при ударе
            useGolfControls = true;

            // Сбрасываем текущую скорость перед ударом (опционально)
            // rb.velocity = Vector3.zero;

            // Применяем силу удара
            rb.AddForce(force, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// Останавливает мяч полностью
    /// </summary>
    public void Stop()
    {
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    /// <summary>
    /// Переключает режим управления мячом
    /// </summary>
    /// <param name="useGolf">True для режима гольфа, false для стандартного управления</param>
    public void SetGolfMode(bool useGolf)
    {
        useGolfControls = useGolf;
    }

    public Vector3 GetBallPosition()
    {
        return transform.position;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}