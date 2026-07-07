using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void Exit()
    {

        Debug.Log("ゲーム終了");

        // ビルド後に終了
        Application.Quit();

        // Unityエディタ再生中の場合は停止
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }
}
