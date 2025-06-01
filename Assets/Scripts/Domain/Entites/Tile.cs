namespace Domain
{
    public class Tile
    {
        public TileAddress Address { get; private set; }
        public Species Species { get; private set; }
        public Tile(TileAddress address, Species species)
        {
            Address = address;
            Species = species;
        }
        public void MoveTo(TileAddress newAddress)
        {
            Address = newAddress;
        }
    }
}
