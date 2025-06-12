using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bullet : MonoBehaviour
{
    public bool drawDebugRay = true;
    public Color debugRayColor = Color.red;
    public float debugRayDuration = 1f;

    private int frameBuffer = 2;
    private int currentFrame = 0;
    private Vector3 previousPosition;
    private Material debugMaterial;

    void Start()
    {
        previousPosition = transform.position;
        CreateDebugMaterial();
    }

    void Update()
    {
        if (currentFrame < frameBuffer)
        {
            currentFrame++;
            return;
        }
        currentFrame = 0;

        if (drawDebugRay)
        {
            Debug.DrawRay(previousPosition, transform.position - previousPosition, debugRayColor, debugRayDuration);
        }
        previousPosition = transform.position;
    }

    void OnRenderObject()
    {
        if (!drawDebugRay) return;

        debugMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(debugRayColor);
        GL.Vertex(previousPosition);
        GL.Vertex(transform.position);
        GL.End();
    }

    private void CreateDebugMaterial()
    {
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        debugMaterial = new Material(shader);
        debugMaterial.hideFlags = HideFlags.HideAndDontSave;
        debugMaterial.SetInt("_SrcBlend", (int)BlendMode.SrcAlpha);
        debugMaterial.SetInt("_DstBlend", (int)BlendMode.OneMinusSrcAlpha);
        debugMaterial.SetInt("_Cull", (int)CullMode.Off);
        debugMaterial.SetInt("_ZWrite", 0);
    }

    private void OnCollisionEnter(Collision objectWeHit)
    {
        if (objectWeHit.gameObject.CompareTag("Target") ||
            objectWeHit.gameObject.CompareTag("Wall"))
        {
            CreateBulletImpactEffect(objectWeHit);
            Destroy(gameObject);
        }
        else if (objectWeHit.gameObject.CompareTag("Beer"))
        {
            objectWeHit.gameObject.GetComponent<BeerBottle>().Shatter();
            Destroy(gameObject);
        }
    }

    void CreateBulletImpactEffect(Collision objectWeHit)
    {
        ContactPoint contact = objectWeHit.contacts[0];
        GameObject hole = Instantiate(
            GlobalReferences.Instance.bulletImpactEffectPrefab,
            contact.point,
            Quaternion.LookRotation(contact.normal)
        );
        hole.transform.SetParent(objectWeHit.gameObject.transform);
    }

    void OnDestroy()
    {
        if (debugMaterial != null)
        {
            Destroy(debugMaterial);
        }
    }
}