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

    [Header("pickup settings")]
    public bool canPickup; 
    public float pickupRange = 3f;
    public LayerMask pickupMask;

    public static PlayerMovementCC current;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        current = this;
    }

   
    void Update()
    {
        groundcheck();
        jump();
        sprint();
        HandlePickup();

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

    void OnDrawGizmos()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        Gizmos.DrawLine(rayOrigin, rayOrigin + Vector3.down * groundCheckDistance);

        Vector3 lookDir = Camera.main.transform.forward;
        Gizmos.color = canPickup ? Color.green : Color.red;
        Gizmos.DrawLine(rayOrigin, rayOrigin + lookDir * pickupRange);
    }
    void HandlePickup()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * (controller.skinWidth + 0.05f);
        Vector3 lookDir = Camera.main.transform.forward;
        canPickup = Physics.Raycast(rayOrigin, lookDir, pickupRange);

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
