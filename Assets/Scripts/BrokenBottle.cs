using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class BrokenBottle : MonoBehaviour
{
    [SerializeField] GameObject[] pieces;
    [SerializeField] float velMultiplier = 2f;
    [SerializeField] float timeBeforeDestroying = 60f;
    private int frameBuffer = 2;
    private int currentFrame = 0;
    private Material debugMaterial;
    public bool drawTrajectories = true;
    public Color trajectoryColor = new Color(1, 0.5f, 0, 0.5f); // Orange with transparency

    void Start()
    {
        CreateDebugMaterial();
        Destroy(gameObject, timeBeforeDestroying);
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
        if (!drawTrajectories) return;

        debugMaterial.SetPass(0);
        GL.Begin(GL.LINES);
        GL.Color(trajectoryColor);

        foreach (var piece in pieces)
        {
            if (piece == null) continue;

            Rigidbody rb = piece.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 start = piece.transform.position;
            Vector3 end = start + rb.velocity * 0.2f; // Short prediction

            GL.Vertex(start);
            GL.Vertex(end);
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

    public void RandomVelocities()
    {
        for (int i = 0; i < pieces.Length; i++)
        {
            if (pieces[i] == null) continue;

            float xVel = Random.Range(0f, 1f);
            float yVel = Random.Range(0f, 1f);
            float zVel = Random.Range(0f, 1f);
            Vector3 vel = new Vector3(velMultiplier * xVel, velMultiplier * yVel, velMultiplier * zVel);

            Rigidbody rb = pieces[i].GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = vel;
            }
        }
    }

    void OnDestroy()
    {
        if (debugMaterial != null)
        {
            Destroy(debugMaterial);
        }
    }
}