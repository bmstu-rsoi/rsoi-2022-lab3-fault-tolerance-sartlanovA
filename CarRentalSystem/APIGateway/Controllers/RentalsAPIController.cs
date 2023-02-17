using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using APIGateway.Domain;
using APIGateway.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using ModelsDTO.Rentals;
using Swashbuckle.AspNetCore.Annotations;

namespace APIGateway.Controllers
{
    [ApiController]
    [Produces(MediaTypeNames.Application.Json)]
    [Route("/api/v1/rental")]
    public class RentalsAPIController : ControllerBase
    {
        private readonly IRentalsService _rentalsService;
        private readonly ILogger<RentalsRepository> _logger;

        public RentalsAPIController(IRentalsService rentalsService, ILogger<RentalsRepository> logger)
        {
            _rentalsService = rentalsService;
            _logger = logger;
        }

        /// <summary>Получить информацию о всех арендах пользователя</summary>
        /// <param name="X-User-Name">Имя пользователя</param>
        /// <response code="200">Информация обо всех арендах</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RentalResponse>))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllRentalsByUsername(
            [Required, FromHeader(Name = "X-User-Name")] string username)
        {
            try
            {
                var rentals = await _rentalsService.GetAllAsync(username);
                return Ok(rentals);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+ Error occurred trying GetAllRentalsByUsername!");
                throw;
            }
        }

        /// <summary>Информация по конкретной аренде пользователя</summary>
        /// <param name="rentalUid">UUID аренды</param>
        /// <param name="X-User-Name">Имя пользователя</param>
        /// <response code="200">Информация по конкретному бронированию</response>
        /// <response code="404">Билет не найден</response>
        [HttpGet("{rentalUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RentalResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRentalByUid([Required, FromHeader(Name = "X-User-Name")] string username,
            Guid rentalUid)
        {
            try
            {
                var response = await _rentalsService.GetAsyncByUid(username, rentalUid);
                return Ok(response);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(username);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+ Error occurred trying GetRentalByRentalUid!");
                throw;
            }
        }

        /// <summary>Забронировать автомобиль</summary>
        /// <param name="X-User-Name">Имя пользователя</param>
        /// <response code="200">Информация о бронировании авто</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, type: typeof(CreateRentalResponse),
            description: "Информация о бронировании авто")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Ошибка валидации данных")]
        public async Task<IActionResult> BookCar([Required, FromHeader(Name = "X-User-Name")] string username,
            [FromBody] CreateRentalRequest request)
        {
            if (!(await _rentalsService.HealthCheckAsync()))
            {
                var response = new ExceptionResponse("Payment Service unavailable");
                Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                return new ObjectResult(response);
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
            
                var response = await _rentalsService.RentCar(username, request);
                return Ok(response);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPIContoller: Error occurred trying BookCar!");
                throw;
            }
        }

        /// <summary> Завершение аренды автомобиля </summary>
        /// <param name="rentalUid"> UUID аренды </param>
        /// <param name="X-User-Name"> Имя пользователя </param>
        /// <response code="204"> Аренда успешно завершена </response>
        /// <response code="404"> Аренда не найдена </response>
        [HttpPost("{rentalUid}/finish")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FinishBookCar([Required, FromHeader(Name = "X-User-Name")] string username,
            [FromRoute] Guid rentalUid)
        {
            try
            {
                await _rentalsService.FinishRent(username, rentalUid);
                return NoContent();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPIContoller: Error occurred trying FinishBookCar!");
                throw;
            }
        }

        /// <summary> Отмена аренды автомобиля </summary>
        /// <param name="rentalUid"> UUID аренды </param>
        /// <param name="X-User-Name"> Имя пользователя </param>
        /// <response code="204"> Аренда успешно отменена </response>
        /// <response code="404"> Аренда не найдена </response>
        [HttpDelete("{rentalUid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CancelBookCar([Required, FromHeader(Name = "X-User-Name")] string username,
            [FromRoute] Guid rentalUid)
        {
            try
            {
                await _rentalsService.CancelRent(username, rentalUid);
                return NoContent();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(e.Message);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPIContoller: Error occurred trying FinishBookCar!");
                throw;
            }
        }
    }
}