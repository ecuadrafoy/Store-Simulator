using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionReference moveAction;
    public CharacterController characterController;
    public float moveSpeed = 5f;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // transform.position = transform.position + move * Time.deltaTime * moveSpeed;
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        Vector3 moveAmount = new Vector3(moveInput.x, 0f, moveInput.y);
        moveAmount = moveAmount * moveSpeed;
        characterController.Move(moveAmount * Time.deltaTime);
    }
}
