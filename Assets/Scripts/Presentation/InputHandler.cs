using Application;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Presentation
{
    // UIイベントを受けてコントローラを呼ぶだけ
    public class InputHandler : MonoBehaviour
    {
        public PuzzleController controller;
        private Vector2 startTouchPos;
        private Vector2 endTouchPos;
        private int selectedX, selectedY;
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private PuzzlePresenter presenter;
        [SerializeField] private int gridSize = 3;
        [SerializeField, Range(0, 2)] private int puzzleDifficulty = 0; // 0: Easy, 1: Normal, 2: Hard

        private void Awake()
        {
            // PlayerPrefsから難易度を取得（なければデフォルト値0）
            puzzleDifficulty = PlayerPrefs.GetInt("puzzleDifficulty", puzzleDifficulty);
            // DTO生成
            var request = new PuzzleInitializeRequestDto(gridSize, puzzleDifficulty);
            // Interactor生成
            var interactor = new PuzzleInteractor(request, presenter);
            // コントローラ初期化
            controller.Initialize(interactor);
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetMouseButtonDown(0))
            {
                startTouchPos = Input.mousePosition;
                (selectedX, selectedY) = GetTileAddressUnderPointer(startTouchPos);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                endTouchPos = Input.mousePosition;
                HandleSwipe();
            }
#else
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPos = touch.position;
                    (selectedX, selectedY) = GetTileAddressUnderPointer(startTouchPos);
                }
                else if (touch.phase == TouchPhase.Ended)
                {
                    endTouchPos = touch.position;
                    HandleSwipe();
                }
            }
#endif
        }

        private (int, int) GetTileAddressUnderPointer(Vector2 screenPos)
        {
            // presenterからboardParentを参照
            PointerEventData ped = new(EventSystem.current) { position = screenPos };
            var results = new System.Collections.Generic.List<RaycastResult>();
            var raycaster = presenter.boardParent.GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();

            raycaster.Raycast(ped, results);
            foreach (var result in results)
            {
                foreach (var kv in presenter.TileObjects)
                {
                    if (result.gameObject == kv.Value)
                        return (kv.Key.X, kv.Key.Y);
                }
            }
            return (-1, -1); // 該当なし
        }

        private void HandleSwipe()
        {
            Vector2 delta = endTouchPos - startTouchPos;
            if (delta.magnitude < minSwipeDistance) return;
            var dir = delta.normalized;
            SwipeDirection swipeDir;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                swipeDir = dir.x > 0 ? SwipeDirection.Right : SwipeDirection.Left;
            else
                swipeDir = dir.y > 0 ? SwipeDirection.Up : SwipeDirection.Down;
            controller.OnTileSwipe(selectedX, selectedY, swipeDir);
        }
    }
}
