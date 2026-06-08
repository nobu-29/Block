using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControler : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float MoveBoost = 2f;
    public float JumpPower = 1.5f;
    public Camera _camera;
    public Rigidbody _rb;

    [SerializeField] private bool _isDash = false;
    private bool _isJump = true;
    private Vector2 moveInput;

    void Update()
    {
        // カメラの向きを基準に移動方向を作る
        Vector3 forward = _camera.transform.forward;
        Vector3 right = _camera.transform.right;

        // 上下方向の成分を消す（地面を滑るように移動するため）
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // 入力に応じて移動方向を決定
        Vector3 move = forward * moveInput.y + right * moveInput.x;
        if (_isDash)
        {
            transform.Translate(move * MoveSpeed * MoveBoost * Time.deltaTime, Space.World);
        }
        else
        {
            transform.Translate(move * MoveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void ChangeDash()
    {
        _isDash = !_isDash;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (_isJump)
        {
            _rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _isJump = false;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
         _isJump = true;
    }
}
