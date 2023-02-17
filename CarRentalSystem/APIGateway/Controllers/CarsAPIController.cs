using System.ComponentModel.DataAnnotations;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using ModelsDTO.Cars;
using Swashbuckle.AspNetCore.Annotations;

namespace APIGateway.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("/api/v1/cars")]
    public class CarsAPIController : ControllerBase
    {
        private readonly ICarsRepository _carsRepository;

        public CarsAPIController(ICarsRepository carsRepository)
        {
            _carsRepository = carsRepository;
        }

        /// <summary>Получить список всех доступных для бронирования автомобилей</summary>
        /// <response code="200">Список доступных для бронирования автомобилей</response>
        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(PaginationCarResponse), 
            description: "Список доступных для бронирования автомобилей")]
        public async Task<IActionResult> GetAllCars([Range(0, int.MaxValue)] int page, [Range(1, 100)] int size, bool showAll)
        {
            var response = await _carsRepository.GetAllAsync(page, size, showAll);
            return Ok(response);
        }
    }
}