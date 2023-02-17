using Microsoft.EntityFrameworkCore;
using Payments.ModelsDB;

namespace Payments.Repositories
{
    public class PaymentsRepository : IPaymentsRepository, IDisposable
    {
        private readonly PaymentContext _db;
        private readonly ILogger<PaymentsRepository> _logger;

        public PaymentsRepository(PaymentContext createDb, ILogger<PaymentsRepository> logDb)
        {
            _db = createDb;
            _logger = logDb;
        }

        public async Task<Payment> FindByUid(Guid paymentUid)
        {
            try
            {
                var payment = await _db.Payments
                    .FirstOrDefaultAsync(x => x.PaymentUid == paymentUid);
                return payment;

            }
            catch (Exception e)
            {
                _logger.LogError(e, "+ Error while trying to FindByUid");
                throw;
            }
        }

        public async Task<Payment> Add(Payment obj)
        {
            try
            {
                var id = _db.Payments.Count() + 1;
                obj.Id = id;

                if (obj.PaymentUid == default)
                    obj.PaymentUid = Guid.NewGuid();

                _db.Payments.Add(obj);
                await _db.SaveChangesAsync();
                
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+ Error while trying to Add");
                throw;
            }
        }

        public async Task Patch(Payment payment)
        {
            try
            {
                _db.Payments.Update(payment);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+ Error while trying to Patch");
                throw;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}