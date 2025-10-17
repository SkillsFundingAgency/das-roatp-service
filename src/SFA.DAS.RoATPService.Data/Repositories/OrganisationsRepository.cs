﻿using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;

namespace SFA.DAS.RoATPService.Data.Repositories;

[ExcludeFromCodeCoverage]
internal class OrganisationsRepository(RoatpDataContext _dataContext) : IOrganisationsRepository
{
    public Task<Organisation> GetOrganisationByUkprn(int ukprn, CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .FirstOrDefaultAsync(o => o.Ukprn == ukprn, cancellationToken);

    public Task<List<Organisation>> GetOrganisations(CancellationToken cancellationToken)
        => _dataContext
            .Organisations
            .Include(o => o.RemovedReason)
            .Include(o => o.OrganisationType)
            .Include(o => o.OrganisationCourseTypes)
            .ThenInclude(oc => oc.CourseType)
            .ToListAsync(cancellationToken);
}
