using Domain;
using Domain.Services;

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
            ShowTestimonyIndicator();
        }

        public void OnTileSwipe(int x, int y, SwipeDirection direction)
        {
            var address = new TileAddress(x, y);
            if (!_board.IsAdjacentToEmpty(address)) return;
            var empty = _board.EmptyCell;
            _board.SwapWithEmpty(address);
            _output.MoveTile(address, empty);
            ShowTestimonyIndicator();
        }

        private void ShowTestimonyIndicator()
        {
            int valid = TestimonyCountService.CountValidTestimonies(_board);
            int total = _board.Size * _board.Size - 1;
            _output.ShowValidTestimonyCount(valid, total);

            UnityEngine.Debug.Log($"Valid testimonies: {valid}/{total}");

            if (valid == total)
            {
                UnityEngine.Debug.Log($"Victory! All testimonies are valid.");
                _output.PlayVictoryEffect();
            }
        }
    }
}
