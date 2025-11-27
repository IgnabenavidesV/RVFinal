using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerMovement : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody rb;
    public Transform playerHead;
    public Transform feetPosition;
    public LayerMask groundLayer;

    [Header("Configuración")]
    public float moveSpeed = 3f;
    public float jumpForce = 5f;
    public float groundCheckDistance = 0.1f;

    private Vector2 inputDirection;
    private bool isGrounded;
    private PlayerInput playerInput;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void OnMove(InputValue value)
    {
        inputDirection = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
            Jump();
    }

    void Update()
    {
        CheckGrounded();
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void CheckGrounded()
    {
        isGrounded = Physics.Raycast(
            feetPosition.position,
            Vector3.down,
            groundCheckDistance,
            groundLayer
        );
    }

    void MovePlayer()
    {
        if (inputDirection.magnitude < 0.1f)
            return;

        Vector3 forward = Vector3.ProjectOnPlane(playerHead.forward, Vector3.up).normalized;

        Vector3 moveDirection =
            forward * inputDirection.y +
            playerHead.right * inputDirection.x;

        moveDirection.Normalize();

        Vector3 newVel = moveDirection * moveSpeed;
        newVel.y = rb.linearVelocity.y;     // ← ahora sí correcto

        rb.linearVelocity = newVel;
    }

    void Jump()
    {
        rb.linearVelocity = new Vector3(
            rb.linearVelocity.x,
            jumpForce,
            rb.linearVelocity.z
        );
    }
}
