using Application;
using Domain;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

namespace Presentation
{
    // OutputPort: UIアニメーションや表示を担当
    public class PuzzlePresenter : MonoBehaviour, IPuzzleOutputPort
    {
        public GameObject tilePrefab;
        public RectTransform boardParent;
        public float tileSpacing = 0f;
        [HideInInspector] public Dictionary<TileAddress, GameObject> TileObjects = new();
        public TextMeshProUGUI validTestimonyIndicator;

        public void MoveTile(TileAddress from, TileAddress to)
        {
            if (!TileObjects.TryGetValue(from, out var tileObj)) return;
            TileObjects.Remove(from);
            TileObjects[to] = tileObj;
            StartCoroutine(AnimateTileMove(tileObj.GetComponent<RectTransform>(), to));
        }

        public void OnPuzzleInitialized(PuzzleBoard board)
        {
            foreach (var obj in TileObjects.Values)
                Destroy(obj);
            TileObjects.Clear();
            foreach (var tile in board.GetAllTiles())
            {
                var obj = Instantiate(tilePrefab, boardParent);
                var rect = obj.GetComponent<RectTransform>();
                SetRectTransformToGrid(rect, tile.Address, board.Size);
                TileObjects[tile.Address] = obj;
            }
        }

        private void SetRectTransformToGrid(RectTransform rect, TileAddress address, int gridSize)
        {
            float parentWidth = boardParent.rect.width;
            float parentHeight = boardParent.rect.height;
            float spacing = tileSpacing; // パラメータ化
            float cellWidth = (parentWidth - spacing * (gridSize - 1)) / gridSize;
            float cellHeight = (parentHeight - spacing * (gridSize - 1)) / gridSize;
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            float posX = address.X * (cellWidth + spacing);
            float posY = -address.Y * (cellHeight + spacing);
            rect.anchoredPosition = new Vector2(posX, posY);
            rect.sizeDelta = new Vector2(cellWidth, cellHeight);
        }

        private System.Collections.IEnumerator AnimateTileMove(RectTransform rect, TileAddress to)
        {
            float duration = 0.1f;
            float parentWidth = boardParent.rect.width;
            float parentHeight = boardParent.rect.height;
            int gridSize = (int)Mathf.Sqrt(TileObjects.Count + 1);
            float spacing = tileSpacing;
            float cellWidth = (parentWidth - spacing * (gridSize - 1)) / gridSize;
            float cellHeight = (parentHeight - spacing * (gridSize - 1)) / gridSize;
            Vector2 start = rect.anchoredPosition;
            Vector2 end = new(to.X * (cellWidth + spacing), -to.Y * (cellHeight + spacing));
            float elapsed = 0f;
            while (elapsed < duration)
            {
                rect.anchoredPosition = Vector2.Lerp(start, end, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            rect.anchoredPosition = end;
            rect.sizeDelta = new Vector2(cellWidth, cellHeight);
        }

        // タイルの証言テキストを更新する
        public void UpdateTileTestimonyText(Tile tile)
        {
            if (!TileObjects.TryGetValue(tile.Address, out var obj)) return;
            var text = obj.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null)
                text.text = tile.Testimony.ToJapaneseString();
        }

        // 全タイルの証言テキストを一括更新する
        public void UpdateAllTileTestimonyTexts(PuzzleBoard board)
        {
            foreach (var tile in board.GetAllTiles())
            {
                UpdateTileTestimonyText(tile);
            }
        }

        public void ShowValidTestimonyCount(int count, int total)
        {
            if (validTestimonyIndicator != null)
            {
                validTestimonyIndicator.text = $"証言成立: {count} / {total}";
            }
        }
    }
}
