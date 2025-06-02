using Domain;

namespace Application
{
    public interface IPuzzleOutputPort
    {
        void MoveTile(TileAddress from, TileAddress to);
        void OnPuzzleInitialized(PuzzleBoard board);
        void UpdateTileTestimonyText(Tile tile);
        void UpdateAllTileTestimonyTexts(PuzzleBoard board);
        void ShowValidTestimonyCount(int count, int total);
    }
}
