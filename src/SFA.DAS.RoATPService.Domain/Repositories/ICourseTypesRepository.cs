using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Entities;

namespace SFA.DAS.RoATPService.Domain.Repositories;
public interface ICourseTypesRepository
{
    Task<List<CourseType>> GetAllCourseTypes(CancellationToken cancellationToken);
}
