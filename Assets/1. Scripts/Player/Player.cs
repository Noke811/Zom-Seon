using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Moving")]
    public float moveSpeed;
    public float jumpForce;
    private Vector2 moveInput;
    public Rigidbody rigidbody;
    
    private void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
	
    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        rigidbody.MovePosition(this.transform.position + move * moveSpeed * Time.deltaTime);
    }
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            moveInput = context.ReadValue<Vector2>();
        else if (context.phase == InputActionPhase.Canceled)
            moveInput = Vector2.zero;
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}