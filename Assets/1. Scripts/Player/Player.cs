using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Moving")]
    public float moveSpeed;
    public float jumpForce;
    private Vector2 _moveInput;
    public Rigidbody rigidbody;
    // public LayerMask groundLayerMask;
    // public float groundRayDistance;
    private bool _isGround;

    [Header("Looking")]
    public Transform cameraContainer;
    public float sensitive;
    private Vector2 mouseDelta;
    public float camCurXRot;
    
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
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
        Vector3 moveInput = new Vector3(_moveInput.x, 0, _moveInput.y);
        float moveTime = moveSpeed * Time.deltaTime;
        rigidbody.MovePosition(transform.position + moveInput * moveTime);
    }

    private void Look()
    {
        camCurXRot += mouseDelta.y * sensitive;
        camCurXRot = Mathf.Clamp(camCurXRot, -90, 90);
        cameraContainer.localEulerAngles = new Vector3(-camCurXRot, 0, 0);
        
        transform.eulerAngles += new Vector3(0, mouseDelta.x * sensitive, 0);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            _moveInput = context.ReadValue<Vector2>();
        else if (context.phase == InputActionPhase.Canceled)
            _moveInput = Vector2.zero;
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        mouseDelta = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && _isGround)
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    // IsGround Ray
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
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Ground"))
            _isGround = false;
    }
}