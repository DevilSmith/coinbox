using coinbox_service.Web.DAL.Models;

namespace coinbox_service.Web.DAL.Repositories;

public interface ICoinsRepository: IDisposable 
{
    public Task PutCoinsAsync (uint countOfCoins);
    public Task<uint> GetCurrentNumberOfCoinsAsync();
    public Task TakeCoinsAsync(uint countOfCoinsTaken);
    public IQueryable<ChangesToCoins> GetChangesToCoins(uint countOfRecords);
    public IQueryable<NumberOfCoins> GetNumbersOfCoins(uint countOfRecords);
}