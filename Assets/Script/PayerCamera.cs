using UnityEngine;
using UnityEngine.InputSystem;

public class PayerCamera : MonoBehaviour
{
    public Transform playerBody;
    public Transform cameraHolder;
   // public Camera _playerCamera;
    public float sensitivityMouse = 10f;
    public float sensitivityGamepad = 100f;

    private Vector2 lookInput;
    private float xRotation = 0f;

    private void LateUpdate()
    {
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();

        var device = context.control.device;

        if(device is Mouse)
        {
            lookInput *= sensitivityMouse;
        }
        else if(device is Gamepad)
        {
            Vector2 stick = lookInput;

            if(stick.magnitude < 0.1f)
                stick = Vector2.zero;

            lookInput = stick * sensitivityGamepad * Time.deltaTime;
        }
    }
}
