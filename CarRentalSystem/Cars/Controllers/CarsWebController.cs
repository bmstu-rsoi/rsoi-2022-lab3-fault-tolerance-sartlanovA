using Cars.ModelsDB;
using Cars.Repositories;

namespace Cars.Controllers
{
    public class CarsWebController
    {
        private readonly ICarsRepository _carsRepository;

        public CarsWebController(ICarsRepository carsRepository)
        {
            _carsRepository = carsRepository;
        }

        public async Task<List<Car>> GetAllCars(int page, int size)
        {
            return await _carsRepository.FindAll(page, size);
        }
        
        public async Task<List<Car>> GetAvailableCars(int page, int size)
        {
            return await _carsRepository.FindAvailable(page, size);
        }
        
        public async Task<Car> GetCarByUid(Guid carUid)
        {
            return await _carsRepository.FindByUid(carUid);
        }

        public async Task ReserveCarByUid(Car car)
        {
            await _carsRepository.Patch(car);
        }
    }
}