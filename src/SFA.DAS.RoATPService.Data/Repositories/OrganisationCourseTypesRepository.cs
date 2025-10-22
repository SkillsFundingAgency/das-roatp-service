using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Data.Repositories;

internal class OrganisationCourseTypesRepository(RoatpDataContext context) : IOrganisationCourseTypesRepository
{
    public async Task UpdateOrganisationCourseTypes(Guid organisationId, IEnumerable<int> courseTypeIds, string userId, CancellationToken cancellationToken)
    {
        List<int> existingCourseTypes = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId).Select(o => o.CourseTypeId).ToListAsync(cancellationToken);

        context.OrganisationCourseTypes.RemoveRange(context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId));

        context.OrganisationCourseTypes.AddRange(courseTypeIds.Select(c => new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisationId, CourseTypeId = c }));

        AuditLogEntry entry = new()
        {
            FieldChanged = "ShortCourseTypes",
            NewValue = string.Join(",", courseTypeIds),
            PreviousValue = string.Join(",", existingCourseTypes)
        };

        AuditData auditData = new()
        {
            OrganisationId = organisationId,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            FieldChanges = [entry]
        };

        Audit audit = new()
        {
            OrganisationId = organisationId,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = auditData
        };

        context.Audits.Add(audit);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrganisationShortCourseTypes(Guid organisationId, string userId, CancellationToken cancellationToken)
    {
        List<int> existingCourseTypes = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId && o.CourseType.LearningType == LearningType.ShortCourse).Select(o => o.CourseTypeId).ToListAsync(cancellationToken);

        context.OrganisationCourseTypes.RemoveRange(context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisationId && o.CourseType.LearningType == LearningType.ShortCourse));

        AuditLogEntry entry = new()
        {
            FieldChanged = "Course Types",
            NewValue = null,
            PreviousValue = string.Join(",", existingCourseTypes)
        };

        AuditData auditData = new()
        {
            OrganisationId = organisationId,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            FieldChanges = [entry]
        };

        Audit audit = new()
        {
            OrganisationId = organisationId,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = auditData
        };

        context.Audits.Add(audit);

        await context.SaveChangesAsync(cancellationToken);
    }
}
