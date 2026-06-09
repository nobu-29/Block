using Unity.VisualScripting.Antlr3.Runtime.Misc;
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
    public int selectedBlockID = 1;
    public HotbarMG _hotbar;

    public float reachDistance = 5f; // ブロックの届く距離
    public BlockWorld blockchunk;
    //public ChunkGenerator chunk;    //破壊・設置対象のチャンク

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

        float speed = _isDash ? MoveSpeed * MoveBoost : MoveSpeed;
        transform.Translate(move * speed * Time.deltaTime, Space.World);
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
        if (context.performed && _isJump)
        {
            _rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _isJump = false;
        }
        
    }
    private void OnCollisionEnter(Collision collision)
    {
         _isJump = true;
    }

    // --- ブロック破壊 ---
    public void BreakBlock(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = new Ray(_camera.transform.position,_camera.transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Vector3 pos = hit.point - hit.normal * 0.5f;
            Vector3Int blockPos = Vector3Int.FloorToInt(pos);

            Destroy(hit.collider.gameObject);
            //chunk.BreakBlock(blockPos);
        }
    }

    // --- ブロック設置 ---
    public void PlaceBlock(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Vector3 pos = hit.point + hit.normal * 0.5f;
            Vector3Int blockPos = Vector3Int.FloorToInt(pos);

            int blockID = _hotbar.GetSelectedBlockID();

            Instantiate(blockchunk.BlockPF, pos, Quaternion.identity);
            //chunk.PlaceBlock(blockPos,selectedBlockID);
        }
    }

    public void UIMoveLeft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        int newIndex = _hotbar.selectedIndex - 1;
        if(newIndex < 0) newIndex = _hotbar.slots.Length - 1;
        
        _hotbar.Select(newIndex);
    }
    public void UIMoveRight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        int newIndex = (_hotbar.selectedIndex + 1) % _hotbar.slots.Length;

        _hotbar.Select(newIndex);
    }
}
