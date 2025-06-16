using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Canvases")]
    public GameObject gameCanvas;
    public GameObject pauseCanvas;
    public GameObject winCanvas;

    private bool isGamePaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeUI();
    }

    void InitializeUI()
    {
        if (gameCanvas != null) gameCanvas.SetActive(true);
        if (pauseCanvas != null) pauseCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(false);

        Time.timeScale = 1f;
    }

    public void TogglePause()
    {
        isGamePaused = !isGamePaused;
        Time.timeScale = isGamePaused ? 0f : 1f;

        if (gameCanvas != null) gameCanvas.SetActive(!isGamePaused);
        if (pauseCanvas != null) pauseCanvas.SetActive(isGamePaused);
    }

    public void ShowWinScreen()
    {
        Time.timeScale = 0f;

        if (gameCanvas != null) gameCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void QuitToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}