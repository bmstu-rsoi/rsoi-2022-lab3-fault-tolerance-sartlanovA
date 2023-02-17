using Microsoft.EntityFrameworkCore;
using Rentals.ModelsDB;

namespace Rentals.Repositories
{
    public class RentalsRepository : IRentalsRepository, IDisposable
    {
        private readonly RentalContext _db;
        private readonly ILogger<RentalsRepository> _logger;

        public RentalsRepository(RentalContext createDb, ILogger<RentalsRepository> logDb)
        {
            _db = createDb;
            _logger = logDb;
        }

        public async Task<List<Rental>> FindAll(int page, int size)
        {
            try
            {
                var rentals = await _db.Rentals
                    .OrderBy(x => x.Id)
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();
                return rentals;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsRep : Error while trying to FindAll");
                throw;
            }
        }

        public async Task<List<Rental>> FindByName(string username)
        {
            try
            {
                var rentals = await _db.Rentals
                    .Where(x => x.Username == username)
                    .OrderBy(x => x.Id)
                    .ToListAsync();
                return rentals;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsRep : Error while trying to FindByName");
                throw;
            }
        }

        public async Task<Rental?> FindByRentalUid(string username, Guid RentalUid)
        {
            try
            {
                var rental = await _db.Rentals
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Username == username && x.RentalUid.Equals(RentalUid));
                return rental;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsRep : Error while trying to FindByRentalUid");
                throw;
            }
        }

        public async Task<Rental> Add(Rental obj)
        {
            try
            {
                var id = _db.Rentals.Count() + 1;
                obj.Id = id;

                if (obj.RentalUid == default)
                    obj.RentalUid = Guid.NewGuid();
                if (obj.PaymentUid == default)
                    obj.PaymentUid = Guid.NewGuid();
                
                _db.Rentals.Add(obj);
                await _db.SaveChangesAsync();
                
                return obj;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsRep : Error while trying to Add to Rentals");
                throw;
            }
        }

        public async Task Patch(Rental obj)
        {
            try
            {
                _db.Rentals.Update(obj);
                await _db.SaveChangesAsync();
            
                _logger.LogInformation("+RentalsRep : Rental {Number} was patched at Rentals", obj.Id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "+RentalsRep : Error while trying to Patch to Rentals");
                throw;
            }
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}