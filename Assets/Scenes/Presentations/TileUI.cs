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
}
