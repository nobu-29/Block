using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class PayerCamera : MonoBehaviour
{
    public Transform playerBody;
    public Transform cameraHolder;
    public Camera _playerCamera;
    public float sensitivityMouse = 10f;
    public float sensitivityGamepad = 100f;

    public BlockWorld world;

    private Color normalFogColor;
    private float normalFogDensity;

    private Vector2 lookInput;
    private float xRotation = 0f;

    private void Start()
    {
        normalFogColor = RenderSettings.fogColor;
        normalFogDensity = RenderSettings.fogDensity;
    }

    private void LateUpdate()
    {
        float mouseX = lookInput.x;
        float mouseY = lookInput.y;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraHolder.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerBody.Rotate(Vector3.up, mouseX, Space.Self);

        UpdateUnderwaterEffect();
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

            lookInput = stick * sensitivityGamepad;
        }
    }

    void UpdateUnderwaterEffect()
    {
        Vector3Int cameraPos = Vector3Int.FloorToInt(cameraHolder.position);
        int blockID = world.GetBlock(cameraPos);
        bool isUnderwater = blockID == 8 || (blockID >= 800 && blockID <= 806);

        if (isUnderwater)
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0f, 0.25f, 0.5f);
            RenderSettings.fogDensity = 0.08f;

            _playerCamera.backgroundColor = new Color(0f, 0.25f, 0.5f);
        }
        else
        {
            RenderSettings.fog = true;
            RenderSettings.fogColor = normalFogColor;
            RenderSettings.fogDensity = normalFogDensity;
        }
    }
}
