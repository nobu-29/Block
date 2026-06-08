using UnityEngine;
using UnityEngine.InputSystem;

public class PayerCamera : MonoBehaviour
{
    public Transform playerBody;
    public Camera _playerCamera;
    public float sensitivity = 2f;

    private Vector2 lookInput;
    private float xRotation = 0f;

    private void LateUpdate()
    {
        float mouseX = lookInput.x * sensitivity;
        float mouseY = lookInput.y * sensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        _playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }

    public void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }
}
