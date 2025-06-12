using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Bottle : MonoBehaviour
{
    [SerializeField] GameObject brokenBottlePrefab;
    private int frameBuffer = 2;
    private int currentFrame = 0;
    private Material debugMaterial;
    public bool drawExplosionRadius = false;
    public float explosionRadius = 1.5f;
    public Color explosionRadiusColor = new Color(1, 0, 0, 0.3f); // Red with transparency

    void Start()
    {
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            Explode();
        }
    }

    void OnRenderObject()
    {
        if (!drawExplosionRadius) return;

        debugMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(explosionRadiusColor);

        // Draw a circle representing explosion radius
        int segments = 32;
        float anglePerSegment = 2 * Mathf.PI / segments;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * anglePerSegment;
            float angle2 = (i + 1) % segments * anglePerSegment;

            Vector3 pos1 = transform.position + new Vector3(
                Mathf.Cos(angle1) * explosionRadius,
                0,
                Mathf.Sin(angle1) * explosionRadius);

            Vector3 pos2 = transform.position + new Vector3(
                Mathf.Cos(angle2) * explosionRadius,
                0,
                Mathf.Sin(angle2) * explosionRadius);

            GL.Vertex(pos1);
            GL.Vertex(pos2);
        }
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

    void Explode()
    {
        GameObject brokenBottle = Instantiate(brokenBottlePrefab, transform.position, Quaternion.identity);
        brokenBottle.GetComponent<BrokenBottle>().RandomVelocities();
        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (debugMaterial != null)
        {
            Destroy(debugMaterial);
        }
    }
}