using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IOrganisationStatusEventsRepository
{
    Task<OrganisationStatusEvent> GetLatestStatusChangeEvent(int ukprn, Entities.OrganisationStatus status, CancellationToken cancellationToken);
}
