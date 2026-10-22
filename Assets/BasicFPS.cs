using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerFPS : MonoBehaviour
{
    public float playerSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;

    public float rotationSpeed = 1f;

    public Transform playerCamera;

    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private float tiltRotation = 0f;

    [Header("Input Actions")]
    public InputActionReference moveAction; // expects Vector2
    public InputActionReference jumpAction; // expects Button
    public InputActionReference lookAction; // expect Vector2


    private void Start()
    {
        controller = gameObject.GetComponent<CharacterController>();

        // Lock the Cursor
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        lookAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
        lookAction.action.Disable();
    }

    void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // Rotate Body First 
        Vector2 look = lookAction.action.ReadValue<Vector2>();
        transform.Rotate(Vector3.up, (look.x * rotationSpeed * Time.deltaTime));

        // update tilt rotation
        tiltRotation += (-look.y * rotationSpeed * Time.deltaTime);

        // check for bad values
        tiltRotation = Mathf.Clamp(tiltRotation, -90f, 90f);

        // set the rotation to this value
        playerCamera.localEulerAngles = new Vector3 (tiltRotation, 0,0);

        // where we are moving
        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 move = (transform.right * input.x) + (transform.forward * input.y);

        // Jump
        if (jumpAction.action.triggered && groundedPlayer)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2.0f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;

        // Combine horizontal and vertical movement
        Vector3 finalMove = (move * playerSpeed) + (playerVelocity.y * Vector3.up);
        controller.Move(finalMove * Time.deltaTime);

    }


}
