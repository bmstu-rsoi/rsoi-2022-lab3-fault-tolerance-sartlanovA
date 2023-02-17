using System.ComponentModel.DataAnnotations;
using Cars.ModelsDB;
using Microsoft.AspNetCore.Mvc;
using ModelsDTO.Cars;

namespace Cars.Controllers
{
    [ApiController]
    [Route("/api/v1/cars")]
    public class CarsAPIController : ControllerBase
    {
        private readonly CarsWebController _carsController;

        public CarsAPIController(CarsWebController carsController)
        {
            _carsController = carsController;
        }

        private CarResponse InitCarResponse(Car car)
        {
            CarResponse carResponse = new CarResponse()
            {
                CarUid = car.CarUid,
                Brand = car.Brand,
                Model = car.Model, 
                RegistrationNumber = car.RegistrationNumber,
                Power = car.Power,
                Price = car.Price,
                Type = car.Type,
                Available = car.Availability
            };
            return carResponse;
        }

        private List<CarResponse> ListCarResponse(List<Car> lCars)
        {
            List<CarResponse> lCarsResponse = new List<CarResponse>();
            foreach (var car in lCars)
            {
                var carResponse = InitCarResponse(car);
                lCarsResponse.Add(carResponse);
            }

            return lCarsResponse;
        }

        /// <summary>Получить список всех доступных для бронирования автомобилей</summary>
        /// <param name="page"> Page number </param>
        /// <param name="size"> Number of elements per page </param>
        /// <returns>Список доступных для бронирования автомобилей</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaginationCarResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCars([Range(0, int.MaxValue)] int page, [Range(1, 100)] int size, bool showAll)
        {
            var cars = (showAll) ? await _carsController.GetAllCars(page, size) : 
                    await _carsController.GetAvailableCars(page, size);

            var response = new PaginationCarResponse()
            {
                PageSize = size,
                Page = page,
                TotalElements = cars.Count,
                Items = ListCarResponse(cars).ToArray()
            };
            
            return Ok(response);
        }
        
        /// <summary>Получить автомобиль по Uuid</summary>
        /// <returns>Список доступных для бронирования автомобилей</returns>
        [HttpGet("{carUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CarResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetCarByUid(Guid carUid)
        {
            var car = await _carsController.GetCarByUid(carUid);
            var response = InitCarResponse(car);
            
            return Ok(response);
        }

        /// <summary>Забронировать автомобиль по Uuid</summary>
        /// <returns>Забронированный автомобиль</returns>
        [HttpPatch("{carUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CarResponse))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReserveCarByUid(Guid carUid, bool availability)
        {
            var car = await _carsController.GetCarByUid(carUid);
            car.Availability = availability;
            await _carsController.ReserveCarByUid(car);
            
            var response = InitCarResponse(car);
            return Ok(response);
        }
    }
}