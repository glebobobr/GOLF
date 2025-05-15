using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public void LoadTestScene()
    {
        SceneManager.LoadScene("testScene");
    }
}
