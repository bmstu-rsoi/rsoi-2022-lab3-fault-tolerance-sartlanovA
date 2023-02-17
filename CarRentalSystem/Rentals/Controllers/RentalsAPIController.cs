using System.ComponentModel.DataAnnotations;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rentals.ModelsDB;
using ModelsDTO.Rentals;

namespace Rentals.Controllers
{
    [ApiController]
    [Route("/api/v1/rental")]
    public class RentalsAPIController : ControllerBase
    {
        private readonly RentalsWebController _rentalsController;
        private readonly ILogger<RentalsWebController> _logger;

        public RentalsAPIController(RentalsWebController rentalsController, ILogger<RentalsWebController> logger)
        {
            _rentalsController = rentalsController;
            _logger = logger;
        }

        private Rental GetRentalFromDTO(RentalsDTO rentalDTO)
        {
            var rental = new Rental()
            {
                Id = 0,
                RentalUid = rentalDTO.RentalUid,
                Username = rentalDTO.Username,
                PaymentUid = rentalDTO.PaymentUid,
                CarUid = rentalDTO.CarUid,
                DateFrom = rentalDTO.DateFrom.UtcDateTime,
                DateTo = rentalDTO.DateTo.UtcDateTime,
                Status = rentalDTO.Status
            };
            return rental;
        }

        private RentalsDTO InitRentalsDTO(Rental rental)
        {
            var rentalDTO = new RentalsDTO()
            {
                RentalUid = rental.RentalUid,
                Username = rental.Username,
                PaymentUid = rental.PaymentUid,
                CarUid = rental.CarUid,
                DateFrom = rental.DateFrom,
                DateTo = rental.DateTo,
                Status = rental.Status
            };

            return rentalDTO;
        }

        private List<RentalsDTO> ListRentalsDTO(List<Rental> lRentals)
        {
            var lRentalsDTO = new List<RentalsDTO>();
            foreach (var rental in lRentals)
            {
                var rentalDTO = InitRentalsDTO(rental);
                lRentalsDTO.Add(rentalDTO);
            }

            return lRentalsDTO;
        }

        /// <summary>Получить информацию о всех арендах пользователя</summary>
        /// <param name="X-User-Name"> Имя пользователя </param>
        /// <response code="200">Информация обо всех арендах</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<RentalsDTO>))]
        public async Task<IActionResult> GetRentalsByUsername([Required, FromQuery(Name = "X-User-Name")] string username)
        {
            try
            {
                var rentals = await _rentalsController.GetAllRentalsByUsername(username);
                var response = ListRentalsDTO(rentals);
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPI: Error while trying to GetRentalsByUsername!");
                throw;
            }
        }
        
        /// <summary>Информация по конкретной аренде пользователя</summary>
        /// <param name="rentalUid">UUID аренды</param>
        /// <param name="X-User-Name"> Имя пользователя </param>
        /// <response code="200">Информация по конкретному бронированию</response>
        /// <response code="404">Билет не найден</response>
        [HttpGet("{rentalUid}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RentalsDTO))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRentalByUid([Required, FromQuery(Name = "X-User-Name")] string username,
            Guid rentalUid)
        {
            try
            {
                var rental = await _rentalsController.GetRentalByUid(username, rentalUid);
                var response = InitRentalsDTO(rental);
                return Ok(response);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(username);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPI: Error while trying to GetRentalByUid!");
                throw;
            }
        }

        /// <summary>Забронировать автомобиль</summary>
        /// <param name="X-User-Name"> Имя пользователя </param>
        /// <response code="201">Информация о бронировании авто</response>
        /// <response code="400">Ошибка валидации данных</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RentalsDTO))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateRental([FromBody] RentalsDTO rentalDTO)
        {
            try
            {
                var rentalToAdd = GetRentalFromDTO(rentalDTO);
                var addedRental = await _rentalsController.AddRental(rentalToAdd);

                var response = InitRentalsDTO(addedRental);
                return Created($"/api/v1/{addedRental.Id}", response);
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.BadRequest)
            {
                return BadRequest();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPI: Error while trying to CreateRental!");
                throw;
            }
        }
        
        /// <summary>Завершение аренды автомобиля</summary>
        /// <param name="rentalUid">UUID аренды</param>
        /// <param name="X-User-Name">Имя пользователя </param>
        /// <response code="204">Аренда успешно завершена</response>
        /// <response code="404">Аренда не найдена</response>
        [HttpPatch("{username}/{rentalUid}/{status}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeRentalStatus(string username, Guid rentalUid, string status)
        {
            try
            {
                var rental = await _rentalsController.GetRentalByUid(username, rentalUid);
                rental.Status = status;
                await _rentalsController.PatchRental(rental);

                return NoContent();
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return NotFound(username);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsAPI: Error while trying to ChangeRentalStatus!");
                throw;
            }
        }
    }
}