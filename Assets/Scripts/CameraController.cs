using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float zOffset = -1.5f; // Additional zOffset around the objects
    public float smoothSpeed = 0.125f; // Speed of camera movement

    private GridManager gridManager; // Reference to the GridManager

    private void Start()
    {
        gridManager = GridManager.Instance;
    }

    private void LateUpdate()
    {
        // Calculate the bounding box of all objects in the grid
        Bounds bounds = CalculateBoundingBox();

        // Calculate the target position based on the bounding box
        Vector3 targetPosition = bounds.center;
        targetPosition.y = transform.position.y; // Keep the same y position as the camera

        // Calculate desired camera position with zOffset
        Vector3 desiredPosition = targetPosition - Vector3.forward * (bounds.extents.magnitude + zOffset);

        // Smoothly move the camera towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
    }

    private Bounds CalculateBoundingBox()
    {
        if (gridManager == null)
        {
            Debug.LogWarning("GridManager not found in the scene.");
            return new Bounds();
        }

        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool initialized = false;

        // Loop through all grid squares and include their positions in the bounding box
        foreach (GameObject gridSquare in gridManager.GetGridSquares())
        {
            if (!initialized)
            {
                bounds = new Bounds(gridSquare.transform.position, Vector3.zero);
                initialized = true;
            }
            else
            {
                bounds.Encapsulate(gridSquare.transform.position);
            }
        }

        return bounds;
    }
}
