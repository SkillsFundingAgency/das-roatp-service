using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.RoATPService.Domain.Entities;
using SFA.DAS.RoATPService.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.RoATPService.Application.Queries.GetAllCourseTypes;
public class GetAllCourseTypesQueryHandler(ICourseTypesRepository _courseTypesRepository, ILogger<GetAllCourseTypesQueryHandler> _logger) : IRequestHandler<GetAllCourseTypesQuery, GetAllCourseTypesResult>
{
    public async Task<GetAllCourseTypesResult> Handle(GetAllCourseTypesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling Get All Course Types");

        List<CourseType> courseType = await _courseTypesRepository.GetAllCourseTypes(cancellationToken);

        return new() { CourseTypes = courseType.Select(c => (CourseTypeSummary)c) };
    }
}