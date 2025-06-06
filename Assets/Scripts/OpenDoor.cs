using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    public float speed;  // Corregí el nombre "spreed" a "speed"
    public float angle;
    public Vector3 direction;
    public bool isOpen;

    void Start()
    {
        angle = transform.eulerAngles.y;
    }

    void Update()
    {
        // Rotación suave hacia el ángulo objetivo
        if (Mathf.Round(transform.eulerAngles.y) != angle)
        {
            transform.Rotate(direction * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.E) && isOpen)
        {
            if (Mathf.Approximately(angle, 0))
            {
                angle = 80; // Ángulo abierto
            }
            else
            {
                angle = 0; // Ángulo cerrado
            }
            direction = Vector3.up;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isOpen = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isOpen = false;
        }
    }
}