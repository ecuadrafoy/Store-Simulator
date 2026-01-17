
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
    public Camera cameraTransform;
    public float minLookAngle, maxLookAngle;
    public LayerMask whatIsStock;
    public float interactionRange;
    private StockObject heldPickup;
    public Transform holdPoint;

    public float throwForce;
    public LayerMask whatIsShelf;
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
        cameraTransform.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);


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

        //check for pickup
        Ray ray = cameraTransform.ViewportPointToRay(new Vector3(.5f, .5f, 0f));
        RaycastHit hit;
        /*if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
        {
            Debug.Log("I see a pickup");
        }
        else
        {
            Debug.Log("I see nothing");
        }*/
        if (heldPickup == null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsStock))
                {
                    /*heldPickup = hit.collider.gameObject;
                    heldPickup.transform.SetParent(holdPoint);
                    heldPickup.transform.localPosition = Vector3.zero;
                    heldPickup.transform.localRotation = Quaternion.identity;
                    heldPickup.GetComponent<Rigidbody>().isKinematic = true;*/

                    heldPickup = hit.collider.GetComponent<StockObject>();
                    heldPickup.transform.SetParent(holdPoint);
                    heldPickup.Pickup();
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    heldPickup = hit.collider.GetComponent<ShelfSpaceController>().GetStock();
                    if (heldPickup != null)
                    {
                        heldPickup.transform.SetParent(holdPoint);
                        heldPickup.Pickup();
                    }
                }
            }
        }
        else
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                if (Physics.Raycast(ray, out hit, interactionRange, whatIsShelf))
                {
                    /*heldPickup.transform.position = hit.transform.position;
                    heldPickup.transform.rotation = hit.transform.rotation;

                    heldPickup.transform.SetParent(null);
                    heldPickup = null;*/
                    /*heldPickup.Makeplace();
                    heldPickup.transform.SetParent(hit.transform);
                    heldPickup = null;*/
                    hit.collider.GetComponent<ShelfSpaceController>().PlaceStock(heldPickup);
                    if (heldPickup.isPlaced == true)
                    {
                        heldPickup = null;
                    }
                }
            }
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                //Rigidbody pickupRigidBody = heldPickup.GetComponent<Rigidbody>();
                //pickupRigidBody.isKinematic = false;
                heldPickup.Release();
                heldPickup.rigidBody.AddForce(cameraTransform.transform.forward * throwForce, ForceMode.Impulse);
                heldPickup.transform.SetParent(null);
                heldPickup = null;
            }
        }
    }
}
