namespace Cars.ModelsDB
{
    public partial class Car
    {
        public int Id { get; set; }
        public Guid CarUid { get; set; }
        public string Brand { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string RegistrationNumber { get; set; } = null!;
        public int Power { get; set; }
        public int Price { get; set; }
        public string? Type { get; set; }
        public bool Availability { get; set; }
    }
}