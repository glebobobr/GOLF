using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMenuManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resolutionButton;
    public Button displayModeButton;
    public Button playButton;
    public Button exitButton;
    public TMP_Text resolutionText;
    public TMP_Text displayModeText;

    [Header("Resolution Settings")]
    public ResolutionOption[] resolutionOptions = new ResolutionOption[]
    {
        new ResolutionOption(new Vector2Int(640, 480), "VGA (640x480)"),
        new ResolutionOption(new Vector2Int(800, 600), "SVGA (800x600)"),
        new ResolutionOption(new Vector2Int(1024, 768), "XGA (1024x768)"),
        new ResolutionOption(new Vector2Int(1280, 720), "HD (1280x720)"),
        new ResolutionOption(new Vector2Int(1366, 768), "WXGA (1366x768)"),
        new ResolutionOption(new Vector2Int(1600, 900), "HD+ (1600x900)"),
        new ResolutionOption(new Vector2Int(1920, 1080), "Full HD (1920x1080)"),
        new ResolutionOption(new Vector2Int(2560, 1440), "2K (2560x1440)")
    };

    private int currentResolutionIndex = 0;
    private bool isFullScreen = true;

    void Start()
    {
        //инициализация кнопок
        resolutionButton.onClick.AddListener(CycleResolution);
        displayModeButton.onClick.AddListener(ToggleDisplayMode);
        playButton.onClick.AddListener(LoadTestScene);
        exitButton.onClick.AddListener(ExitGame);

        LoadSettings();
        UpdateUI();
    }

    void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void CycleResolution()
    {
        currentResolutionIndex = (currentResolutionIndex + 1) % resolutionOptions.Length;
        ApplyResolution();
        SaveSettings();
        UpdateUI();
    }

    void ToggleDisplayMode()
    {
        isFullScreen = !isFullScreen;
        ApplyResolution();
        SaveSettings();
        UpdateUI();
    }

    void ApplyResolution()
    {
        Vector2Int resolution = resolutionOptions[currentResolutionIndex].resolution;
        Screen.SetResolution(resolution.x, resolution.y, isFullScreen);
    }

    void UpdateUI()
    {
        if (resolutionText != null)
        {
            resolutionText.text = resolutionOptions[currentResolutionIndex].displayName;
        }

        if (displayModeText != null)
        {
            displayModeText.text = isFullScreen ? "Fullscreen" : "Windowed";
        }
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("ResolutionIndex", currentResolutionIndex);
        PlayerPrefs.SetInt("FullScreen", isFullScreen ? 1 : 0);
        PlayerPrefs.Save();
    }

    void LoadSettings()
    {
        if (PlayerPrefs.HasKey("ResolutionIndex"))
        {
            currentResolutionIndex = PlayerPrefs.GetInt("ResolutionIndex");
            currentResolutionIndex = Mathf.Clamp(currentResolutionIndex, 0, resolutionOptions.Length - 1);
        }

        if (PlayerPrefs.HasKey("FullScreen"))
        {
            isFullScreen = PlayerPrefs.GetInt("FullScreen") == 1;
        }

        ApplyResolution();
    }

    public void LoadTestScene()
    {
        SceneManager.LoadScene("testScene");
    }
}

[System.Serializable]
public class ResolutionOption
{
    public Vector2Int resolution;
    public string displayName;

    public ResolutionOption(Vector2Int res, string name)
    {
        resolution = res;
        displayName = name;
    }
}
