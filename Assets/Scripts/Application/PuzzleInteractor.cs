using Domain;

namespace Application
{
    // Interactor: ビジネスロジック
    public class PuzzleInteractor : IPuzzleInputPort
    {
        private readonly PuzzleBoard _board;
        private readonly IPuzzleOutputPort _output;
        public PuzzleInteractor(PuzzleBoard board, IPuzzleOutputPort output)
        {
            _board = board;
            _output = output;
        }

        public void OnTileSwipe(TileAddress address, SwipeDirection direction)
        {
            if (!_board.IsAdjacentToEmpty(address)) return;
            var empty = _board.EmptyCell;
            _board.SwapWithEmpty(address);
            _output.MoveTile(address, empty);
        }
    }
}
