using System.Collections.Generic;
using UnityEngine;

public class wallFade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform playerTarget;
    [SerializeField] LayerMask fadeableLayers;       // Walls layer

    [Header("Materials")]
    [SerializeField] Material solidWallMaterial;        // Opaque version
    [SerializeField] Material transparentWallMaterial;  // Transparent version



    [Header("Fade Settings")]
    [Range(0f, 1f)]
    [SerializeField] float fadedAlpha = 0.2f;
    [SerializeField] float fadeSpeed = 10f;

    [Header("Aim")]
    [SerializeField] float aimHeight = 1.0f;

    // Stores original alpha value for each wall renderer
    Dictionary<Renderer, float> originalAlphaByRenderer = new Dictionary<Renderer, float>();
    Dictionary<Renderer, Material> originalMaterialByRenderer = new Dictionary<Renderer, Material>();



    // Renderers hit THIS frame
    HashSet<Renderer> blockingRenderersThisFrame = new HashSet<Renderer>();


    void LateUpdate()
    {
        if (!GameManager.instance.player) return;

        blockingRenderersThisFrame.Clear();

        Vector3 cameraPosition = transform.position;
        Vector3 playerAimPoint = GameManager.instance.player.transform.position + Vector3.up * aimHeight; // chest height
        Vector3 directionToPlayer = playerAimPoint - cameraPosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= 0.01f) return;

        Ray rayToPlayer = new Ray(cameraPosition, directionToPlayer.normalized);
        RaycastHit[] hits = Physics.RaycastAll(
            rayToPlayer,
            distanceToPlayer,
            fadeableLayers);

        // Fade walls currently blocking the view
        for (int i = 0; i < hits.Length; i++)
        {
            Renderer wallRenderer =
                hits[i].collider.GetComponentInChildren<Renderer>();

            if (!wallRenderer) continue;

            blockingRenderersThisFrame.Add(wallRenderer);

            if (!originalMaterialByRenderer.ContainsKey(wallRenderer))
                originalMaterialByRenderer[wallRenderer] = wallRenderer.sharedMaterial;

            if (!originalAlphaByRenderer.ContainsKey(wallRenderer))
                originalAlphaByRenderer[wallRenderer] = 1f;

            if (wallRenderer.sharedMaterial != transparentWallMaterial)
                wallRenderer.sharedMaterial = transparentWallMaterial;

            float currentAlpha = wallRenderer.sharedMaterial.color.a;
            float newAlpha = Mathf.Lerp(currentAlpha, fadedAlpha, fadeSpeed * Time.deltaTime);



            float fadedValue = Mathf.Lerp(
                wallRenderer.material.color.a,
                fadedAlpha,
                fadeSpeed * Time.deltaTime);

            SetRendererAlpha(wallRenderer, fadedValue);
        }

        // Restore walls no longer blocking the view
        List<Renderer> tracked = new List<Renderer>(originalMaterialByRenderer.Keys);


        for (int i = 0; i < tracked.Count; i++)
        {
            Renderer wallRenderer = tracked[i];

            if (!wallRenderer)
            {
                originalMaterialByRenderer.Remove(wallRenderer);
                originalAlphaByRenderer.Remove(wallRenderer);
                continue;
            }

            if (blockingRenderersThisFrame.Contains(wallRenderer))
                continue;

            float currentAlpha = wallRenderer.sharedMaterial.color.a;
            float newAlpha = Mathf.Lerp(currentAlpha, 1f, fadeSpeed * Time.deltaTime);

            SetRendererAlpha(wallRenderer, newAlpha);

            if (Mathf.Abs(newAlpha - 1f) < 0.01f)
            {
                wallRenderer.sharedMaterial = solidWallMaterial;
                originalMaterialByRenderer.Remove(wallRenderer);
                originalAlphaByRenderer.Remove(wallRenderer);
            }
        }
    }

    void SetRendererAlpha(Renderer renderer, float alpha)
    {
        Color color = renderer.material.color;
        color.a = alpha;
        renderer.material.color = color;
    }
}





