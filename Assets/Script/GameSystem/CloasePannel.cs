using UnityEngine;

public class CloasePannel : MonoBehaviour
{
    public GameObject _targetPannel;

    public void Cloase()
    {
        if(_targetPannel != null)
        {
            _targetPannel.SetActive(false);
        }
    }
}
