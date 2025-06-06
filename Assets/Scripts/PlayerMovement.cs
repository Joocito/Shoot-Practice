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

    public Vector2 mapXBounds = new Vector2(-50f, 50f); // Límites en eje X
    public Vector2 mapZBounds = new Vector2(-50f, 50f); // Límites en eje Z
    public float yLimit = -10f; // Si cae por debajo de este valor, se reinicia

    Vector3 velocity;
    bool isGrounded;
    bool isMoving;
    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);
    public float skinWidth = 0.1f;
    private Vector3 respawnPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        respawnPosition = transform.position; // Guarda posición inicial para respawn
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

        // Normaliza el vector para movimiento diagonal
        if (move.magnitude > 1f) move.Normalize();

        // Verifica colisiones antes de mover
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

        // Verificar límites en X
        if (currentPosition.x < mapXBounds.x || currentPosition.x > mapXBounds.y)
        {
            currentPosition.x = Mathf.Clamp(currentPosition.x, mapXBounds.x, mapXBounds.y);
        }

        // Verificar límites en Z
        if (currentPosition.z < mapZBounds.x || currentPosition.z > mapZBounds.y)
        {
            currentPosition.z = Mathf.Clamp(currentPosition.z, mapZBounds.x, mapZBounds.y);
        }

        // Verificar si cayó del mapa
        if (currentPosition.y < yLimit)
        {
            RespawnPlayer();
            return;
        }

        transform.position = currentPosition;
    }

    void RespawnPlayer()
    {
        // Reiniciar posición y velocidad
        transform.position = respawnPosition;
        velocity = Vector3.zero;

        Debug.Log("Player respawned due to falling off the map");
    }

    void UpdateMovementStatus()
    {
        isMoving = (lastPosition != transform.position && isGrounded);
        lastPosition = transform.position;
    }

    bool CheckCollision(Vector3 direction, float distance)
    {
        RaycastHit hit;
        float collisionDistance = controller.radius + distance;

        return Physics.SphereCast(
            transform.position,
            controller.radius,
            direction,
            out hit,
            collisionDistance,
            groundMask);
    }

    // Dibuja gizmos para visualizar los límites en el editor
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