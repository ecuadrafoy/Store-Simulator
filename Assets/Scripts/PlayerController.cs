
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input References")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference lookAction;

    [Header("Movement")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float maxFallSpeed = 50f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float lookSpeed = 2f;
    [SerializeField] private float minLookAngle = -90f;
    [SerializeField] private float maxLookAngle = 90f;

    private float verticalVelocity;
    private float horizontalRotation;
    private float verticalRotation;

    private void OnEnable()
    {
        ValidateReferences();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.Confined;
    }

    private void Update()
    {
        UpdateCameraRotation();
        UpdatePlayerMovement();
    }

    private void UpdateCameraRotation()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        horizontalRotation += lookInput.x * Time.deltaTime * lookSpeed;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        verticalRotation -= lookInput.y * Time.deltaTime * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void UpdatePlayerMovement()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = CalculateMoveDirection(moveInput);
        Vector3 moveAmount = moveDirection * moveSpeed;

        HandleJumpAndGravity();

        moveAmount.y = verticalVelocity;
        characterController.Move(moveAmount * Time.deltaTime);
    }

    private Vector3 CalculateMoveDirection(Vector2 moveInput)
    {
        Vector3 forward = transform.forward * moveInput.y;
        Vector3 right = transform.right * moveInput.x;
        Vector3 moveDirection = (forward + right).normalized;
        return moveDirection;
    }

    private void HandleJumpAndGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;

            if (jumpAction.action.WasPressedThisFrame())
            {
                verticalVelocity = jumpForce;
            }
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, jumpForce);
    }

    private void ValidateReferences()
    {
        if (moveAction == null)
            Debug.LogError("Move Action is not assigned!", this);

        if (jumpAction == null)
            Debug.LogError("Jump Action is not assigned!", this);

        if (lookAction == null)
            Debug.LogError("Look Action is not assigned!", this);

        if (characterController == null)
            Debug.LogError("Character Controller is not assigned!", this);

        if (cameraTransform == null)
            Debug.LogError("Camera Transform is not assigned!", this);
    }
}
