using UnityEngine;
using UnityEngine.Events;

public class HoleManager : MonoBehaviour
{
    [Header("Ball Reference")]
    public GameObject ball;

    [Header("Hole Settings")]
    public float winDelay = 1f;
    public float MaxStayTime = 2f;
    public float MaxHoleDropOffset = 0.5f;
    public GameObject holePos;

    [Header("Events")]
    public UnityEvent OnBallEnterHole;

    private bool isWinTriggered = false;
    private bool hasDropped = false;
    private float stayTimer = 0;
    private GolfBallController ballController;
    private Rigidbody ballRigidbody;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }

        if (ball != null)
        {
            ballController = ball.GetComponent<GolfBallController>();
            ballRigidbody = ball.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("Ball reference is not set in HoleManager!");
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (ball == null || other.gameObject != ball) return;
        if (!other.enabled) return;

        stayTimer += Time.deltaTime;

        Vector2 ballXZ = new Vector2(ball.transform.position.x, ball.transform.position.z);
        Vector2 holeXZ = new Vector2(transform.position.x, transform.position.z);
        float distance = Vector2.Distance(ballXZ, holeXZ);

        if (distance < MaxHoleDropOffset &&
           (ballController.IsMoving() == false || stayTimer >= MaxStayTime))
        {
            if (!hasDropped)
            {
                ball.transform.position = holePos.transform.position;
                ballController.Stop();

                if (ballRigidbody != null)
                {
                    ballRigidbody.isKinematic = true;
                }

                hasDropped = true;
            }

            if (!isWinTriggered)
            {
                isWinTriggered = true;
                OnBallEnterHole.Invoke();
                Invoke(nameof(TriggerWin), winDelay);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (ball != null && other.gameObject == ball)
        {
            stayTimer = 0;
        }
    }

    void TriggerWin()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShowWinScreen();
        }
        else
        {
            Debug.LogWarning("GameManager instance not found! Loading next level directly.");
            GameManager.Instance.LoadNextLevel();
        }
    }
}