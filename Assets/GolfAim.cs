using UnityEngine;

public class GolfAim : MonoBehaviour
{
    [Header("Объекты прицела")]
    public GameObject aimCircle;    // Круг-прицел (пунктир)
    public GameObject aimArrow;     // Стрелка удара
    public SpriteRenderer arrowRenderer; // SpriteRenderer стрелки

    [Header("Параметры силы удара")]
    public float maxPower = 10f;    // Максимальная сила удара

    [Header("Градиент цвета стрелки")]
    public Gradient powerGradient;  // Градиент для цвета стрелки

    private Vector3 startPoint;
    private bool isAiming = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aimCircle.SetActive(true);
        aimArrow.SetActive(false);
    }

    void Update()
    {
        // Если мяч почти не двигается — можно целиться
        if (rb.linearVelocity.magnitude < 0.01f)
        {
            // Вращаем круг-прицел
            if (aimCircle.activeSelf)
                aimCircle.transform.Rotate(0, 0, 60 * Time.deltaTime);

            if (Input.GetMouseButtonDown(0))
            {
                isAiming = true;
                startPoint = Input.mousePosition;
                aimCircle.SetActive(false);
                aimArrow.SetActive(true);
            }
        }
        else
        {
            aimCircle.SetActive(false);
            aimArrow.SetActive(false);
        }

        if (isAiming)
        {
            Vector3 currentPoint = Input.mousePosition;
            Vector3 dir = (startPoint - currentPoint);
            float power = Mathf.Clamp(dir.magnitude / 100f, 0, maxPower);

            // Получаем направление удара в мировых координатах
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(currentPoint.x, currentPoint.y, Camera.main.transform.position.y - transform.position.y));
            Vector3 aimDir = (transform.position - worldPoint).normalized;

            // Ставим стрелку на мяч и поворачиваем по направлению удара
            aimArrow.transform.position = transform.position;
            float angle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg - 90f;
            aimArrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            // Плавное изменение цвета через градиент
            float normalizedPower = power / maxPower;
            arrowRenderer.color = powerGradient.Evaluate(normalizedPower);

            // Масштаб стрелки по силе удара (можно скорректировать по твоим спрайтам)
            aimArrow.transform.localScale = new Vector3(1, 1 + normalizedPower * 2f, 1);

            // Отпускаем мышь — удар
            if (Input.GetMouseButtonUp(0))
            {
                rb.AddForce(aimDir * power * 500, ForceMode.Impulse);
                isAiming = false;
                aimArrow.SetActive(false);
                aimCircle.SetActive(true);
            }
        }
    }
}