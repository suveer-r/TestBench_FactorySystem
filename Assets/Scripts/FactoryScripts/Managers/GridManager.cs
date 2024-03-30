using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] private GameObject gridSquarePrefab; // Prefab for the grid square
    [SerializeField] private int rows = 10; // Number of rows
    [SerializeField] private int columns = 10; // Number of columns
    [SerializeField] private float gridSize = 1.1f; // Size of each grid square

    private GameObject[,] grid; // 2D array to store grid squares

    public int Rows { get => rows; }
    public int Columns { get => columns; }
    public float GridSize { get => gridSize; }

    #region Monobehaviour 
    private void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 clickPosition = hit.point;
                Vector2Int gridCoordinate = WorldToGridPosition(clickPosition);
                Debug.Log("Clicked at grid coordinate: " + gridCoordinate);
                GameObject clickedObject = GetObjectAtCoordinate(gridCoordinate);
                if (clickedObject != null)
                {
                    Debug.Log("Object at clicked position: " + clickedObject.name);
                }
                else
                {
                    Debug.Log("No object found at clicked position.");
                }
            }
        }
    }
    #endregion

    #region Public API
    public void PlaceObjectAtCoordinate(GameObject obj, Vector2Int coordinate)
    {
        if (IsValidCoordinate(coordinate))
        {
            Destroy(grid[coordinate.x, coordinate.y]); // Destroy any existing object at the coordinate
            grid[coordinate.x, coordinate.y] = Instantiate(obj, GridToWorldPosition(coordinate), Quaternion.identity);
        }
        else
        {
            Debug.LogError("Invalid coordinate provided.");
        }
    }

    public GameObject GetObjectAtCoordinate(Vector2Int coordinate)
    {
        if (IsValidCoordinate(coordinate))
        {
            return grid[coordinate.x, coordinate.y];
        }
        else
        {
            Debug.LogError("Invalid coordinate provided.");
            return null;
        }
    }
    public GameObject[,] GetGrid()
    {
        return grid;
    }

    public GameObject[] GetGridSquares()
    {
        int totalGridSquares = rows * columns;
        GameObject[] gridSquares = new GameObject[totalGridSquares];
        int index = 0;

        // Loop through each grid square and add it to the array
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                gridSquares[index] = grid[x, y];
                index++;
            }
        }

        return gridSquares;
    }
    #endregion

    #region Private Methods
    private void GenerateGrid()
    {
        grid = new GameObject[columns, rows]; // Initialize the grid array

        // Loop through each row and column to create grid squares
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Calculate position for the grid square
                Vector3 position = new Vector3(x * gridSize, 0f, y * gridSize);

                // Instantiate the grid square prefab at the calculated position
                grid[x, y] = Instantiate(gridSquarePrefab, position, Quaternion.identity);
            }
        }
    }

    private Vector2Int WorldToGridPosition(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt(worldPosition.x / gridSize);
        int y = Mathf.FloorToInt(worldPosition.z / gridSize);
        return new Vector2Int(x, y);
    }

    private bool IsValidCoordinate(Vector2Int coordinate)
    {
        return coordinate.x >= 0 && coordinate.x < columns && coordinate.y >= 0 && coordinate.y < rows;
    }

    private Vector3 GridToWorldPosition(Vector2Int coordinate)
    {
        return new Vector3(coordinate.x * gridSize, 0f, coordinate.y * gridSize);
    }
    #endregion

}
