using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Services;

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

        public PuzzleBoard(int size, PuzzleDifficulty difficulty)
        {
            _size = size;
            _tiles = new Tile[size, size];
            InitializeTiles(difficulty);
        }

        public static PuzzleBoard Create(int size, PuzzleDifficulty difficulty)
        {
            return new PuzzleBoard(size, difficulty);
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

        private void InitializeTiles(PuzzleDifficulty difficulty)
        {
            int foxCount = (int)difficulty;
            int totalTiles = _size * _size - 1;
            int owlCount = totalTiles - foxCount;
            var positions = Enumerable.Range(0, totalTiles)
                .Select(i => (x: i % _size, y: i / _size)).ToArray();
            var speciesPool = Enumerable.Repeat(Species.Fox, foxCount)
                .Concat(Enumerable.Repeat(Species.Owl, owlCount)).ToArray();
            var rnd = new Random();
            int maxScore = -1;
            Species[] bestPattern = null;
            int tryCount = Math.Min(100000, Factorial(totalTiles)); // 10万回 or 全パターン少ない場合は全探索
            for (int t = 0; t < tryCount; t++)
            {
                var arr = (Species[])speciesPool.Clone();
                // Fisher-Yatesシャッフル
                for (int i = arr.Length - 1; i > 0; i--)
                {
                    int j = rnd.Next(i + 1);
                    (arr[i], arr[j]) = (arr[j], arr[i]);
                }
                // 仮配置
                for (int i = 0; i < totalTiles; i++)
                {
                    var (x, y) = positions[i];
                    _tiles[x, y] = new Tile(new TileAddress(x, y), arr[i], TestimonyStatement.UpIsFox);
                }
                int score = TestimonyCountService.CountValidTestimonies(this);
                if (score > maxScore)
                {
                    maxScore = score;
                    bestPattern = (Species[])arr.Clone();
                    if (score == totalTiles) break; // 全て証言通りなら即break
                }
            }
            // 最良パターンを反映
            for (int i = 0; i < totalTiles; i++)
            {
                var (x, y) = positions[i];
                _tiles[x, y] = new Tile(new TileAddress(x, y), bestPattern[i], TestimonyStatement.UpIsFox);
            }
            _tiles[_size - 1, _size - 1] = null;
            _emptyCell = new TileAddress(_size - 1, _size - 1);
            AssignTestimonies();
            ShuffleAndMakePuzzle();
        }

        // ランダムシャッフルして作問する処理を分離
        private void ShuffleAndMakePuzzle()
        {
            var rndSwap = new Random();
            int validTestimonies = TestimonyCountService.CountValidTestimonies(this);
            int maxShuffle = 100000; // 無限ループ防止
            int shuffleCount = 0;
            while (validTestimonies > 0 && shuffleCount < maxShuffle)
            {
                // 空きマスに隣接するタイルを列挙
                var neighbors = new List<TileAddress>();
                int ex = _emptyCell.X, ey = _emptyCell.Y;
                var dirs = new[] { (dx: 0, dy: -1), (dx: 0, dy: 1), (dx: -1, dy: 0), (dx: 1, dy: 0) };
                foreach (var (dx, dy) in dirs)
                {
                    int nx = ex + dx, ny = ey + dy;
                    if (nx >= 0 && ny >= 0 && nx < _size && ny < _size && _tiles[nx, ny] != null)
                        neighbors.Add(new TileAddress(nx, ny));
                }
                if (neighbors.Count == 0) break;

                // ランダムに1つ選んでSwap
                var addr = neighbors[rndSwap.Next(neighbors.Count)];
                SwapWithEmpty(addr);
                validTestimonies = TestimonyCountService.CountValidTestimonies(this);
                shuffleCount++;
            }
        }

        private int Factorial(int n)
        {
            int res = 1;
            for (int i = 2; i <= n; i++) res *= i;
            return res;
        }

        private void Permute(Species[] arr, int k, List<Species[]> result)
        {
            if (k == arr.Length)
            {
                result.Add((Species[])arr.Clone());
            }
            else
            {
                for (int i = k; i < arr.Length; i++)
                {
                    (arr[k], arr[i]) = (arr[i], arr[k]);
                    Permute(arr, k + 1, result);
                    (arr[k], arr[i]) = (arr[i], arr[k]);
                }
            }
        }

        // 指定座標の隣接タイルを取得（範囲外や空マスはnull）
        private Tile GetNeighborTile(int x, int y, (int dx, int dy, TestimonyStatement fox, TestimonyStatement owl) dir)
        {
            int nx = x + dir.dx, ny = y + dir.dy;
            if (nx < 0 || ny < 0 || nx >= _size || ny >= _size) return null;
            return _tiles[nx, ny];
        }

        // 方向配列をFisher-Yatesでシャッフル
        private void ShuffleDirections((int dx, int dy, TestimonyStatement fox, TestimonyStatement owl)[] dirs, System.Random rnd)
        {
            for (int i = dirs.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (dirs[j], dirs[i]) = (dirs[i], dirs[j]);
            }
        }

        // 証言を決定するロジック
        private TestimonyStatement DecideTestimony(Species self, Species neighbor, (TestimonyStatement fox, TestimonyStatement owl) dir)
        {
            if (self == Species.Owl)
                return neighbor == Species.Fox ? dir.fox : dir.owl;
            else
                return neighbor == Species.Fox ? dir.owl : dir.fox;
        }

        // 証言割り当て
        private void AssignTestimonies()
        {
            int totalTiles = _size * _size - 1;
            var allTiles = GetAllTiles().ToList();
            if (allTiles.Count != totalTiles)
                throw new InvalidOperationException($"AssignTestimonies: タイル数不一致 (expected={totalTiles}, actual={allTiles.Count})");
            var directions = new[]
            {
                (dx: 0, dy: -1, fox: TestimonyStatement.UpIsFox, owl: TestimonyStatement.UpIsOwl),
                (dx: 0, dy: 1, fox: TestimonyStatement.DownIsFox, owl: TestimonyStatement.DownIsOwl),
                (dx: -1, dy: 0, fox: TestimonyStatement.LeftIsFox, owl: TestimonyStatement.LeftIsOwl),
                (dx: 1, dy: 0, fox: TestimonyStatement.RightIsFox, owl: TestimonyStatement.RightIsOwl)
            };
            var rnd = new Random();
            foreach (var tile in allTiles)
            {
                var (x, y) = (tile.Address.X, tile.Address.Y);
                var dirs = directions.ToArray();
                ShuffleDirections(dirs, rnd);
                foreach (var dir in dirs)
                {
                    var neighbor = GetNeighborTile(x, y, dir);
                    if (neighbor == null) continue;
                    tile.Testimony = DecideTestimony(tile.Species, neighbor.Species, (dir.fox, dir.owl));
                    break;
                }
            }
            // 証言正答数チェック
            int validCount = TestimonyCountService.CountValidTestimonies(this);
            UnityEngine.Debug.Log($"AssignTestimonies: 証言正答数={validCount} (expected={totalTiles})");
            if (validCount != totalTiles)
                throw new InvalidOperationException($"AssignTestimonies: 証言正答数不一致 (expected={totalTiles}, actual={validCount})");
        }

        public Tile GetTile(TileAddress address)
        {
            return _tiles[address.X, address.Y];
        }

        public bool IsAdjacentToEmpty(TileAddress address)
        {
            int dx = Math.Abs(_emptyCell.X - address.X);
            int dy = Math.Abs(_emptyCell.Y - address.Y);
            return dx + dy == 1;
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
