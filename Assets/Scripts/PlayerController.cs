
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Input")]
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

    [Header("Interaction")]
    [SerializeField] private LayerMask whatIsStock;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float throwForce = 6f;

    private float verticalVelocity;
    private float horizontalRotation;
    private float verticalRotation;

    private GameObject heldPickup;
    private Rigidbody heldRigidbody;

    // physics/throw scheduling
    private Rigidbody toThrowRigidbody;
    private Vector3 toThrowForce;
    private bool throwPending;

    private void OnEnable()
    {
        ValidateReferences();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook();
        HandleMovement();
        HandleInteraction();
    }

    private void FixedUpdate()
    {
        if (throwPending && toThrowRigidbody != null)
        {
            toThrowRigidbody.AddForce(toThrowForce, ForceMode.Impulse);
            toThrowRigidbody = null;
            throwPending = false;
        }
    }

    private void HandleLook()
    {
        Vector2 lookInput = Vector2.zero;
        if (lookAction != null && lookAction.action != null)
            lookInput = lookAction.action.ReadValue<Vector2>();

        horizontalRotation += lookInput.x * Time.deltaTime * lookSpeed;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        verticalRotation -= lookInput.y * Time.deltaTime * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);

        if (cameraTransform != null)
            cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector2 moveInput = Vector2.zero;
        if (moveAction != null && moveAction.action != null)
            moveInput = moveAction.action.ReadValue<Vector2>();

        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        Vector3 moveAmount = moveDirection * moveSpeed;

        if (characterController != null && characterController.isGrounded)
        {
            verticalVelocity = 0f;
            if (jumpAction != null && jumpAction.action != null && jumpAction.action.WasPressedThisFrame())
            {
                verticalVelocity = jumpForce;
            }
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        verticalVelocity = Mathf.Clamp(verticalVelocity, -maxFallSpeed, Mathf.Abs(jumpForce));

        moveAmount.y = verticalVelocity;

        if (characterController != null)
            characterController.Move(moveAmount * Time.deltaTime);
    }

    private void HandleInteraction()
    {
        if (cameraTransform == null)
            return;

        Ray ray = Camera.main != null ? Camera.main.ViewportPointToRay(new Vector3(.5f, .5f, 0f)) : new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        // pick up
        if (heldPickup == null)
        {
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    heldPickup = hit.collider.gameObject;
                    if (heldPickup != null)
                    {
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.transform.localPosition = Vector3.zero;
                        heldPickup.transform.localRotation = Quaternion.identity;
                        if (heldPickup.TryGetComponent<Rigidbody>(out var rb))
                        {
                            heldRigidbody = rb;
                            heldRigidbody.isKinematic = true;
                        }
                        else
                        {
                            Debug.LogWarning("Picked up object has no Rigidbody", heldPickup);
                        }
                    }
                }
            }
        }
        else
        {
            // throw / drop
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (heldRigidbody != null)
                {
                    heldRigidbody.isKinematic = false;
                    toThrowRigidbody = heldRigidbody;
                    toThrowForce = (cameraTransform != null ? cameraTransform.forward : transform.forward) * throwForce;
                    throwPending = true;
                }

                if (heldPickup != null)
                {
                    heldPickup.transform.SetParent(null);
                    heldPickup = null;
                }

                heldRigidbody = null;
            }
        }
    }

    private void ValidateReferences()
    {
        if (moveAction == null) Debug.LogError("Move Action is not assigned", this);
        if (jumpAction == null) Debug.LogError("Jump Action is not assigned", this);
        if (lookAction == null) Debug.LogError("Look Action is not assigned", this);
        if (characterController == null) Debug.LogError("CharacterController is not assigned", this);
        if (cameraTransform == null) Debug.LogError("Camera Transform is not assigned", this);
        if (holdPoint == null) Debug.LogError("Hold Point is not assigned", this);
    }
}
