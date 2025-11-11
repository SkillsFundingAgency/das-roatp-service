using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Application.Services;
using SFA.DAS.RoATPService.Domain;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationsRepository(RoatpDataContext _dataContext) : IOrganisationsRepository
{
    public Task<Domain.Entities.Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .FirstOrDefaultAsync(o => o.Ukprn == ukprn, cancellationToken);

    public Task<List<Domain.Entities.Organisation>> GetOrganisations(CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .ToListAsync(cancellationToken);

    public async Task UpdateOrganisation(Domain.Entities.Organisation organisation, Audit audit, OrganisationStatusEvent statusEvent, bool removeNonStandardCourseTypes, string userId, CancellationToken cancellationToken)
    {
        if (statusEvent != null)
        {
            _dataContext.OrganisationStatusEvents.Add(statusEvent);
        }
        _dataContext.Organisations.Update(organisation);
        _dataContext.Audits.Add(audit);

        if (removeNonStandardCourseTypes)
        {
            List<OrganisationCourseType> courseTypesToRemove = await _dataContext.OrganisationCourseTypes.Where(o => o.OrganisationId == organisation.Id && o.CourseType.LearningType == LearningType.ShortCourse).ToListAsync(cancellationToken);

            _dataContext.OrganisationCourseTypes.RemoveRange(courseTypesToRemove);

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

            Audit auditRecord = new()
            {
                OrganisationId = organisation.Id,
                UpdatedBy = userId,
                UpdatedAt = DateTime.UtcNow,
                AuditData = auditData
            };

            _dataContext.Audits.Add(auditRecord);
        }

        await _dataContext.SaveChangesAsync(cancellationToken);
    }
}
