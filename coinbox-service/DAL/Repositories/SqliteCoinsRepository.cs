using coinbox_service.Web.DAL.Models;
using Microsoft.EntityFrameworkCore;
using WeatherArchiveApp.Models;

namespace coinbox_service.Web.DAL.Repositories;

public enum CoinsRepositoryErrors
{
    OutOfCoinsNumberBounds
}

public class CoinsRepositoryException : Exception
{
    public CoinsRepositoryErrors Error { get; }
    public CoinsRepositoryException(string message, CoinsRepositoryErrors error)
       : base(message)
    {
        Error = error;
    }
}

public class SqliteCoinsRepository : ICoinsRepository
{
    ApplicationContext db = new ApplicationContext();

    public void Dispose()
    {
        db.Dispose();
    }

    public IQueryable<ChangesToCoins> GetChangesToCoins(uint countOfRecords)
    {
        var partOfRecords = db.ChangesToCoins.AsNoTracking().OrderByDescending(r => r.CurrentDateTime).Take((int)countOfRecords);
        return partOfRecords;
    }

    public async Task<uint> GetCurrentNumberOfCoinsAsync()
    {
        var latestRecord = await db.NumberOfCoins.AsNoTracking().OrderByDescending(r => r.CurrentDateTime).FirstOrDefaultAsync();
        if (latestRecord is null) return 0;
        else return latestRecord.Count;
    }

    public IQueryable<NumberOfCoins> GetNumbersOfCoins(uint countOfRecords)
    {
        var partOfRecords = db.NumberOfCoins.AsNoTracking().OrderByDescending(r => r.CurrentDateTime).Take((int)countOfRecords);
        return partOfRecords;
    }

    public async Task PutCoinsAsync(uint countOfCoins)
    {
        var currentNumberOfCoins = await GetCurrentNumberOfCoinsAsync();

        var currentDateTime = DateTime.Now;

        var increasedNumberOfCoins = new NumberOfCoins()
        {
            Count = currentNumberOfCoins + countOfCoins,
            CurrentDateTime = currentDateTime
        };
        var newChangeToCoins = new ChangesToCoins()
        {
            Count = countOfCoins,
            CurrentDateTime = currentDateTime
        };

        await db.ChangesToCoins.AddAsync(newChangeToCoins);
        await db.NumberOfCoins.AddAsync(increasedNumberOfCoins);
        await db.SaveChangesAsync();
    }

    public async Task TakeCoinsAsync(uint countOfCoinsTaken)
    {
        var currentNumberOfCoins = await GetCurrentNumberOfCoinsAsync();

        var currentDateTime = DateTime.Now;

        try
        {
            var decreasedNumberOfCoins = new NumberOfCoins()
            {
                Count = checked(currentNumberOfCoins - countOfCoinsTaken),
                CurrentDateTime = currentDateTime
            };
            var newChangeToCoins = new ChangesToCoins()
            {
                Count = countOfCoinsTaken,
                CurrentDateTime = currentDateTime
            };

            await db.ChangesToCoins.AddAsync(newChangeToCoins);
            await db.NumberOfCoins.AddAsync(decreasedNumberOfCoins);
            await db.SaveChangesAsync();

        }
        catch (OverflowException)
        {
            throw new CoinsRepositoryException("Out of coins count bounds. Revert taking of coins", CoinsRepositoryErrors.OutOfCoinsNumberBounds);
        }
    }
}