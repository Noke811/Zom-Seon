using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    public float jumpForce;
    public float _baseMoveSpeed;
    private Vector2 _moveInput;
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    private float _dashSpeed;
    public float curMoveSpeed;
    
    private bool _isGround;
    private int _isRun;
    private int _isJump;

    [Header("Looking")]
    public Transform cameraContainer;
    public float sensitive;
    private Vector2 _mouseDelta;
    private float _camCurXRot;

    private Animator _animator;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        cameraContainer = transform.Find("Camera Container");

        _isRun = Animator.StringToHash("isRun");
        _isJump = Animator.StringToHash("isJump");
        
        //Cursor.lockState = CursorLockMode.Locked;

        curMoveSpeed = _baseMoveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        if(!GameManager.Instance.UIManager.IsUIActive)
            Look();
    }

    private void Move()
    {
        _direction = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        _direction *= curMoveSpeed;
        _direction.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = _direction;
    }

    private void Look()
    {
        _camCurXRot += _mouseDelta.y * sensitive;
        _camCurXRot = Mathf.Clamp(_camCurXRot, -90, 90);
        cameraContainer.localEulerAngles = new Vector3(-_camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, _mouseDelta.x * sensitive, 0);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            _animator.SetBool(_isRun, true);
        }
        else if (context.canceled)
        {
            _moveInput = Vector2.zero;
            _animator.SetBool(_isRun, false);
        }
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && _isGround)
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGround = true;
        _animator.SetBool(_isJump, false);
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGround = false;
        _animator.SetBool(_isJump, true);
    }

    // 상호작용 키 (F)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("상호작용");
        }
    }

    // 인벤토리 키 (I)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("인벤토리");
            // 인벤토리 UI 열기
            GameManager.Instance.UIManager.SetInventoryUI();
        }
    }

    // 제작 메뉴 (Tab)
    public void OnCraft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("제작 메뉴");
            // GameManager.Instance.UIManager.제작UI토글();
        }
    }

    // 퀵슬롯 (1 ~ 5)
    public void OnQuickSlot(InputAction.CallbackContext context)
    {
        int controlNum = int.Parse(context.control.name);
        if (context.started)
            switch (controlNum)
            {
                case 1:
                    Debug.Log("1번 슬롯 선택");
                    break;
                case 2:
                    Debug.Log("2번 슬롯 선택");
                    break;
                case 3:
                    Debug.Log("3번 슬롯 선택");
                    break;
                case 4:
                    Debug.Log("4번 슬롯 선택");
                    break;
                case 5:
                    Debug.Log("5번 슬롯 선택");
                    break;
            }
    }

    // 공격 (좌클릭)
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("공격!");
            // 공격 애니메이션 실행
            // _animator.SetTrigger("Attack");
        }
    }

    // 달리기 (쉬프트)
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
            curMoveSpeed = _dashSpeed = (_baseMoveSpeed * 1.4f);
        else if (context.canceled)
            curMoveSpeed = _baseMoveSpeed;
    }   
}