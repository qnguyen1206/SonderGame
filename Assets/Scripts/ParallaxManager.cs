using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ParallaxLayer
{
    public Transform layerTransform;
    [Range(0f, 1f)] public float parallaxStrength = 0.5f;
    public float driftSpeed = 0.5f;
    public bool enableWrapping = true;
    public float wrapOffset = 20f;
    public Vector3 startPosition;
}

public class ParallaxManager : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Parallax Layers")]
    [SerializeField] private List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    [Header("Camera Settings")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private bool autoFindCamera = true;

    private Vector3 previousPlayerPosition;

    void Start()
    {
        // Auto-find camera if enabled
        if (autoFindCamera && mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // Auto-find player if enabled
        if (autoFindPlayer && player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
                Debug.Log("Player automatically found for parallax!");
            }
            else
            {
                Debug.LogWarning("ParallaxManager: No player found with 'Player' tag!");
            }
        }

        if (player != null)
        {
            previousPlayerPosition = player.position;
        }

        // Store initial positions of all layers
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                layer.startPosition = layer.layerTransform.position;
            }
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Calculate how much the player has moved
        Vector3 playerMovement = player.position - previousPlayerPosition;

        // Apply parallax effect to each layer
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                // Move the layer based on parallax strength
                // Lower strength = further away = moves less
                Vector3 parallaxOffset = playerMovement * layer.parallaxStrength;
                
                // Add constant drift to the left
                Vector3 driftOffset = Vector3.left * layer.driftSpeed * Time.deltaTime;
                
                layer.layerTransform.position += parallaxOffset + driftOffset;

                // Handle wrapping
                if (layer.enableWrapping && mainCamera != null)
                {
                    WrapLayer(layer);
                }
            }
        }

        // Update previous position for next frame
        previousPlayerPosition = player.position;
    }

    private void WrapLayer(ParallaxLayer layer)
    {
        // Get the camera bounds
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        
        Vector3 cameraPosition = mainCamera.transform.position;
        float leftEdge = cameraPosition.x - (cameraWidth / 2f);
        float rightEdge = cameraPosition.x + (cameraWidth / 2f);

        // Check if layer has moved past the left edge of the screen
        if (layer.layerTransform.position.x < leftEdge - layer.wrapOffset)
        {
            // Wrap to the right side
            Vector3 newPos = layer.layerTransform.position;
            newPos.x = rightEdge + layer.wrapOffset;
            layer.layerTransform.position = newPos;
        }
        // Optional: wrap from right to left if player moves right fast
        else if (layer.layerTransform.position.x > rightEdge + layer.wrapOffset)
        {
            Vector3 newPos = layer.layerTransform.position;
            newPos.x = leftEdge - layer.wrapOffset;
            layer.layerTransform.position = newPos;
        }
    }

    // Helper method to add a layer at runtime
    public void AddParallaxLayer(Transform layerTransform, float parallaxStrength)
    {
        ParallaxLayer newLayer = new ParallaxLayer
        {
            layerTransform = layerTransform,
            parallaxStrength = parallaxStrength,
            startPosition = layerTransform.position
        };
        parallaxLayers.Add(newLayer);
    }

    // Reset all layers to their starting positions
    public void ResetLayers()
    {
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerTransform != null)
            {
                layer.layerTransform.position = layer.startPosition;
            }
        }

        if (player != null)
        {
            previousPlayerPosition = player.position;
        }
    }
}
