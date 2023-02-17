using ModelsDTO.Payments;

namespace APIGateway;

public interface IPaymentsRepository
{
    Task<PaymentInfo> GetAsyncByUid(Guid paymentUid);
    Task<PaymentInfo> CreateAsync(PaymentInfo paymentInfo);
    Task<PaymentInfo> CancelAsync(Guid paymentUid);
}