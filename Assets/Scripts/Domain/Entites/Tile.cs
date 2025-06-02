namespace Domain
{
    public class Tile
    {
        public TileAddress Address { get; private set; }
        public Species Species { get; private set; }
        public TestimonyStatement Testimony { get; internal set; }
        public Tile(TileAddress address, Species species, TestimonyStatement testimony)
        {
            Address = address;
            Species = species;
            Testimony = testimony;
        }
        public void MoveTo(TileAddress newAddress)
        {
            Address = newAddress;
        }
        // 証言が信頼できるか
        public bool IsTestimonyReliable()
        {
            return Species == Species.Owl;
        }
    }
}
