using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void SwitchToScene(int sceneId)
    {
        SceneManager.LoadScene(sceneId);
    }
}
