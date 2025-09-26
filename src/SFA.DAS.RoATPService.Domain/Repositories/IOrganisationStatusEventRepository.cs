using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IOrganisationStatusEventRepository
{
    Task<OrganisationStatusEvent> GetLatestStatusChangeEvent(int ukprn, Entities.OrganisationStatus status, CancellationToken cancellationToken);
}
