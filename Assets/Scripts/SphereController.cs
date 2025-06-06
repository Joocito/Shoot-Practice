using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class SphereController : MonoBehaviour
{

    public float baseSpeed = 1f;                
    public float speedIncreasePerLevel = 2f;   
    public Vector3 movementLimitsMin = new Vector3(-5f, 1f, -5f);
    public Vector3 movementLimitsMax = new Vector3(5f, 5f, 5f);
    public float directionChangeInterval = 2f;  
    public float boundaryThreshold = 0.1f;      

    public float respawnTime = 10f;             
    public int points = 10;                     
    public GameObject destructionEffect;       
    public AudioClip destructionSound;          

    public TextMeshProUGUI speedDisplayText;    

    // Variables privadas
    private float currentSpeed;                
    private bool isActive = true;
    private Vector3 initialPosition;
    private Vector3 currentDirection;
    private float lastDirectionChangeTime;
    private Rigidbody rb;
    private AudioSource audioSource;
    private int currentSpeedLevel = 0;

    void Start()
    {
        // Inicialización de componentes
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configuración física
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        // Estado inicial
        initialPosition = transform.position;
        currentSpeed = baseSpeed;
        ChangeDirection();

        // Suscripción a eventos
        ScoreManager.OnSpeedIncrease += UpdateSpeedLevel;
        UpdateSpeedDisplay();
    }

    void OnDestroy()
    {
        // Limpieza de suscripción
        ScoreManager.OnSpeedIncrease -= UpdateSpeedLevel;
    }

    void FixedUpdate()
    {
        if (isActive)
        {
            MoveSphere();
            CheckBoundaries();
        }
    }

    void MoveSphere()
    {
        // Cambio de dirección programado
        if (Time.time - lastDirectionChangeTime > directionChangeInterval)
        {
            ChangeDirection();
        }

        // Movimiento con física
        Vector3 movement = currentDirection * currentSpeed * Time.fixedDeltaTime;
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
            rb.velocity = currentDirection * currentSpeed;
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

        // Efectos de destrucción
        if (destructionEffect != null)
        {
            Instantiate(destructionEffect, transform.position, Quaternion.identity);
        }

        if (destructionSound != null)
        {
            audioSource.PlayOneShot(destructionSound);
        }

        // Desactivar visualmente
        gameObject.SetActive(false);

        // Añadir puntos
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(points);
        }

        // Programar respawn
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

    void UpdateSpeedLevel(int speedLevel)
    {
        currentSpeedLevel = speedLevel;
        currentSpeed = baseSpeed + (speedIncreasePerLevel * speedLevel);
        UpdateSpeedDisplay();
    }

    void UpdateSpeedDisplay()
    {
        if (speedDisplayText != null)
        {
            speedDisplayText.text = $"Vel: {currentSpeed:F1}";
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualización de límites en el editor
        Gizmos.color = Color.cyan;
        Vector3 center = (movementLimitsMax + movementLimitsMin) * 0.5f;
        Vector3 size = movementLimitsMax - movementLimitsMin;
        Gizmos.DrawWireCube(center, size);
    }
}