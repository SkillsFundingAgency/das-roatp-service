using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationCourseTypesRepository(RoatpDataContext context) : IOrganisationCourseTypesRepository
{
    public async Task UpdateOrganisationShortCourseTypes(Guid organisationId, IEnumerable<int> courseTypeIds, string userId, CancellationToken cancellationToken)
    {
        context.OrganisationCourseTypes.RemoveRange(context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId && o.CourseType.LearningType == Domain.Entities.LearningType.ShortCourse));

        context.OrganisationCourseTypes.AddRange(courseTypeIds.Select(c => new Domain.Entities.OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = c }));



        await context.SaveChangesAsync(cancellationToken);
    }
}
