using ModelsDTO.Cars;

namespace APIGateway;

public interface ICarsRepository
{
    Task<PaginationCarResponse> GetAllAsync(int page, int size, bool showAll);
    Task<CarResponse> GetAsyncByUid(Guid carUid);
    Task<CarResponse> ReserveCar(Guid carUid, bool availability);
    Task<bool> HealthCheckAsync();
}