using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void OpenUI(InputAction.CallbackContext context,GameObject openPannel)
    {
        openPannel.SetActive(!openPannel.activeSelf);
    }
}
