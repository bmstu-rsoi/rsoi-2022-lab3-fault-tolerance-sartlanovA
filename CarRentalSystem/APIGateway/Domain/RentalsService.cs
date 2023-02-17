using System.Net;
using System.Reflection;
using System.Web;
using APIGateway.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using ModelsDTO.Cars;
using ModelsDTO.Payments;
using ModelsDTO.Rentals;

namespace APIGateway.Domain;

public class RentalsService : IRentalsService
{
    private readonly ICarsRepository _carsRepository;
    private readonly IRentalsRepository _rentalsRepository;
    private readonly IPaymentsRepository _paymentsRepository;

    public RentalsService(ICarsRepository carsRepository, IRentalsRepository rentalsRepository, 
        IPaymentsRepository paymentsRepository)
    {
        _carsRepository = carsRepository;
        _rentalsRepository = rentalsRepository;
        _paymentsRepository = paymentsRepository;
    }

    private RentalsDTO InitRentalsDTO(string username, Guid paymentUid, CreateRentalRequest request, string status)
    {
        var rentalDTO = new RentalsDTO()
        {
            RentalUid = Guid.NewGuid(),
            Username = username,
            PaymentUid = paymentUid,
            CarUid = request.CarUid,
            DateFrom = request.DateFrom.UtcDateTime,
            DateTo = request.DateTo.UtcDateTime,
            Status = status
        };
        return rentalDTO;
    }

    private CreateRentalResponse InitCreateRentalResponse(Guid rentalUid, Guid carUid, 
        DateTimeOffset dateFrom, DateTimeOffset dateTo)
    {
        var response = new CreateRentalResponse()
        {
            RentalUid = rentalUid,
            Status = "IN_PROGRESS",
            CarUid = carUid,
            DateFrom = dateFrom.ToString("yyyy-MM-dd"),
            DateTo = dateTo.ToString("yyyy-MM-dd")
        };
        return response;
    }

    private RentalResponse GetRentalResponse(RentalsDTO rental)
    {
        var response = new RentalResponse()
        {
            RentalUid = rental.RentalUid,
            Status = rental.Status,
            DateFrom = rental.DateFrom.ToString("yyyy-MM-dd"),
            DateTo = rental.DateTo.ToString("yyyy-MM-dd")
        };
        return response;
    }

    private PaymentInfo InitPaymentInfo(string status, int price)
    {
        var paymentInfo = new PaymentInfo()
        {
            PaymentUid = Guid.NewGuid(),
            Status = status,
            Price = price
        };
        return paymentInfo;
    }

    private async Task<RentalResponse> AddCarInfoAsync(Guid carUid, RentalResponse rental)
    {
        var car = await _carsRepository.GetAsyncByUid(carUid);
        rental.Car = new CarInfo()
        {
            CarUid = carUid,
            Brand = car.Brand,
            Model = car.Model,
            RegistrationNumber = car.RegistrationNumber
        };

        return rental;
    }

    private async Task<RentalResponse> AddPaymentInfoAsync(Guid paymentUid, RentalResponse rental)
    {
        var payment = await _paymentsRepository.GetAsyncByUid(paymentUid);
        rental.Payment = new PaymentInfo()
        {
            PaymentUid = payment.PaymentUid,
            Status = payment.Status,
            Price = payment.Price
        };

        return rental;
    }

    private async Task<CreateRentalResponse> AddPaymentInfoAsync(Guid paymentUid, CreateRentalResponse rental)
    {
        var payment = await _paymentsRepository.GetAsyncByUid(paymentUid);
        rental.Payment = new PaymentInfo()
        {
            PaymentUid = payment.PaymentUid,
            Status = payment.Status,
            Price = payment.Price
        };

        return rental;
    }

    public async Task<List<RentalResponse>?> GetAllAsync(string username)
    {
        var rentals = await _rentalsRepository.GetAllAsyncByUsername(username);
        var response = new List<RentalResponse>(rentals.Count);
        foreach (var rental in rentals)
        {
            var res = GetRentalResponse(rental);
            await AddCarInfoAsync(rental.CarUid, res);
            await AddPaymentInfoAsync(rental.PaymentUid, res);
            response.Add(res);
        }

        return response;
    }

    public async Task<RentalResponse> GetAsyncByUid(string username, Guid rentalUid)
    {
        var response = new RentalResponse();
        
        if (await _rentalsRepository.HealthCheckAsync())
        {
            var rental = await _rentalsRepository.GetAsyncByUsernameAndRentalUid(username, rentalUid);
            response = GetRentalResponse(rental);

            if (await _carsRepository.HealthCheckAsync())
            {
                await AddCarInfoAsync(rental.CarUid, response);

                if (await _paymentsRepository.HealthCheckAsync())
                {
                    await AddPaymentInfoAsync(rental.PaymentUid, response);
                }
                else
                {
                    response.Payment = null;
                }
            }
        }

        return response;
    }

    public async Task<CreateRentalResponse> RentCar(string username, CreateRentalRequest request)
    {
        var carUid = request.CarUid;
        var car = await _carsRepository.ReserveCar(carUid, false);

        var duration = (int) (request.DateTo - request.DateFrom).TotalDays;
        var newPaymentInfo = InitPaymentInfo("PAID", duration * car.Price);
        var payment = await _paymentsRepository.CreateAsync(newPaymentInfo);

        var newRentalDTO = InitRentalsDTO(username, payment.PaymentUid, request, "IN_PROGRESS");
        var rental = await _rentalsRepository.CreateAsync(newRentalDTO);

        var response = InitCreateRentalResponse(rental.RentalUid, car.CarUid, request.DateFrom, request.DateTo);
        await AddPaymentInfoAsync(rental.PaymentUid, response);
        
        return response;
    }

    public async Task FinishRent(string username, Guid rentalUid)
    {
        var rental = await _rentalsRepository.GetAsyncByUsernameAndRentalUid(username, rentalUid);
        var carUid = rental.CarUid;

        await _carsRepository.ReserveCar(carUid, true);
        await _rentalsRepository.ProcessRent(username, rentalUid, "FINISHED");
    }

    public async Task CancelRent(string username, Guid rentalUid)
    {
        var rental = await _rentalsRepository.GetAsyncByUsernameAndRentalUid(username, rentalUid);
        
        var carUid = rental.CarUid;
        var paymentUid = rental.PaymentUid;
            
        await _carsRepository.ReserveCar(carUid, true);
        await _rentalsRepository.ProcessRent(username, rentalUid, "CANCELED");
        await _paymentsRepository.CancelAsync(paymentUid);
    }

    public async Task<bool> HealthCheckAsync()
    {
        var responseCars = await _carsRepository.HealthCheckAsync();
        var responseRentals = await _rentalsRepository.HealthCheckAsync();
        var responsePayments = await _paymentsRepository.HealthCheckAsync();
        return responseCars && responseRentals && responsePayments;
    }
}
