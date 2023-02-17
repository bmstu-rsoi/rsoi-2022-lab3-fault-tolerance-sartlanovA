using ModelsDTO.Payments;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using ModelsDTO.Payments.Cancel;
using Payments.ModelsDB;

namespace Payments.Controllers;

public class PaymentConsumer : IConsumer<CancelPayment>
{
    private readonly PaymentContext _context;

    public PaymentConsumer(PaymentContext context)
    {
        _context = context;
    }
    
    public async Task Consume(ConsumeContext<CancelPayment> context)
    {
        var paymentUid = context.Message.PaymentUid;
        
        var payment = await _context.Payments
            .FirstOrDefaultAsync(x => x.PaymentUid == paymentUid);
        
        payment.Status = "CANCELED";
        
        _context.Payments.Update(payment);
        await _context.SaveChangesAsync();
    }
}