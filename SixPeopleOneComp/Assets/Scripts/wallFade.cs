using System.Collections.Generic;
using UnityEngine;

public class wallFade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform playerTarget;     // Player transform
    [SerializeField] LayerMask wallLayer;        // Walls layer

    [Header("Fade Settings")]
    [Range(0f, 1f)]
    [SerializeField] float fadedAlpha = 0.2f;
    [SerializeField] float fadeSpeed = 10f;

    // Stores original alpha value for each wall renderer
    Dictionary<Renderer, float> originalAlphaByRenderer = new Dictionary<Renderer, float>();

    // Renderers hit THIS frame
    HashSet<Renderer> blockingRenderersThisFrame = new HashSet<Renderer>();


    void LateUpdate()
    {
        if (!playerTarget) return;

        blockingRenderersThisFrame.Clear();

        Vector3 cameraPosition = transform.position;
        Vector3 playerAimPoint = playerTarget.position + Vector3.up * 1.0f; // chest height
        Vector3 directionToPlayer = playerAimPoint - cameraPosition;
        float distanceToPlayer = directionToPlayer.magnitude;

        if (distanceToPlayer <= 0.01f) return;

        Ray rayToPlayer = new Ray(cameraPosition, directionToPlayer.normalized);
        RaycastHit[] hits = Physics.RaycastAll(
            rayToPlayer,
            distanceToPlayer,
            wallLayer);

        // Fade walls currently blocking the view
        for (int i = 0; i < hits.Length; i++)
        {
            Renderer wallRenderer =
                hits[i].collider.GetComponentInChildren<Renderer>();

            if (!wallRenderer) continue;

            blockingRenderersThisFrame.Add(wallRenderer);

            if (!originalAlphaByRenderer.ContainsKey(wallRenderer))
            {
                originalAlphaByRenderer[wallRenderer] =
                    wallRenderer.material.color.a;
            }

            float fadedValue = Mathf.Lerp(
                wallRenderer.material.color.a,
                fadedAlpha,
                fadeSpeed * Time.deltaTime);

            SetRendererAlpha(wallRenderer, fadedValue);
        }

        // Restore walls no longer blocking the view
        List<Renderer> trackedRenderers =
            new List<Renderer>(originalAlphaByRenderer.Keys);

        for (int i = 0; i < trackedRenderers.Count; i++)
        {
            Renderer wallRenderer = trackedRenderers[i];

            if (!wallRenderer)
            {
                originalAlphaByRenderer.Remove(wallRenderer);
                continue;
            }

            if (blockingRenderersThisFrame.Contains(wallRenderer))
                continue;

            float originalAlpha = originalAlphaByRenderer[wallRenderer];

            float restoredValue = Mathf.Lerp(
                wallRenderer.material.color.a,
                originalAlpha,
                fadeSpeed * Time.deltaTime);

            SetRendererAlpha(wallRenderer, restoredValue);

            if (Mathf.Abs(restoredValue - originalAlpha) < 0.01f)
            {
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





