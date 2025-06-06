using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    public float speed = 1f;
    public Vector3 movementLimitsMin = new Vector3(-5f, 1f, -5f);
    public Vector3 movementLimitsMax = new Vector3(5f, 5f, 5f);
    public float directionChangeInterval = 2f;
    public float boundaryThreshold = 0.1f; // Margen para detectar límites

    public float respawnTime = 10f;
    public int points = 10;

    private bool isActive = true;
    private Vector3 initialPosition;
    private Vector3 currentDirection;
    private float lastDirectionChangeTime;
    private Rigidbody rb;

    void Start()
    {
        initialPosition = transform.position;
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.useGravity = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous; // Mejor detección de colisiones
        }

        ChangeDirection();
    }

    void FixedUpdate() // Cambiado a FixedUpdate para mejor física
    {
        if (isActive)
        {
            MoveSphere();
        }
    }

    void MoveSphere()
    {
        if (Time.time - lastDirectionChangeTime > directionChangeInterval)
        {
            ChangeDirection();
        }

        // Movimiento con física
        Vector3 movement = currentDirection * speed * Time.fixedDeltaTime;
        Vector3 newPosition = rb.position + movement;

        // Aplicar límites con margen
        newPosition.x = Mathf.Clamp(newPosition.x,
                                  movementLimitsMin.x + boundaryThreshold,
                                  movementLimitsMax.x - boundaryThreshold);
        newPosition.y = Mathf.Clamp(newPosition.y,
                                  movementLimitsMin.y + boundaryThreshold,
                                  movementLimitsMax.y - boundaryThreshold);
        newPosition.z = Mathf.Clamp(newPosition.z,
                                  movementLimitsMin.z + boundaryThreshold,
                                  movementLimitsMax.z - boundaryThreshold);

        rb.MovePosition(newPosition);

        // Detección mejorada de límites
        CheckBoundaries();
    }

    void CheckBoundaries()
    {
        Vector3 pos = transform.position;
        bool hitBoundary = false;

        // Verifica cada eje con un margen
        if (pos.x <= movementLimitsMin.x + boundaryThreshold ||
            pos.x >= movementLimitsMax.x - boundaryThreshold)
        {
            currentDirection.x *= -1;
            hitBoundary = true;
        }

        if (pos.y <= movementLimitsMin.y + boundaryThreshold ||
            pos.y >= movementLimitsMax.y - boundaryThreshold)
        {
            currentDirection.y *= -1;
            hitBoundary = true;
        }

        if (pos.z <= movementLimitsMin.z + boundaryThreshold ||
            pos.z >= movementLimitsMax.z - boundaryThreshold)
        {
            currentDirection.z *= -1;
            hitBoundary = true;
        }

        if (hitBoundary)
        {
            lastDirectionChangeTime = Time.time;
            // Ajuste adicional para evitar quedarse pegado
            rb.velocity = currentDirection * speed;
        }
    }

    void ChangeDirection()
    {
        currentDirection = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(-0.5f, 0.5f), // Movimiento vertical más suave
            Random.Range(-1f, 1f)
        ).normalized;

        lastDirectionChangeTime = Time.time;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!isActive) return;

        if (collision.gameObject.CompareTag("Bullet") ||
            collision.gameObject.GetComponent<Bullet>() != null)
        {
            DestroySphere();
        }
        else
        {
            // Cambia dirección al chocar con otros objetos
            Vector3 normal = collision.contacts[0].normal;
            currentDirection = Vector3.Reflect(currentDirection, normal).normalized;
            lastDirectionChangeTime = Time.time;
        }
    }

    void DestroySphere()
    {
        if (!isActive) return;

        isActive = false;
        gameObject.SetActive(false);

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
        }
        else
        {
            Debug.LogError("ScoreManager instance not found!");
        }

        Invoke(nameof(RespawnSphere), respawnTime);
    }

    void RespawnSphere()
    {
        isActive = true;
        transform.position = initialPosition;
        rb.velocity = Vector3.zero;
        ChangeDirection();
        gameObject.SetActive(true);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (movementLimitsMax + movementLimitsMin) * 0.5f;
        Vector3 size = movementLimitsMax - movementLimitsMin;
        Gizmos.DrawWireCube(center, size);
    }
}