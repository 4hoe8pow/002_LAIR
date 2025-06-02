namespace Domain.Services
{
    public static class TestimonyCountService
    {
        public static int CountValidTestimonies(PuzzleBoard board)
        {
            // boardのprivateメソッドを移植
            int valid = 0;
            int size = board.Size;
            var directions = new[]
            {
                (dx: 0, dy: -1, fox: TestimonyStatement.UpIsFox, owl: TestimonyStatement.UpIsOwl),
                (dx: 0, dy: 1, fox: TestimonyStatement.DownIsFox, owl: TestimonyStatement.DownIsOwl),
                (dx: -1, dy: 0, fox: TestimonyStatement.LeftIsFox, owl: TestimonyStatement.LeftIsOwl),
                (dx: 1, dy: 0, fox: TestimonyStatement.RightIsFox, owl: TestimonyStatement.RightIsOwl)
            };
            foreach (var tile in board.GetAllTiles())
            {
                var (x, y) = (tile.Address.X, tile.Address.Y);
                foreach (var (dx, dy, fox, owl) in directions)
                {
                    int nx = x + dx, ny = y + dy;
                    if (nx < 0 || ny < 0 || nx >= size || ny >= size) continue;
                    var neighbor = board.GetTile(new TileAddress(nx, ny));
                    if (neighbor == null) continue;
                    bool testimonyTrue = false;
                    if (tile.IsTestimonyReliable())
                        testimonyTrue = (neighbor.IsTestimonyReliable() && owl.ToString().Contains("Owl")) || (!neighbor.IsTestimonyReliable() && fox.ToString().Contains("Fox"));
                    else
                        testimonyTrue = !((neighbor.IsTestimonyReliable() && owl.ToString().Contains("Owl")) || (!neighbor.IsTestimonyReliable() && fox.ToString().Contains("Fox")));
                    if (testimonyTrue)
                    {
                        valid++;
                        break;
                    }
                }
            }
            return valid;
        }
    }
}
