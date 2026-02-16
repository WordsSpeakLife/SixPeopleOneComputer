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

    [Header("Layers")]
    [SerializeField] int fadedWallLayer = 21;

    // Renderers hit THIS frame
    Dictionary<Renderer, Material> originalMaterialByRenderer = new Dictionary<Renderer, Material>();
    HashSet<Renderer> blockingRenderersThisFrame = new HashSet<Renderer>();

    Dictionary<Collider, int> originalLayerByCollider = new Dictionary<Collider, int>();

    // Stores original alpha value for each wall renderer
    Dictionary<Renderer, Collider> rendererToCollider = new Dictionary<Renderer, Collider>();

    void Start()
    {
        Debug.Log("fadeableLayers mask value = " + fadeableLayers.value);
    }
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

            Collider hitCol = hits[i].collider;
            if (!hitCol) continue;

            Renderer wallRenderer = hitCol.GetComponent<Renderer>();
            if (!wallRenderer)
                wallRenderer = hitCol.GetComponentInChildren<Renderer>();

            if (!wallRenderer) continue;

            blockingRenderersThisFrame.Add(wallRenderer);



            if (!originalMaterialByRenderer.ContainsKey(wallRenderer))
                originalMaterialByRenderer[wallRenderer] = wallRenderer.sharedMaterial;

            if (!originalLayerByCollider.ContainsKey(hitCol))
                originalLayerByCollider[hitCol] = hitCol.gameObject.layer;

            hitCol.gameObject.layer = fadedWallLayer;

            if (!rendererToCollider.ContainsKey(wallRenderer))
                rendererToCollider[wallRenderer] = hitCol;

            if (wallRenderer.sharedMaterial != transparentWallMaterial)
                wallRenderer.sharedMaterial = transparentWallMaterial;

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
                rendererToCollider.Remove(wallRenderer);
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

                if (rendererToCollider.TryGetValue(wallRenderer, out Collider col) && col)
                {
                    if (originalLayerByCollider.TryGetValue(col, out int originalLayer))
                    {
                        col.gameObject.layer = originalLayer;
                        originalLayerByCollider.Remove(col);
                    }
                }

                rendererToCollider.Remove(wallRenderer);
                originalMaterialByRenderer.Remove(wallRenderer);

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






