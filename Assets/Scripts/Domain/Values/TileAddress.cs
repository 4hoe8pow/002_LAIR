namespace Domain
{
    // 値オブジェクト: タイルの座標
    public readonly struct TileAddress
    {
        public int X { get; }
        public int Y { get; }
        public TileAddress(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override readonly bool Equals(object obj)
        {
            if (obj is TileAddress other)
                return X == other.X && Y == other.Y;
            return false;
        }
        public override readonly int GetHashCode() => (X, Y).GetHashCode();
    }
}
