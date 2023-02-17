namespace ModelsDTO.Cars;

public class CarResponse
{
    public Guid CarUid { get; set; }
    public string Brand { get; set; }
    public string Model { get; set; }
    public string RegistrationNumber { get; set; }
    public int Power { get; set; }
    public string? Type { get; set; }
    public int Price { get; set; }
    public bool Available { get; set; }
}