using UnityEngine;

public class PlayerMovementCC : MonoBehaviour
{
    [Header("Movment Settings")]
    public float speed = 5f; 
    public float gravity = -9.81f; 
    public float jumpHeight = 1.5f;
    public float sprintSpeed = 8f;

    [Header("Ground Check Settings")]
    public float groundCheckDistance = 0.1f;
    public LayerMask groundMask;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    
    void Start()
    {
        controller = GetComponent<CharacterController>(); 
    }

   
    void Update()
    {
        groundcheck();
        jump();
        sprint();

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        Vector3 move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");
        controller.Move(move * speed * Time.deltaTime);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void groundcheck()
    {  
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundCheckDistance, groundMask);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);
    }

    void jump()
    {
        if (GameManager.current == null)
            return;
        if (Input.GetButtonDown("Jump") && isGrounded && GameManager.current.canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            Debug.Log("Jumping with velocity: " + velocity.y);
            GameManager.current.currentStamina -= 10f;
        }
    }
    void sprint()
    {
        if (GameManager.current == null)
            return;

        if (Input.GetKey(KeyCode.LeftShift) && GameManager.current.canSprint && isGrounded)
        {
            speed = sprintSpeed;
            GameManager.current.currentStamina -= 20f * Time.deltaTime;
        }
        else
        {
            speed = 5f;
            GameManager.current.RegenerateStamina(5f * Time.deltaTime);
        }
    }
}
