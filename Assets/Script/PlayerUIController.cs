using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public GameObject openPannel;
    public void OpenUI(InputAction.CallbackContext context)
    {
        openPannel.SetActive(!openPannel.activeSelf);
    }
}
