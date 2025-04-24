using UnityEngine;

public class GolfAim : MonoBehaviour
{
    [Header("Объекты прицела")]
    public GameObject aimCircle;
    public GameObject aimArrow;
    public SpriteRenderer arrowRenderer;

    [Header("Параметры силы удара")]
    public float maxPower = 10f;

    [Header("Градиент цвета стрелки")]
    public Gradient powerGradient;
    
    [Header("Скорость вращения прицела")]
    public float rotationSpeed = 60f;

    private Vector3 startPoint;
    private bool isAiming = false;
    private Rigidbody rb;
    private float circleRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        aimCircle.SetActive(false);
        aimArrow.SetActive(false);
    }

    void Update()
    {
        //если мяч почти не двигается — можно целиться
        if (rb.velocity.magnitude < 0.01f)
        {
            //показываем прицел, когда мяч остановился
            if (!aimCircle.activeSelf && !isAiming)
            {
                aimCircle.SetActive(true);
            }
            
            if (aimCircle.activeSelf)
            {
                aimCircle.transform.position = transform.position;
                circleRotation += rotationSpeed * Time.deltaTime;
                
                // Сначала устанавливаем прицел параллельно экрану
                aimCircle.transform.rotation = Quaternion.LookRotation(
                    Camera.main.transform.forward, 
                    Camera.main.transform.up);
                
                // Затем применяем вращение вокруг оси, направленной к камере
                aimCircle.transform.Rotate(Vector3.forward, circleRotation, Space.Self);
            }

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
            
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(new Vector3(currentPoint.x, currentPoint.y, Camera.main.transform.position.y - transform.position.y));
            Vector3 aimDir = (transform.position - worldPoint).normalized;

            aimArrow.transform.position = transform.position;
            float angle = Mathf.Atan2(aimDir.z, aimDir.x) * Mathf.Rad2Deg - 90f;
            
            aimArrow.transform.rotation = Quaternion.Euler(90, 0, angle);
            
            float normalizedPower = power / maxPower;
            arrowRenderer.color = powerGradient.Evaluate(normalizedPower);
            
            aimArrow.transform.localScale = new Vector3(1, 1 + normalizedPower * 2f, 1);
            
            if (Input.GetMouseButtonUp(0))
            {
                rb.AddForce(aimDir * power * 500, ForceMode.Impulse);
                isAiming = false;
                aimArrow.SetActive(false);
                // Прицел автоматически появится снова, когда мяч остановится
            }
        }
    }
}