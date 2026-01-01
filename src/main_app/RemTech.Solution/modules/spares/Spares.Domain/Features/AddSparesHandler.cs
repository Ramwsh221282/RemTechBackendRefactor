using RemTech.SharedKernel.Core.FunctionExtensionsModule;
using RemTech.SharedKernel.Core.Handlers;
using RemTech.SharedKernel.Core.Handlers.Attributes;
using Spares.Domain.Contracts;
using Spares.Domain.Models;

namespace Spares.Domain.Features;

[TransactionalHandler]
public sealed class AddSparesHandler(ISparesRepository repository) : ICommandHandler<AddSparesCommand, int>
{
    public async Task<Result<int>> Execute(AddSparesCommand command, CancellationToken ct = default)
    {
        Result<Spare[]> spares = CreateSpares(command.Spares);
        int addedCount = await repository.AddMany(spares.Value);
        return addedCount;
    }

    public static Spare[] CreateSpares(IEnumerable<AddSpareCommandPayload> spareInfo)
    {
        return spareInfo.Select(info =>
                SparesFactory.Create(
                    spareId: info.SpareId,
                    containedItemId: info.ContainedItemId,
                    source: info.Source,
                    oem: info.Oem,
                    title: info.Title,
                    price: info.Price,
                    isNds: info.IsNds,
                    type: info.Type,
                    address: info.Address,
                    photoPaths: info.PhotoPaths))
            .Where(r => r.IsSuccess)
            .Select(s => s.Value)
            .ToArray(); 
    }
}