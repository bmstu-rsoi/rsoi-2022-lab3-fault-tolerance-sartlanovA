using APIGateway.ModelsDTO;
using ModelsDTO.Rentals;
using ModelsDTO.Cars;

namespace APIGateway;

public interface IRentalsRepository
{
    Task<List<RentalsDTO>?> GetAllAsyncByUsername(string username);
    Task<RentalsDTO> GetAsyncByUsernameAndRentalUid(string username, Guid rentalUid);
    Task<RentalsDTO> CreateAsync(RentalsDTO rentalDTO);
    Task<RentalsDTO> ProcessRent(string username, Guid rentalUid, string status);
    Task<bool> HealthCheckAsync();
}
