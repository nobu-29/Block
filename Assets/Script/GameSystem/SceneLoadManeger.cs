using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManeger : MonoBehaviour
{
    public void SceneGo(string SceneName)
    {
        SceneManager.LoadScene(SceneName);
    }
}
