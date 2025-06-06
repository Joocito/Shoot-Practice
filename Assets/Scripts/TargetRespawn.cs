using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]

public class TargetRespawn : MonoBehaviour
{


    [Header("Respawn Settings")]
    [Tooltip("Tiempo en segundos antes de reaparecer")]
    public float respawnTime = 5f;

    [Header("Visual Settings")]
    [Tooltip("Modelo normal del target")]
    public GameObject intactModel;
    [Tooltip("Modelo destruido (opcional)")]
    public GameObject destroyedModel;

    [Header("Effects")]
    [Tooltip("Efecto de partículas al destruir")]
    public ParticleSystem destructionEffect;
    [Tooltip("Sonido al destruir")]
    public AudioClip destructionSound;
    [Tooltip("Sonido al reaparecer")]
    public AudioClip respawnSound;

    // Variables privadas
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Rigidbody rb;
    private Collider col;
    private AudioSource audioSource;
    private bool isActive = true;

    private void Awake()
    {
        // Guardar transformación original
        originalPosition = transform.position;
        originalRotation = transform.rotation;

        // Obtener componentes
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();

        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f; // Sonido 3D
        }

        // Configurar modelo inicial
        if (intactModel == null) intactModel = gameObject;
    }

    public void TakeDamage()
    {
        if (!isActive) return;

        StartCoroutine(DestroyAndRespawn());
    }

    private IEnumerator DestroyAndRespawn()
    {
        // Estado destruido
        isActive = false;

        // 1. Desactivar física
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (col != null) col.enabled = false;

        // 2. Cambiar modelo visual
        if (intactModel != null) intactModel.SetActive(false);
        if (destroyedModel != null) destroyedModel.SetActive(true);

        // 3. Reproducir efectos
        if (destructionEffect != null) destructionEffect.Play();
        if (destructionSound != null) audioSource.PlayOneShot(destructionSound);

        // Esperar tiempo de respawn
        yield return new WaitForSeconds(respawnTime);

        // 4. Restaurar posición/rotación
        transform.position = originalPosition;
        transform.rotation = originalRotation;

        // 5. Reactivar modelo
        if (intactModel != null) intactModel.SetActive(true);
        if (destroyedModel != null) destroyedModel.SetActive(false);

        // 6. Reactivar física
        if (rb != null) rb.isKinematic = false;
        if (col != null) col.enabled = true;

        // 7. Reproducir sonido de respawn
        if (respawnSound != null) audioSource.PlayOneShot(respawnSound);

        // 8. Marcar como activo
        isActive = true;
    }

    // Detección de colisión con balas
    private void OnCollisionEnter(Collision collision)
    {
        if (isActive && collision.gameObject.CompareTag("Bullet"))
        {
            Destroy(collision.gameObject); // Destruir la bala
            TakeDamage();
        }
    }

    // Método para ser llamado por raycast
    public void RegisterHit()
    {
        TakeDamage();
    }
}