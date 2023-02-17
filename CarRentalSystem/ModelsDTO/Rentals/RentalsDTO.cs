namespace ModelsDTO.Rentals;

public class RentalsDTO
{
    public Guid RentalUid { get; set; }
    public string Username { get; set; }
    public Guid PaymentUid { get; set; }
    public Guid CarUid { get; set; }
    public DateTimeOffset DateFrom { get; set; }
    public DateTimeOffset DateTo { get; set; }
    public string Status { get; set; }
}