using Domain;

namespace Application
{
    // InputPort: コントローラが呼ぶインターフェース
    public interface IPuzzleInputPort
    {
        void OnTileSwipe(TileAddress address, SwipeDirection direction);
    }
    public enum SwipeDirection { Up, Down, Left, Right }
}
