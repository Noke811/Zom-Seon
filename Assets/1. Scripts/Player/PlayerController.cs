using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    public float moveSpeed;
    public float jumpForce;
    private Vector2 _moveInput;
    private Rigidbody _rigidbody;
    private Vector3 _direction;
    // public LayerMask groundLayerMask;
    // public float groundRayDistance;
    
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
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void LateUpdate()
    {
        Look();
    }

    private void Move()
    {
        _direction = transform.forward * _moveInput.y + transform.right * _moveInput.x;
        _direction *= moveSpeed;
        _direction.y = _rigidbody.velocity.y;
        
        _rigidbody.velocity = _direction;
        
        // Vector3 moveInput = new Vector3(_moveInput.x, 0, _moveInput.y);
        // Vector3 worldMove = transform.TransformDirection(moveInput);
        // if (worldMove.magnitude > 1)
        //     worldMove.Normalize();
        // Vector3 moveVelocity = worldMove * moveSpeed;
        // rigidbody.velocity = new Vector3(moveVelocity.x, rigidbody.velocity.y, moveVelocity.z);

        // float moveTime = moveSpeed * Time.deltaTime;
        // rigidbody.MovePosition(transform.position + moveInput * moveTime);
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
        if (context.phase == InputActionPhase.Performed)
        {
            _moveInput = context.ReadValue<Vector2>();
            _animator.SetBool(_isRun, true);
        }
        else if (context.phase == InputActionPhase.Canceled)
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

    // bool IsGround Ray
    // private bool IsGround()
    // {
    //     Ray[] rays = new Ray[4]
    //     {
    //         new Ray(transform.position + (transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down * 100.0f),
    //         new Ray(transform.position + (-transform.forward * 0.2f) + (transform.up * 0.01f), Vector3.down * 100.0f),
    //         new Ray(transform.position + (transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down * 100.0f),
    //         new Ray(transform.position + (-transform.right * 0.2f) + (transform.up * 0.01f), Vector3.down * 100.0f)
    //     };
    //
    //     for (int i = 0; i < rays.Length; i++)
    //     {
    //         if (Physics.Raycast(rays[i], groundRayDistance, groundLayerMask))
    //         {
    //             return true;
    //         }
    //     }
    //
    //     return false;
    // }

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

    // 인벤토리 키 (I)

    // 제작 메뉴 (Tab)

    // 퀵슬롯 (1 ~ 5)

    // 공격 (좌클릭)

    // 달리기 (쉬프트)
}