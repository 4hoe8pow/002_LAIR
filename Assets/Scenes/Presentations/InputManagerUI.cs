using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManagerUI : MonoBehaviour
{
    // タッチ開始時に選択したタイル UI
    private TileUI selectedTile = null;

    // スワイプ検出用の座標
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;

    // スワイプ判定の最小距離（ピクセル）
    [SerializeField]
    private float minSwipeDistance = 50f;

    // 参照：GraphicRaycaster と PointerEventData（UI レイキャストに利用）
    private GraphicRaycaster _graphicRaycaster;
    private EventSystem _eventSystem;

    private void Awake()
    {
        // Canvas（または同じ階層）の GraphicRaycaster を取得
        _graphicRaycaster = GetComponent<GraphicRaycaster>();
        if (_graphicRaycaster == null)
        {
            Debug.LogError("InputManagerUI をアタッチしたオブジェクトに GraphicRaycaster が見つかりません。");
        }

        _eventSystem = EventSystem.current;
        if (_eventSystem == null)
        {
            Debug.LogError("EventSystem がシーン上に見つかりません。必ず配置してください。");
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        // エディタ上ではマウス入力をタッチに見立てる
        if (Input.GetMouseButtonDown(0))
        {
            startTouchPos = Input.mousePosition;
            selectedTile = GetTileUnderPointer(startTouchPos);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            endTouchPos = Input.mousePosition;
            HandleSwipe();
        }
#else
        // 実機：タッチ入力を扱う
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startTouchPos = touch.position;
                selectedTile = GetTileUnderPointer(startTouchPos);
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                endTouchPos = touch.position;
                HandleSwipe();
            }
        }
#endif
    }

    /// <summary>
    /// 指定スクリーン座標上にある TileUI を返す。存在しない場合は null。
    /// </summary>
    private TileUI GetTileUnderPointer(Vector2 screenPos)
    {
        PointerEventData ped = new(_eventSystem)
        {
            position = screenPos
        };

        List<RaycastResult> results = new();
        _graphicRaycaster.Raycast(ped, results);

        foreach (var result in results)
        {
            // 先に TileUI コンポーネントを持っているかチェック
            TileUI tile = result.gameObject.GetComponent<TileUI>();
            if (tile != null)
            {
                return tile;
            }
        }
        return null;
    }

    /// <summary>
    /// スワイプの方向を判定し、隣接していれば GridManagerUI.SwapWithEmpty() を呼び出す。
    /// </summary>
    private void HandleSwipe()
    {
        if (selectedTile == null)
        {
            // タイル上でタッチされていない、もしくはクリックが小さい動きだった場合
            selectedTile = null;
            return;
        }

        Vector2 delta = endTouchPos - startTouchPos;
        if (delta.magnitude < minSwipeDistance)
        {
            // 微小なタッチ → スワイプとみなさない
            selectedTile = null;
            return;
        }

        // スワイプ方向ベクトルを単位化して、上下左右のいずれかに分類
        Vector2 dir = delta.normalized;
        Vector2Int targetPos = new(selectedTile.gridX, selectedTile.gridY);

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            // 横方向のスワイプ
            if (dir.x > 0)
            {
                // 右スワイプ
                targetPos.x += 1;
            }
            else
            {
                // 左スワイプ
                targetPos.x -= 1;
            }
        }
        else
        {
            // 縦方向のスワイプ
            if (dir.y > 0)
            {
                // 上スワイプ
                targetPos.y -= 1;
            }
            else
            {
                // 下スワイプ
                targetPos.y += 1;
            }
        }

        // 目標セル位置が有効範囲内かチェック
        if (targetPos.x < 0 || targetPos.x >= GridManagerUI.Instance.gridSize ||
            targetPos.y < 0 || targetPos.y >= GridManagerUI.Instance.gridSize)
        {
            // グリッド外を参照しているため無効
            selectedTile = null;
            return;
        }

        // targetPos が空白セルかチェックし、隣接であればスワップ
        Vector2Int empty = GridManagerUI.Instance.emptyCell;
        if (empty.x == targetPos.x && empty.y == targetPos.y)
        {
            // 選択タイル座標 → (selectedTile.gridX, selectedTile.gridY)
            GridManagerUI.Instance.SwapWithEmpty(selectedTile.gridX, selectedTile.gridY);
        }

        // 判定後は selectedTile をクリア
        selectedTile = null;
    }
}
