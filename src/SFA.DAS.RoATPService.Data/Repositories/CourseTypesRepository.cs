using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class CourseTypesRepository(RoatpDataContext _dataContext) : ICourseTypesRepository
{
    public Task<List<Domain.Entities.CourseType>> GetAllCourseTypes(System.Threading.CancellationToken cancellationToken)
        => _dataContext.CourseTypes.ToListAsync(cancellationToken);
}
