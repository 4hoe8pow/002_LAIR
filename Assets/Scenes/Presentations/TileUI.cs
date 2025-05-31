using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TileUI : MonoBehaviour
{
    // グリッド上の位置を格納する変数
    public int gridX;
    public int gridY;

    // GridManager から呼び出される初期化メソッド
    public void Initialize(int x, int y, Sprite tileSprite = null)
    {
        gridX = x;
        gridY = y;

        // 任意でスプライトや色を切り替えたい場合
        if (tileSprite != null)
        {
            Image img = GetComponent<Image>();
            img.sprite = tileSprite;
            img.preserveAspect = true;
        }
    }

    // タイルを新座標 (newX, newY) へ瞬時移動（スナップ）させるメソッド
    public void MoveTo(int newX, int newY)
    {
        gridX = newX;
        gridY = newY;
    }

    // アニメーションでグリッド座標(x, y)へ移動（完了時コールバック付き）
    public IEnumerator AnimateToGridPosition(int x, int y, float duration, float cellWidth, float cellHeight, float spacing, System.Action onComplete = null)
    {
        if (!TryGetComponent<RectTransform>(out var rect))
        {
            Debug.LogError($"RectTransform is null on {gameObject.name}");
            onComplete?.Invoke();
            yield break;
        }
        Vector2 start = rect.anchoredPosition;
        Vector2 end = new(x * (cellWidth + spacing), -y * (cellHeight + spacing));
        float elapsed = 0f;
        while (elapsed < duration)
        {
            rect.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        rect.anchoredPosition = end;
        rect.sizeDelta = new Vector2(cellWidth, cellHeight);
        onComplete?.Invoke();
    }
}
