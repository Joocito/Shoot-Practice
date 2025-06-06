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

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    // Añade un pequeño desplazamiento para evitar pegarse a las paredes
    public float skinWidth = 0.1f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Normaliza el vector para que no se mueva más rápido en diagonal
        if (move.magnitude > 1f)
        {
            move.Normalize();
        }

        // Verifica colisiones antes de mover
        if (CheckCollision(move, skinWidth))
        {
            move = Vector3.zero; // No te muevas si hay colisión
        }

        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != gameObject.transform.position && isGrounded == true)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = gameObject.transform.position;
    }

    // Método para verificar colisiones antes de moverte
    private bool CheckCollision(Vector3 direction, float distance)
    {
        RaycastHit hit;
        float collisionDistance = controller.radius + distance;

        if (Physics.SphereCast(
            transform.position,
            controller.radius,
            direction,
            out hit,
            collisionDistance,
            groundMask))
        {
            return true; // Hay colisión
        }
        return false;
    }
}