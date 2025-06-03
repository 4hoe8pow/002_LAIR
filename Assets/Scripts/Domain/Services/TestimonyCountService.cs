namespace Domain.Services
{
    public static class TestimonyCountService
    {
        public static int CountValidTestimonies(PuzzleBoard board)
        {
            int valid = 0;
            int size = board.Size;
            foreach (var tile in board.GetAllTiles())
            {
                bool isReliable = tile.IsTestimonyReliable();
                var testimony = tile.Testimony;
                int tx = tile.Address.X;
                int ty = tile.Address.Y;
                // 証言内容から方向と期待種別を取得
                int dx = 0, dy = 0;
                Species expectedSpecies = Species.Fox;
                switch (testimony)
                {
                    case TestimonyStatement.UpIsFox:
                        dx = 0; dy = -1; expectedSpecies = Species.Fox; break;
                    case TestimonyStatement.UpIsOwl:
                        dx = 0; dy = -1; expectedSpecies = Species.Owl; break;
                    case TestimonyStatement.DownIsFox:
                        dx = 0; dy = 1; expectedSpecies = Species.Fox; break;
                    case TestimonyStatement.DownIsOwl:
                        dx = 0; dy = 1; expectedSpecies = Species.Owl; break;
                    case TestimonyStatement.LeftIsFox:
                        dx = -1; dy = 0; expectedSpecies = Species.Fox; break;
                    case TestimonyStatement.LeftIsOwl:
                        dx = -1; dy = 0; expectedSpecies = Species.Owl; break;
                    case TestimonyStatement.RightIsFox:
                        dx = 1; dy = 0; expectedSpecies = Species.Fox; break;
                    case TestimonyStatement.RightIsOwl:
                        dx = 1; dy = 0; expectedSpecies = Species.Owl; break;
                }
                int nx = tx + dx, ny = ty + dy;
                if (nx < 0 || ny < 0 || nx >= size || ny >= size) continue;
                var neighbor = board.GetTile(new TileAddress(nx, ny));
                if (neighbor == null) continue;
                bool testimonyFact = neighbor.Species == expectedSpecies;
                if (testimonyFact == isReliable)
                {
                    valid++;
                }
            }
            return valid;
        }
    }
}
