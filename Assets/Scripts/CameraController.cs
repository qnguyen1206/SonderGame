using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform target;
    [SerializeField] private bool autoFindPlayer = true;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10f);

    [Header("Boundary Settings")]
    [SerializeField] private bool useBoundaries = true;
    [SerializeField] private float mapMinX = -10f;
    [SerializeField] private float mapMaxX = 10f;
    [SerializeField] private float mapMinY = -10f;
    [SerializeField] private float mapMaxY = 10f;

    private Camera cam;
    private float minX, maxX, minY, maxY; // Actual camera boundaries (calculated)

    void Start()
    {
        cam = GetComponent<Camera>();

        // Auto-find player if enabled
        if (autoFindPlayer && target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                Debug.Log("Camera target (Player) automatically found!");
            }
            else
            {
                Debug.LogWarning("CameraController: No player found with 'Player' tag!");
            }
        }

        // Calculate actual camera boundaries accounting for camera size
        CalculateCameraBoundaries();
    }

    private void CalculateCameraBoundaries()
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Calculate camera half-extents
        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        // Adjust boundaries so camera edge (not center) stops at map edge
        minX = mapMinX + cameraHalfWidth;
        maxX = mapMaxX - cameraHalfWidth;
        minY = mapMinY + cameraHalfHeight;
        maxY = mapMaxY - cameraHalfHeight;

        Debug.Log($"Camera boundaries calculated: X({minX:F2} to {maxX:F2}), Y({minY:F2} to {maxY:F2})");
        Debug.Log($"Camera size: Width={cameraHalfWidth * 2:F2}, Height={cameraHalfHeight * 2:F2}");
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate desired position
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate to desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // Apply boundaries if enabled
        if (useBoundaries)
        {
            smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minX, maxX);
            smoothedPosition.y = Mathf.Clamp(smoothedPosition.y, minY, maxY);
        }

        // Keep the original Z position (camera depth)
        smoothedPosition.z = offset.z;

        transform.position = smoothedPosition;
    }

    // Helper method to set boundaries at runtime
    public void SetBoundaries(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
        useBoundaries = true;
    }

    // Helper method to calculate boundaries from tilemap or collider bounds
    public void SetBoundariesFromBounds(Bounds bounds)
    {
        if (cam == null) cam = GetComponent<Camera>();

        // Calculate camera half-extents
        float cameraHalfHeight = cam.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * cam.aspect;

        // Set boundaries accounting for camera size
        minX = bounds.min.x + cameraHalfWidth;
        maxX = bounds.max.x - cameraHalfWidth;
        minY = bounds.min.y + cameraHalfHeight;
        maxY = bounds.max.y - cameraHalfHeight;

        useBoundaries = true;

        Debug.Log($"Camera boundaries set: X({minX:F2} to {maxX:F2}), Y({minY:F2} to {maxY:F2})");
    }

    // Visualize boundaries in editor
    private void OnDrawGizmosSelected()
    {
        if (!useBoundaries) return;

        // Calculate boundaries for visualization
        if (cam == null) cam = GetComponent<Camera>();
        if (cam != null)
        {
            float cameraHalfHeight = cam.orthographicSize;
            float cameraHalfWidth = cameraHalfHeight * cam.aspect;

            float calcMinX = mapMinX + cameraHalfWidth;
            float calcMaxX = mapMaxX - cameraHalfWidth;
            float calcMinY = mapMinY + cameraHalfHeight;
            float calcMaxY = mapMaxY - cameraHalfHeight;

            // Draw camera center boundaries (cyan)
            Gizmos.color = Color.cyan;
            Vector3 topLeft = new Vector3(calcMinX, calcMaxY, 0);
            Vector3 topRight = new Vector3(calcMaxX, calcMaxY, 0);
            Vector3 bottomLeft = new Vector3(calcMinX, calcMinY, 0);
            Vector3 bottomRight = new Vector3(calcMaxX, calcMinY, 0);

            Gizmos.DrawLine(topLeft, topRight);
            Gizmos.DrawLine(topRight, bottomRight);
            Gizmos.DrawLine(bottomRight, bottomLeft);
            Gizmos.DrawLine(bottomLeft, topLeft);

            // Draw actual map boundaries (yellow)
            Gizmos.color = Color.yellow;
            Vector3 mapTopLeft = new Vector3(mapMinX, mapMaxY, 0);
            Vector3 mapTopRight = new Vector3(mapMaxX, mapMaxY, 0);
            Vector3 mapBottomLeft = new Vector3(mapMinX, mapMinY, 0);
            Vector3 mapBottomRight = new Vector3(mapMaxX, mapMinY, 0);

            Gizmos.DrawLine(mapTopLeft, mapTopRight);
            Gizmos.DrawLine(mapTopRight, mapBottomRight);
            Gizmos.DrawLine(mapBottomRight, mapBottomLeft);
            Gizmos.DrawLine(mapBottomLeft, mapTopLeft);
        }
    }
}
