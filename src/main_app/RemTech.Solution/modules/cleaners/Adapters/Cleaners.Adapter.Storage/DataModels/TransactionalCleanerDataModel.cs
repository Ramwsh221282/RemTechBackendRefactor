using Microsoft.EntityFrameworkCore.Storage;
using RemTech.Core.Shared.Result;

namespace Cleaners.Adapter.Storage.DataModels;

public sealed class TransactionalCleanerDataModel : ICleanerDataModel
{
    private readonly ICleanerDataModel _origin;
    private readonly IDbContextTransaction _txn;
    private readonly CleanersDbContext _context;

    public TransactionalCleanerDataModel(
        ICleanerDataModel origin,
        IDbContextTransaction txn,
        CleanersDbContext context
    )
    {
        _origin = origin;
        _txn = txn;
        _context = context;
    }

    public Guid Id
    {
        get => _origin.Id;
        set => _origin.Id = value;
    }
    public int CleanedAmount
    {
        get => _origin.CleanedAmount;
        set => _origin.CleanedAmount = value;
    }
    public DateTime? LastRun
    {
        get => _origin.LastRun;
        set => _origin.LastRun = value;
    }
    public DateTime? NextRun
    {
        get => _origin.NextRun;
        set => _origin.NextRun = value;
    }
    public int WaitDays
    {
        get => _origin.WaitDays;
        set => _origin.WaitDays = value;
    }
    public string State
    {
        get => _origin.State;
        set => _origin.State = value;
    }
    public int Hours
    {
        get => _origin.Hours;
        set => _origin.Hours = value;
    }
    public int Minutes
    {
        get => _origin.Minutes;
        set => _origin.Minutes = value;
    }
    public int Seconds
    {
        get => _origin.Seconds;
        set => _origin.Seconds = value;
    }

    public int ItemsDateDayThreshold
    {
        get => _origin.ItemsDateDayThreshold;
        set => _origin.ItemsDateDayThreshold = value;
    }

    public async Task<Status> Save(
        string onError,
        Serilog.ILogger logger,
        CancellationToken ct = default
    )
    {
        try
        {
            await _context.SaveChangesAsync(ct);
            await _txn.CommitAsync(ct);
            return Status.Success();
        }
        catch (Exception ex)
        {
            logger.Error("Error at saving cleaner: {message}", ex);
            await _txn.RollbackAsync(ct);
            return Status.Internal(onError);
        }
        finally
        {
            await _txn.DisposeAsync();
        }
    }
}
