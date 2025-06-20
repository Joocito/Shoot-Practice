using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    public LayerMask obstacleMask; // NUEVO: máscara para objetos de tiro

    public Vector2 mapXBounds = new Vector2(-50f, 50f);
    public Vector2 mapZBounds = new Vector2(-50f, 50f);
    public float yLimit = -10f;

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = Vector3.zero;
    public float skinWidth = 0.1f;
    private Vector3 respawnPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        respawnPosition = transform.position;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        ApplyGravity();
        CheckMapBoundaries();
        UpdateMovementStatus();
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        if (move.magnitude > 1f) move.Normalize();

        // Usa las máscaras combinadas
        if (!CheckCollision(move, skinWidth))
        {
            controller.Move(move * speed * Time.deltaTime);
        }
    }

    void HandleJump()
    {
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void ApplyGravity()
    {
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    void CheckMapBoundaries()
    {
        Vector3 currentPosition = transform.position;

        currentPosition.x = Mathf.Clamp(currentPosition.x, mapXBounds.x, mapXBounds.y);
        currentPosition.z = Mathf.Clamp(currentPosition.z, mapZBounds.x, mapZBounds.y);

        if (currentPosition.y < yLimit)
        {
            RespawnPlayer();
            return;
        }

        transform.position = currentPosition;
    }

    void RespawnPlayer()
    {
        transform.position = respawnPosition;
        velocity = Vector3.zero;
        Debug.Log("Player respawned due to falling off the map");
    }

    void UpdateMovementStatus()
    {
        isMoving = (lastPosition != transform.position && isGrounded);
        lastPosition = transform.position;
    }

    // MODIFICADO: ahora combina ground y obstáculos
    bool CheckCollision(Vector3 direction, float distance)
    {
        RaycastHit hit;
        float collisionDistance = controller.radius + distance;
        int combinedMask = groundMask | obstacleMask;

        return Physics.SphereCast(
            transform.position,
            controller.radius,
            direction,
            out hit,
            collisionDistance,
            combinedMask);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 center = new Vector3(
            (mapXBounds.x + mapXBounds.y) / 2,
            0,
            (mapZBounds.x + mapZBounds.y) / 2);
        Vector3 size = new Vector3(
            mapXBounds.y - mapXBounds.x,
            20f,
            mapZBounds.y - mapZBounds.x);
        Gizmos.DrawWireCube(center, size);
    }
}
