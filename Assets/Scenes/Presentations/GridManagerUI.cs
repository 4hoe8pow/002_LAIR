using UnityEngine;

public class GridManagerUI : MonoBehaviour
{
    // シングルトン化
    public static GridManagerUI Instance { get; private set; }

    [Header("Grid 設定")]
    [Tooltip("グリッドのマス数（例：3 → 3x3）")]
    public int gridSize = 3;

    [Tooltip("セル間のスペース（ピクセル）")]
    public float spacing = 0f;

    [Header("Prefab / 親オブジェクト参照")]
    [Tooltip("タイルの UI Prefab（Image もしくは Button）")]
    public GameObject tilePrefab;

    // タイル配置を保持する 2D 配列（空白セルは null）
    [HideInInspector]
    public TileUI[,] gridTiles;

    // 空白セルの現在座標を保持
    [HideInInspector]
    public Vector2Int emptyCell;

    private RectTransform _rectTransform;

    private void Awake()
    {
        InitializeSingleton();
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        InitializeGridArray();
        InitializeEmptyCell();
        CreateAndPlaceTiles();
    }

    private void InitializeSingleton()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("GridManagerUI が既に存在します。重複したインスタンスを破棄します。");
            Destroy(gameObject);
        }
    }

    private void InitializeGridArray()
    {
        gridTiles = new TileUI[gridSize, gridSize];
    }

    private void InitializeEmptyCell()
    {
        emptyCell = new Vector2Int(gridSize - 1, gridSize - 1);
    }

    private (float cellWidth, float cellHeight) CalculateCellSize()
    {
        float parentWidth = _rectTransform.rect.width;
        float parentHeight = _rectTransform.rect.height;
        float cellWidth = (parentWidth - spacing * (gridSize - 1)) / gridSize;
        float cellHeight = (parentHeight - spacing * (gridSize - 1)) / gridSize;
        return (cellWidth, cellHeight);
    }

    private void SetRectTransformToGrid(RectTransform rect, int x, int y, float cellWidth, float cellHeight)
    {
        SetRectTransformAnchorPivot(rect);
        rect.sizeDelta = new Vector2(cellWidth, cellHeight);
        float posX = x * (cellWidth + spacing);
        float posY = -y * (cellHeight + spacing);
        rect.anchoredPosition = new Vector2(posX, posY);
    }

    private void SetRectTransformAnchorPivot(RectTransform rect)
    {
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.pivot = new Vector2(0, 1);
    }

    private void CreateAndPlaceTiles()
    {
        var (cellWidth, cellHeight) = CalculateCellSize();
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (x == emptyCell.x && y == emptyCell.y) continue;
                GameObject tileObj = Instantiate(tilePrefab, _rectTransform);
                RectTransform tileRect = tileObj.GetComponent<RectTransform>();
                if (tileRect != null)
                {
                    SetRectTransformToGrid(tileRect, x, y, cellWidth, cellHeight);
                }
                TileUI tileUI = tileObj.GetComponent<TileUI>();
                if (tileUI == null)
                {
                    Debug.LogError("tilePrefab に TileUI スクリプトがアタッチされていません。");
                    continue;
                }
                gridTiles[x, y] = tileUI;
                tileUI.Initialize(x, y);
            }
        }
    }

    /// <summary>
    /// (x, y) のタイルが空白セルと隣接しているか判定する。
    /// </summary>
    public bool IsAdjacentToEmpty(int x, int y)
    {
        // マンハッタン距離が 1 なら隣接
        if (Mathf.Abs(emptyCell.x - x) + Mathf.Abs(emptyCell.y - y) == 1)
            return true;
        return false;
    }

    /// <summary>
    /// (x, y) のタイルを空白セルの位置へスナップ移動し、空白セル座標を更新する。
    /// </summary>
    public void SwapWithEmpty(int x, int y)
    {
        // 隣接しているかどうかをチェック
        if (!IsAdjacentToEmpty(x, y))
        {
            Debug.LogWarning($"({x},{y}) のタイルは空白セルと隣接していないため移動できません。");
            return;
        }

        // 移動対象タイルを取得
        TileUI movingTile = gridTiles[x, y];
        if (movingTile == null)
        {
            Debug.LogError($"座標 ({x},{y}) にタイルが存在しません。");
            return;
        }

        // 空白セル座標を保持
        Vector2Int previousEmpty = emptyCell;

        // gridTiles 配列の位置を入れ替え（データ即時更新）
        gridTiles[emptyCell.x, emptyCell.y] = movingTile;
        gridTiles[x, y] = null;

        // タイルのgridX,gridYも即時更新
        movingTile.MoveTo(previousEmpty.x, previousEmpty.y);

        // 空白セル座標を移動前のタイル位置へ更新
        emptyCell = new Vector2Int(x, y);

        // アニメーションで見た目を移動（非同期）
        var (cellWidth, cellHeight) = CalculateCellSize();
        RectTransform tileRect = movingTile.GetComponent<RectTransform>();
        if (tileRect != null)
        {
            StartCoroutine(movingTile.AnimateToGridPosition(
                movingTile.gridX, movingTile.gridY,
                0.2f, cellWidth, cellHeight, spacing, null));
        }
    }
}
