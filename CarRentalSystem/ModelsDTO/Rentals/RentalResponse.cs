using ModelsDTO.Cars;
using ModelsDTO.Payments;

namespace ModelsDTO.Rentals;

public class RentalResponse
{
    public Guid RentalUid { get; set; }
    public string Status { get; set; }
    public string DateFrom { get; set; }
    public string DateTo { get; set; }
    public CarInfo Car { get; set; }
    public PaymentInfo? Payment { get; set; }
}