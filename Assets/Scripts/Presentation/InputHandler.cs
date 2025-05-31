using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Presentation
{
    // UIイベントを受けてコントローラを呼ぶだけ
    public class InputHandler : MonoBehaviour
    {
        public PuzzleController controller;
        // ここでUIイベントを受けてcontroller.OnTileSwipeを呼ぶ
        // 例: タイルがスワイプされたとき
        private Vector2 startTouchPos;
        private Vector2 endTouchPos;
        private int selectedX, selectedY;
        [SerializeField] private float minSwipeDistance = 50f;
        [SerializeField] private PuzzlePresenter presenter;
        [SerializeField] private int gridSize = 3;

        private void Awake()
        {
            // Domain層のボード生成
            var board = new Domain.PuzzleBoard(gridSize);
            // Interactor生成
            var interactor = new Application.PuzzleInteractor(board, presenter);
            // コントローラ初期化
            controller.Initialize(interactor);
            // 盤面初期化表示
            presenter.OnPuzzleInitialized(board);
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
            Application.SwipeDirection swipeDir;
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                swipeDir = dir.x > 0 ? Application.SwipeDirection.Right : Application.SwipeDirection.Left;
            else
                swipeDir = dir.y > 0 ? Application.SwipeDirection.Up : Application.SwipeDirection.Down;
            controller.OnTileSwipe(new Domain.TileAddress(selectedX, selectedY), swipeDir);
        }
    }
}
