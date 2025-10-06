using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Domain.Repositories;

public interface IOrganisationsRepository
{
    Task<Entities.Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken);
}
