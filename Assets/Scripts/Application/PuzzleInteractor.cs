using Domain;

namespace Application
{
    public class PuzzleInteractor : IPuzzleInputPort
    {
        private readonly PuzzleBoard _board;
        private readonly IPuzzleOutputPort _output;
        public PuzzleInteractor(PuzzleInitializeRequestDto request, IPuzzleOutputPort output)
        {
            _board = PuzzleBoard.Create(request.GridSize, request.ToPuzzleDifficulty());
            _output = output;
            _output.OnPuzzleInitialized(_board);
            _output.UpdateAllTileTestimonyTexts(_board);
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
