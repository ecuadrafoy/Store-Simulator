
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public InputActionReference lookAction;
    public CharacterController characterController;
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    private float ySpeed;
    private float horizontalRotation, verticalRotation;
    public float lookSpeed;
    public Transform cameraTransform;
    public float minLookAngle, maxLookAngle;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 lookInput = lookAction.action.ReadValue<Vector2>();

        horizontalRotation += lookInput.x * Time.deltaTime * lookSpeed;
        transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);

        verticalRotation -= lookInput.y * Time.deltaTime * lookSpeed;
        verticalRotation = Mathf.Clamp(verticalRotation, minLookAngle, maxLookAngle);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);


        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        // Vector3 moveAmount = new Vector3(moveInput.x, 0f, moveInput.y);
        Vector3 verticalMovement = transform.forward * moveInput.y;
        Vector3 horizontalMovement = transform.right * moveInput.x;
        Vector3 moveAmount = verticalMovement + horizontalMovement;
        moveAmount = moveAmount.normalized;
        moveAmount = moveAmount * moveSpeed;
        if (characterController.isGrounded == true)
        {
            ySpeed = 0f;
            if (jumpAction.action.WasPressedThisFrame())
            {
                ySpeed = jumpForce;
            }
        }
        ySpeed = ySpeed + (Physics.gravity.y * Time.deltaTime);

        moveAmount.y = ySpeed;
        characterController.Move(moveAmount * Time.deltaTime);
    }
}
