using Cars.ModelsDB;

namespace Cars.Repositories
{
    public interface ICarsRepository
    {
        Task<List<Car>> FindAll(int page, int size);
        Task<List<Car>> FindAvailable(int page, int size);
        Task<Car> FindByUid(Guid carUid);
        Task Patch(Car obj);
    }
}