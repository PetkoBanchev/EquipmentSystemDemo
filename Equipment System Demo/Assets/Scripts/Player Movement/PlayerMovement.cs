using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Private variables
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float gravity = -9.2f;
    [SerializeField] private float jumpHeight = 3f;
    private Vector3 velocity;

    private GroundCheck groundCheck;
    private bool isGrounded;
    #endregion

    #region Private methods
    private void Awake()
    {
        groundCheck = GetComponentInChildren<GroundCheck>();
    }
    // Update is called once per frame
    private void Update()
    {
        isGrounded = groundCheck.IsGrounded();

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }
    #endregion
}
