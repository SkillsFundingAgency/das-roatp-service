using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface IOrganisationRepository
{
    Task<Entities.Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken);
}
