using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIDragIcon : MonoBehaviour
{
    public static UIDragIcon Instance;

    public Image icon;

    private void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    public void Show(Sprite sprite)
    {
        icon.sprite = sprite;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnCorsol(InputAction.CallbackContext context)
    {
            transform.position = context.ReadValue<Vector2>();
    }
}
