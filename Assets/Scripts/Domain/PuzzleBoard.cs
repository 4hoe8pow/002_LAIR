using System.Collections.Generic;

namespace Domain
{
    // パズル盤面のエンティティ
    public class PuzzleBoard
    {
        private readonly int _size;
        private readonly Tile[,] _tiles;
        private TileAddress _emptyCell;
        public int Size => _size;
        public TileAddress EmptyCell => _emptyCell;

        public PuzzleBoard(int size)
        {
            _size = size;
            _tiles = new Tile[size, size];
            InitializeTiles();
        }

        private void InitializeTiles()
        {
            for (int y = 0; y < _size; y++)
            {
                for (int x = 0; x < _size; x++)
                {
                    if (x == _size - 1 && y == _size - 1)
                    {
                        _tiles[x, y] = null;
                        _emptyCell = new TileAddress(x, y);
                    }
                    else
                    {
                        _tiles[x, y] = new Tile(new TileAddress(x, y));
                    }
                }
            }
        }

        public Tile GetTile(TileAddress address)
        {
            return _tiles[address.X, address.Y];
        }

        public bool IsAdjacentToEmpty(TileAddress address)
        {
            int dx = System.Math.Abs(_emptyCell.X - address.X);
            int dy = System.Math.Abs(_emptyCell.Y - address.Y);
            return dx + dy == 1;
        }

        public void SwapWithEmpty(TileAddress address)
        {
            if (!IsAdjacentToEmpty(address)) return;
            var movingTile = _tiles[address.X, address.Y];
            _tiles[_emptyCell.X, _emptyCell.Y] = movingTile;
            _tiles[address.X, address.Y] = null;
            movingTile?.MoveTo(_emptyCell);
            _emptyCell = address;
        }

        public IEnumerable<Tile> GetAllTiles()
        {
            for (int y = 0; y < _size; y++)
                for (int x = 0; x < _size; x++)
                    if (_tiles[x, y] != null)
                        yield return _tiles[x, y];
        }
    }
}
