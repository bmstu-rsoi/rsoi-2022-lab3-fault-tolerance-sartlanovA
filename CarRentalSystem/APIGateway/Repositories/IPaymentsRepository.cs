using ModelsDTO.Payments;

namespace APIGateway;

public interface IPaymentsRepository
{
    Task<PaymentInfo> GetAsyncByUid(Guid paymentUid);
    Task<PaymentInfo> CreateAsync(PaymentInfo paymentInfo);
    Task CancelAsync(Guid paymentUid);
    Task<bool> HealthCheckAsync();
}