using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMG : MonoBehaviour
{
    public PlayerControler _player;
    public PayerCamera _playerCamera;
    public GameObject _StartPannel;
    public GameObject _ScorePannel;
    public Text _CountText;
    public Text _ScoreText;
    public float _timeMax;

    public SceneLoadManeger _Load;
    public string _SceneName;
    public float _LoadSeconds;

    private Vector3 _startPosition;

    private void Start()
    {
        _player.enabled = false;
        _playerCamera.enabled = false;

        if (_ScorePannel.activeSelf)
            _ScorePannel.SetActive(false);
        
        _StartPannel.SetActive(true);
        StartCoroutine(HideStartPanel());
    }

    IEnumerator HideStartPanel()
    {
        yield return new WaitForSeconds(3f);
        _StartPannel.SetActive(false);

        StartCoroutine(TimerStart(_timeMax));
    }

    IEnumerator TimerStart(float timeSeconds)
    {
        _CountText.gameObject.SetActive(true);
        for(int x = 1; x <= 3; x++)
        {
            _CountText.text = x.ToString();
            yield return new WaitForSeconds(1f);
        }
        _CountText.text = "GO";
        yield return new WaitForSeconds(1f);

        _player.enabled = true;
        _playerCamera.enabled = true;

        _startPosition = _player.transform.position;

        float time = timeSeconds;

        while (time > 0f)
        {
            time -= Time.deltaTime;
            _CountText.text = time.ToString("F1"); // 小数1桁表示
            yield return null; // 毎フレーム更新
        }

        _CountText.text = "Finish!";
        yield return null;

        _player.enabled = false;
        _playerCamera.enabled = false;

        yield return new WaitForSeconds(0.7f);
        _ScorePannel.SetActive(true);

        // 現在位置
        Vector3 currentPosition = _player.transform.position;

        // 距離計算（XZ平面だけなら y を無視）
        float distance = Vector3.Distance(_startPosition, currentPosition);

        // 表示
        Debug.Log("進んだ距離: " + distance);
        _ScoreText.text = distance.ToString("F2") + "m";

        yield return new WaitForSeconds(_LoadSeconds);

        _Load.SceneGo(_SceneName);
    }
}
