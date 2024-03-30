using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 offset; 
    public float smoothSpeed = 0.125f; 

    private GridManager gridManager; 

    private void Start()
    {
        gridManager = GridManager.Instance;
    }

    private void LateUpdate()
    {
        // Calculate the bounding box of all objects in the grid
        Bounds bounds = CalculateBoundingBox();

        Vector3 targetPosition = bounds.center;
        Vector3 desiredPosition = targetPosition + Vector3.left * offset.x + Vector3.up * offset.y + Vector3.forward * offset.z;

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
        foreach (Factory gridSquare in gridManager.GetGridSquares())
        {
            if (!initialized)
            {
                bounds = new Bounds(gridSquare.gameObject.transform.position, Vector3.zero);
                initialized = true;
            }
            else
            {
                bounds.Encapsulate(gridSquare.gameObject.transform.position);
            }
        }

        return bounds;
    }
}
