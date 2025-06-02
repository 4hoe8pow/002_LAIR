namespace Application
{
    // パズル初期化用DTO
    public class PuzzleInitializeRequestDto
    {
        public int GridSize { get; set; }
        public int Difficulty { get; set; }
        public PuzzleInitializeRequestDto(int gridSize, int difficulty)
        {
            GridSize = gridSize;
            Difficulty = difficulty;
        }
        // ドメイン値オブジェクトへの変換
        public Domain.PuzzleDifficulty ToPuzzleDifficulty()
        {
            return (Domain.PuzzleDifficulty)Difficulty;
        }
    }
}
