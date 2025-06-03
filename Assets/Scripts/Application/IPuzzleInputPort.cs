using Domain;

namespace Application
{
    // InputPort: コントローラが呼ぶインターフェース
    public interface IPuzzleInputPort
    {
        void OnTileSwipe(int x, int y, SwipeDirection direction);
    }
    public enum SwipeDirection { Up, Down, Left, Right }
}
