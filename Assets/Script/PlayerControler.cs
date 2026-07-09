using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerControler : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float MoveBoost = 2f;
    public float JumpPower = 1.5f;
    public Camera _camera;
    public Rigidbody _rb;
    public HotbarMG _hotbar;
    public InventoryUIMG _inventoryUI;
    public Inventory _playerInventory;

    public float reachDistance = 5f; // ブロックの届く距離
    public BlockWorld blockchunk;   //破壊・設置対象のチャンク

    public Material _outlineMaterial;
    public Mesh cubeMesh;

    [SerializeField] private bool _isDash = false;
    private bool _isJump = true;
    //private bool _isGround = false;
    private Vector2 moveInput;
    private Outline currentOutline;

    private Vector3 _resetPos;

    public void Start()
    {
        _resetPos = transform.position;
    }

    private void Update()
    {
        if (transform.position.y <= blockchunk.worldMinY　- 10) transform.position = _resetPos;
    }

    void FixedUpdate()
    {
        _isJump = CheckGrounded();

        // カメラの向きを基準に移動方向を作る
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // 上下方向の成分を消す（地面を滑るように移動するため）
        forward.y = 0;
        right.y = 0;

        /*    forward.Normalize();
            right.Normalize();*/

        // 入力に応じて移動方向を決定
        Vector3 move = forward * moveInput.y + right * moveInput.x;

        float speed = _isDash ? MoveSpeed * MoveBoost : MoveSpeed;
        _rb.MovePosition(transform.position + move * speed * Time.fixedDeltaTime);
        //transform.Translate(move * speed * Time.deltaTime, Space.World);

        DrawBlockOutline();
        //HandleOutline();

        if (IsInWater()) _rb.AddForce(Vector3.up * JumpPower, ForceMode.Force);

        //落下時
        if (_rb.linearVelocity.y < 0)
            _rb.AddForce(Physics.gravity * 2f, ForceMode.Acceleration);

        //上昇時
        else if (_rb.linearVelocity.y > 0)
            _rb.AddForce(Physics.gravity * 1.2f, ForceMode.Acceleration);
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
        if (!context.performed) return;

        if (_isJump || IsInWater())
        {
            _rb.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            _isJump = false;
        }

    }
    /*    private void OnCollisionEnter(Collision collision)
        {
           if(collision.gameObject.layer == "Block")
            {

            }
             _isJump = true;
        }*/

    // --- ブロック破壊 ---
    public void BreakBlock(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Vector3 hitPos = hit.point - hit.normal * 0.01f;
            Vector3Int blockPos = Vector3Int.FloorToInt(hitPos);

            int blockID = blockchunk.GetBlock(blockPos);

            if (blockID == 0) return;
            else if (blockID == 99) return;


            // アイテム取得
            ItemObject item =BlockDatabase.Instance.GetItem(blockID); 
            if (item != null)
                _playerInventory.itemGet(item);


            blockchunk.ModifyBlock(blockPos, 0);
        }
        _hotbar.UpdateUI();
        _inventoryUI.UpdateUI();
    }

    // --- ブロック設置 ---
    public void PlaceBlock(InputAction.CallbackContext context)
    {
        if (!context.started) return;

        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Vector3 hitPos = hit.point + hit.normal * 0.01f;
            Vector3Int blockPos = Vector3Int.FloorToInt(hitPos);

            ItemObject item = _hotbar.GetSelectedItem();

            if (item != null && _playerInventory.ItemHas(item))
            {
                Bounds playerBounds = GetComponent<Collider>().bounds;

                if (playerBounds.Contains(blockPos + Vector3.one * 0.5f)) return;

                blockchunk.ModifyBlock(blockPos, item.blockID);

                _playerInventory.ItemRemove(item);
            }
        }
        _hotbar.UpdateUI();
        _inventoryUI.UpdateUI();
    }

    public void UIMoveLeft(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        int newIndex = _hotbar.selectedIndex - 1;
        if (newIndex < 0) newIndex = _hotbar.slots.Length - 1;

        _hotbar.Select(newIndex);
    }
    public void UIMoveRight(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        int newIndex = (_hotbar.selectedIndex + 1) % _hotbar.slots.Length;

        _hotbar.Select(newIndex);
    }

    bool CheckGrounded()
    {
        float checkDistance = 1.2f;

        bool grounded = Physics.Raycast(transform.position, Vector3.down, checkDistance, LayerMask.GetMask("Block"));

        Debug.Log(grounded);
        Debug.DrawRay(transform.position, Vector3.down * checkDistance, grounded ? Color.green : Color.red);

        return  grounded;
    }

    /*    bool TryGetTargetBlock(out Vector3Int blockPos)
        {
            blockPos = default;

            Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

            if(Physics.Raycast(ray, out RaycastHit hit, reachDistance))
            {
                Vector3 hitPos = hit.point - hit.normal * 0.01f;
                blockPos = Vector3Int.FloorToInt(hitPos);
                return true;
            }
            return false;
        }*/

    void HandleOutline()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Outline outline = hit.collider.GetComponent<Outline>();
            if (outline != null)
            {
                if (currentOutline != null && currentOutline != outline)
                    currentOutline.enabled = false;

                outline.enabled = true;
                currentOutline = outline;

                return;
            }
        }

        if (currentOutline != null)
        {
            currentOutline.enabled = false;
            currentOutline = null;
        }
    }

    void DrawBlockOutline()
    {
        Ray ray = new Ray(_camera.transform.position, _camera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, reachDistance))
        {
            Vector3 hitPos = hit.point - hit.normal * 0.01f;
            Vector3Int blockPos = Vector3Int.FloorToInt(hitPos);

            Vector3 center = blockPos + Vector3.one * 0.5f;
            Matrix4x4 matrix = Matrix4x4.TRS(center, Quaternion.identity, Vector3.one * 1.01f);

            _outlineMaterial.SetPass(0);
            Graphics.DrawMeshNow(cubeMesh, matrix);
        }
    }
    public void CraftAdd(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        ItemObject selected = _hotbar.GetSelectedItem();
        if (selected == null) return;

        //_craftingUI.AddItem(selected);
    }

    bool IsInWater()
    {
        Vector3Int blockPos =
            Vector3Int.FloorToInt(transform.position);

        int blockID =
            blockchunk.GetBlock(blockPos);

        return blockID == 8;
    }
}
