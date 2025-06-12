using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BeerBottle : MonoBehaviour
{
    public List<Rigidbody> allParts = new List<Rigidbody>();
    private bool isShattered = false;
    private int frameBuffer = 2;
    private int currentFrame = 0;
    private Material debugMaterial;
    public bool drawDebugOutline = true;
    public Color debugColor = Color.green;

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
    }

    void OnRenderObject()
    {
        if (!drawDebugOutline || isShattered) return;

        debugMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(debugColor);

        // Simple bounding box visualization
        Vector3 size = GetComponent<Collider>().bounds.size;
        Vector3 center = GetComponent<Collider>().bounds.center;

        DrawWireCube(center, size);
        GL.End();
    }

    void DrawWireCube(Vector3 center, Vector3 size)
    {
        Vector3 halfSize = size * 0.5f;

        // Bottom square
        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));

        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));

        // Vertical edges
        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, -halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, halfSize.z));

        GL.Vertex(center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));

        // Top square
        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, -halfSize.z));
        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, halfSize.z));

        GL.Vertex(center + new Vector3(halfSize.x, halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));

        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, halfSize.z));
        GL.Vertex(center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z));
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

    public void Shatter()
    {
        if (isShattered) return;

        isShattered = true;
        foreach (var part in allParts)
        {
            part.isKinematic = false;
        }
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 5f);
    }

    void OnDestroy()
    {
        if (debugMaterial != null)
        {
            Destroy(debugMaterial);
        }
    }
}