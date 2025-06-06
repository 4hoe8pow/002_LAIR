using Application;
using Domain;
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

        // 証言成立数インジケーター用
        [Header("証言成立ゲージ")]
        public RectTransform validTestimonyGaugeParent;
        public GameObject validTestimonyGaugeCellPrefab;
        private readonly List<GameObject> _gaugeCells = new();
        public float gaugeCellSpacing = 3f; // セル間の隙間

        private Coroutine _gaugeAnimCoroutine;

        public ParticleSystem victoryEffectParticle;

        private void Awake()
        {
            // ゲージのセルを8個に固定で作成
            if (validTestimonyGaugeParent != null && validTestimonyGaugeCellPrefab != null)
            {
                foreach (var cell in _gaugeCells) Destroy(cell);
                _gaugeCells.Clear();
                int cellCount = 8;
                float spacing = gaugeCellSpacing;
                float parentWidth = validTestimonyGaugeParent.rect.width;
                float cellWidth = (parentWidth - spacing * (cellCount - 1)) / cellCount;
                float parentHeight = validTestimonyGaugeParent.rect.height;
                for (int i = 0; i < cellCount; i++)
                {
                    var cell = Instantiate(validTestimonyGaugeCellPrefab, validTestimonyGaugeParent);
                    cell.SetActive(false);
                    var cellRect = cell.GetComponent<RectTransform>();
                    cellRect.anchorMin = new Vector2(0, 0.5f);
                    cellRect.anchorMax = new Vector2(0, 0.5f);
                    cellRect.pivot = new Vector2(0, 0.5f);
                    cellRect.sizeDelta = new Vector2(cellWidth, parentHeight);
                    cellRect.anchoredPosition = new Vector2(i * (cellWidth + spacing), 0);
                    _gaugeCells.Add(cell);
                }
            }
        }

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
            if (_gaugeAnimCoroutine != null)
            {
                StopCoroutine(_gaugeAnimCoroutine);
            }
            _gaugeAnimCoroutine = StartCoroutine(AnimateGauge(count, total));
        }

        private System.Collections.IEnumerator AnimateGauge(int count, int total)
        {
            float animDelay = 0.07f; // 1セルごとの遷移速度
            for (int i = 0; i < _gaugeCells.Count; i++)
            {
                bool active = i < total && i < 8;
                _gaugeCells[i].SetActive(active);
            }
            for (int i = 0; i < _gaugeCells.Count; i++)
            {
                if (i < total && i < 8 && _gaugeCells[i].TryGetComponent<UnityEngine.UI.Image>(out var img))
                {
                    img.color = i < count ? Color.yellow : Color.gray;
                }
                yield return new WaitForSeconds(animDelay);
            }
        }

        public void PlayVictoryEffect()
        {
            if (victoryEffectParticle != null)
            {
                victoryEffectParticle.Play();
            }
        }
    }
}
