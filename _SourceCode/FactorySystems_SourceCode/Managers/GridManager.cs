using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] private Factory gridSquarePrefab; // Prefab for the grid square
    [SerializeField] private int rows = 10; // Number of rows
    [SerializeField] private int columns = 10; // Number of columns
    [SerializeField] private float gridSize = 1.1f; // Size of each grid square

    private Factory[,] grid; // 2D array to store grid squares

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
        // Mouse Input
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
        // Touch Input
        else if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0); // Get the first touch

            if (touch.phase == TouchPhase.Began)
            {
                HandleInput(touch.position);
            }
        }
    }

    private void HandleInput(Vector3 position)
    {
        Ray ray = Camera.main.ScreenPointToRay(position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 clickPosition = hit.point;
            Vector2Int gridCoordinate = WorldToGridPosition(clickPosition);
            Factory clickedObject = GetObjectAtCoordinate(gridCoordinate);
            if (clickedObject != null)
            {
                GameEvents.OnFactoryClicked?.Invoke(clickedObject);
            }
            else
            {
                Debug.LogError("No object found at clicked position.");
            }
        }
    }
    #endregion

    #region Public API
    public void PlaceObjectAtCoordinate(Factory obj, Vector2Int coordinate)
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

    public Factory GetObjectAtCoordinate(Vector2Int coordinate)
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
    public Factory[,] GetGrid()
    {
        return grid;
    }

    public Factory[] GetGridSquares()
    {
        int totalGridSquares = rows * columns;
        Factory[] gridSquares = new Factory[totalGridSquares];
        int index = 0;

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
        grid = new Factory[columns, rows]; // Initialize the grid array

        // Loop through each row and column to create grid squares
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Calculate position for the grid square
                Vector3 position = new Vector3(x * gridSize, 0f, y * gridSize);

                // Instantiate the grid square prefab at the calculated position
                grid[x, y] = Instantiate(gridSquarePrefab, position, Quaternion.identity);
                FactoryManager.Instance.RegisterFactory(grid[x, y]);
            }
        }

        FactoryManager.Instance.StartGame();
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
