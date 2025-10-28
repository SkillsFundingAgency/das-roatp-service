using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

internal class OrganisationCourseTypesRepository(RoatpDataContext context) : IOrganisationCourseTypesRepository
{
    public async Task UpdateOrganisationCourseTypes(Domain.Entities.Organisation organisation, IEnumerable<int> courseTypeIds, string userId, CancellationToken cancellationToken)
    {
        organisation.UpdatedAt = DateTime.UtcNow;
        organisation.UpdatedBy = userId;

        List<int> existingCourseTypes = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisation.Id).Select(o => o.CourseTypeId).ToListAsync(cancellationToken);

        context.OrganisationCourseTypes.RemoveRange(context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisation.Id));

        context.OrganisationCourseTypes.AddRange(courseTypeIds.Select(c => new OrganisationCourseType { Id = Guid.NewGuid(), OrganisationId = organisation.Id, CourseTypeId = c }));

        AuditLogEntry entry = new()
        {
            FieldChanged = "ShortCourseTypes",
            NewValue = string.Join(",", courseTypeIds),
            PreviousValue = string.Join(",", existingCourseTypes)
        };

        AuditData auditData = new()
        {
            OrganisationId = organisation.Id,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            FieldChanges = [entry]
        };

        Audit audit = new()
        {
            OrganisationId = organisation.Id,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = auditData
        };

        context.Audits.Add(audit);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteOrganisationShortCourseTypes(Domain.Entities.Organisation organisation, string userId, CancellationToken cancellationToken)
    {
        organisation.UpdatedAt = DateTime.UtcNow;
        organisation.UpdatedBy = userId;

        List<OrganisationCourseType> courseTypesToRemove = await context.OrganisationCourseTypes.Where(o => o.OrganisationId == organisation.Id && o.CourseType.LearningType == LearningType.ShortCourse).ToListAsync(cancellationToken);

        context.OrganisationCourseTypes.RemoveRange(courseTypesToRemove);

        List<int> removedCourseTypeIds = courseTypesToRemove.Select(c => c.CourseTypeId).ToList();

        AuditLogEntry entry = new()
        {
            FieldChanged = AuditLogField.CourseTypes,
            NewValue = null,
            PreviousValue = string.Join(",", removedCourseTypeIds)
        };

        AuditData auditData = new()
        {
            OrganisationId = organisation.Id,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            FieldChanges = [entry]
        };

        Audit audit = new()
        {
            OrganisationId = organisation.Id,
            UpdatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            AuditData = auditData
        };

        context.Audits.Add(audit);

        await context.SaveChangesAsync(cancellationToken);
    }
}
