using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Moving")]
    public float jumpForce;
    public float _baseMoveSpeed;
    public float curMoveSpeed;
    public float rayDistance;
    public LayerMask groundLayerMask;
    
    private Animator _animator;
    private Rigidbody _rigidbody;
    private Vector2 _moveInput;
    private Vector3 _direction;
    private float _dashSpeed;
    
    private bool _isGround;
    private int _isRun;
    private int _isJump;
    public bool isDash;

    [Header("Looking")]
    Transform cameraContainer;
    InteractableDetector interactableDetector;
    public float sensitive;
    private Vector2 _mouseDelta;
    private float _camCurXRot;

    [Header("Attack")]
    public float attackCooltime;

    private float _attackNextCool;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        cameraContainer = GetComponent<Player>().Head.transform;
        interactableDetector = GetComponent<Player>().Head;

        _isRun = Animator.StringToHash("isRun");
        _isJump = Animator.StringToHash("isJump");

        curMoveSpeed = _baseMoveSpeed;
    }

    private void FixedUpdate()
    {
        Move();
        Jumped();
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
        
        bool isWalking = _moveInput.magnitude > 0.1f && isGround();

        if (isWalking)
            SoundManager.Instance.PlayLoopSFX(0);
        else
            SoundManager.Instance.StopLoopSFX();
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
        if (context.started && isGround())
        {
            _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            GameManager.Instance.Player.Condition.JumpStamina();
        }
    }
    
    public bool isGround()
    {
        Ray[] jumpRays = new Ray[4]
        {
            new Ray(transform.position + (transform.forward / 7f), Vector3.down),
            new Ray(transform.position + (-transform.forward / 7f), Vector3.down),
            new Ray(transform.position + (transform.right / 7f),Vector3.down),
            new Ray(transform.position + (-transform.right / 7f), Vector3.down)
        };
    
        for (int i = 0; i < jumpRays.Length; i++)
        {
            if (Physics.Raycast(jumpRays[i], rayDistance, groundLayerMask))
            {
                return true;
            }
        }
    
        return false;
    }

    private void Jumped()
    {
        if (isGround())
            _animator.SetBool(_isJump, false);
        else
            _animator.SetBool(_isJump, true);
    }

    // 상호작용 키 (F)
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            interactableDetector.Interaction();
        }
    }

    // 인벤토리 키 (I)
    public void OnInventory(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.UIManager.SetInventoryUI();
        }
    }

    // 제작 메뉴 (Tab)
    public void OnCraft(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            GameManager.Instance.UIManager.SetArchitectUI();
        }
    }

    // 퀵슬롯 (1 ~ 5)
    public void OnQuickSlot(InputAction.CallbackContext context)
    {
        int controlNum = int.Parse(context.control.name);
        if (context.started && !GameManager.Instance.UIManager.IsUIActive)
            GameManager.Instance.Inventory.SelectQuickslot(controlNum - 1);
    }

    // 공격 (좌클릭)
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && GameManager.Instance.Player.Equipment.IsEquip &&
            !GameManager.Instance.UIManager.IsUIActive)
        {
            bool isCoolDown = Time.time >= _attackNextCool;
            if (isCoolDown)
            {
                GameManager.Instance.Player.Equipment.Attack();
                _attackNextCool = Time.time + attackCooltime;
            }
        }
    }

    // 달리기 (쉬프트)
    public void OnDash(InputAction.CallbackContext context)
    {
        if (context.performed)
        { 
            curMoveSpeed = _dashSpeed = (_baseMoveSpeed * 1.4f);
            isDash = true;
        }
        else if (context.canceled)
        {
            curMoveSpeed = _baseMoveSpeed;
            isDash = false;
        }
    }   
}