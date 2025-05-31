using Domain;

namespace Application
{
    // OutputPort: Presenterが実装
    public interface IPuzzleOutputPort
    {
        void MoveTile(TileAddress from, TileAddress to);
        void OnPuzzleInitialized(Domain.PuzzleBoard board);
    }
}
