namespace Domain
{
    public class Tile
    {
        public TileAddress Address { get; private set; }
        public Tile(TileAddress address)
        {
            Address = address;
        }
        public void MoveTo(TileAddress newAddress)
        {
            Address = newAddress;
        }
    }
}
