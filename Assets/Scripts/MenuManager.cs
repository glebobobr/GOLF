using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameMenuManager : MonoBehaviour
{
    [Header("Canvases")]
    public GameObject mainCanvas;
    public GameObject levelsCanvas;

    [Header("UI Elements")]
    public Button resolutionButton;
    public Button displayModeButton;
    public Button playButton;
    public Button exitButton;
    public TMP_Text resolutionText;
    public TMP_Text displayModeText;

    [Header("Level Buttons (1-10)")]
    public Button[] levelButtons;

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
        resolutionButton.onClick.AddListener(CycleResolution);
        displayModeButton.onClick.AddListener(ToggleDisplayMode);
        exitButton.onClick.AddListener(ExitGame);
        playButton.onClick.RemoveAllListeners();
        playButton.onClick.AddListener(ShowLevelsMenu);
        mainCanvas.SetActive(true);
        levelsCanvas.SetActive(false);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelIndex = i + 1;
            levelButtons[i].onClick.RemoveAllListeners();
            levelButtons[i].onClick.AddListener(() => LoadLevel(levelIndex));
        }

        LoadSettings();
        UpdateUI();
    }

    void ShowLevelsMenu()
    {
        mainCanvas.SetActive(false);
        levelsCanvas.SetActive(true);
    }

    void LoadLevel(int levelIndex)
    {
        SceneManager.LoadScene("level" + levelIndex);
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
        Vector2Int res = resolutionOptions[currentResolutionIndex].resolution;
        Screen.SetResolution(res.x, res.y, isFullScreen);
    }

    void UpdateUI()
    {
        if (resolutionText) resolutionText.text = resolutionOptions[currentResolutionIndex].displayName;
        if (displayModeText) displayModeText.text = isFullScreen ? "Fullscreen" : "Windowed";
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
            isFullScreen = PlayerPrefs.GetInt("FullScreen") == 1;

        ApplyResolution();
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